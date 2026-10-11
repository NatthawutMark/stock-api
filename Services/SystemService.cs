using System.Data;
using System.Security.Claims;
using Dapper;
using Npgsql;
using stock_api.Interfaces;
using static stock_api.Response.AuthRes;
using static stock_api.Request.TransactionRequest;
using Microsoft.AspNetCore.Routing.Matching;
using static stock_api.Response.RunningDocResponse;

namespace stock_api.Services;

public class SystemService : ISystemService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbConnection _connection;
    public SystemService(IHttpContextAccessor httpContextAccessor, IDbConnection connection)
    {
        _httpContextAccessor = httpContextAccessor;
        _connection = connection;
    }

    public string GenGUID()
    {
        return Guid.NewGuid().ToString().ToUpper().Replace("-", "");
    }

    public List<string> GetGUIDList(int count = 10)
    {
        var listGuid = new List<string>();
        for (int i = 0; i < count; i++)
        {
            listGuid.Add(GenGUID());
        }
        return listGuid;
    }

    /// <summary>
    /// ดึง UserId ของผู้ใช้ที่ Login อยู่ จาก Claim NameIdentifier ใน JWT Access Token
    /// </summary>
    public string? GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    /// <summary>
    /// ดึง Username ของผู้ใช้ที่ Login อยู่ จาก Claim Name ใน JWT Access Token
    /// </summary>
    public string? GetUsername()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
    }

    /// <summary>
    /// ดึงเมนูของผู้ใช้ปัจจุบันที่ Login อยู่ (จาก JWT Token)
    /// </summary>
    public async Task<List<Menus>?> GetMenuByUser()
    {
        var currentUserId = GetUserId() ?? GetUsername();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return null;
        }

        return await GetMenuByUser(currentUserId);
    }

    /// <summary>
    /// ดึงเมนูของผู้ใช้โดยส่งข้อมูล userLogin เข้ามา
    /// </summary>
    public async Task<List<Menus>?> GetMenuByUser(userLogin user)
    {
        if (user == null)
        {
            return null;
        }

        var identifier = !string.IsNullOrEmpty(user.userid) ? user.userid : user.username;
        if (string.IsNullOrEmpty(identifier))
        {
            return null;
        }

        return await GetMenuByUser(identifier);
    }

    /// <summary>
    /// ดึงเมนูของผู้ใช้โดยระบุ userId หรือ username
    /// </summary>
    public async Task<List<Menus>?> GetMenuByUser(string userIdentifier)
    {
        if (string.IsNullOrWhiteSpace(userIdentifier))
        {
            return null;
        }

        const string sql = @"
            SELECT menu.id AS menuID, 
                   menu.parent_id AS parentID,
                   menu.name_th AS NameTh,
                   menu.name_en AS NameEn,
                   menu.url,
                   menu.order_no AS orderNo,
                   menu.is_active AS isActive,
                   menu.menu_type AS menuType,
                   menu.icon
            FROM sys_menu_user mu
            INNER JOIN sys_menu menu ON mu.menu_id = menu.id
            LEFT JOIN user_authen ua ON mu.user_id = ua.user_id
            WHERE (mu.user_id = @userIdentifier OR ua.username = @userIdentifier)
              AND mu.is_active = true
              AND (mu.is_delete = false OR mu.is_delete IS NULL)
              AND menu.is_active = true
              AND (menu.is_delete = false OR menu.is_delete IS NULL)";

        var resMenus = (await _connection.QueryAsync<Menus>(sql, new { userIdentifier = userIdentifier.Trim() })).ToList();

        if (!resMenus.Any())
        {
            return null;
        }

        var menuLookup = resMenus.ToLookup(m => m.parentID);

        List<Menus>? BuildMenuTree(string? currentParentId)
        {
            var children = menuLookup[currentParentId].ToList();
            if (!children.Any())
                return null;

            foreach (var child in children)
            {
                child.subMenus = BuildMenuTree(child.menuID);
            }

            return children.OrderBy(m => m.orderNo).ToList();
        }

        var rootMenus = resMenus
            .Where(m => string.IsNullOrEmpty(m.parentID) || !resMenus.Any(p => p.menuID == m.parentID))
            .OrderBy(m => m.orderNo)
            .ToList();

        foreach (var root in rootMenus)
        {
            root.subMenus = BuildMenuTree(root.menuID);
        }

        return rootMenus.Any() ? rootMenus : null;
    }

    /// <summary>
    /// ดึงรายการเมนูประเภท Transaction สำหรับ Dropdown/Select ของผู้ใช้ที่ Login อยู่ (จาก JWT Token)
    /// </summary>
    public async Task<List<TransactionMenuResponse>> GetTransactionMenuByUser()
    {
        var currentUserId = GetUserId() ?? GetUsername();
        return await GetTransactionMenuByUser(currentUserId);
    }

    /// <summary>
    /// ดึงรายการเมนูประเภท Transaction สำหรับ Dropdown/Select โดยส่งข้อมูล userLogin เข้ามา
    /// </summary>
    public async Task<List<TransactionMenuResponse>> GetTransactionMenuByUser(userLogin user)
    {
        if (user == null)
        {
            return new List<TransactionMenuResponse>();
        }

        var identifier = !string.IsNullOrEmpty(user.userid) ? user.userid : user.username;
        return await GetTransactionMenuByUser(identifier);
    }

    /// <summary>
    /// ดึงรายการเมนูประเภท Transaction สำหรับ Dropdown/Select โดยระบุ userId หรือ username
    /// </summary>
    public async Task<List<TransactionMenuResponse>> GetTransactionMenuByUser(string? userIdentifier)
    {
        userIdentifier ??= GetUserId() ?? GetUsername();
        if (string.IsNullOrWhiteSpace(userIdentifier))
        {
            return new List<TransactionMenuResponse>();
        }

        const string sql = @"
            SELECT 
                menu.id AS Id,
                menu.name_th AS nameTh,
                menu.name_en AS nameEn,
                CONCAT(menu.name_th, '(', menu.name_en, ')') AS menuName,
                menu.order_no AS orderNo
            FROM sys_menu_user mu
            INNER JOIN sys_menu menu ON mu.menu_id = menu.id
            LEFT JOIN user_authen ua ON mu.user_id = ua.user_id
            WHERE (mu.user_id = @userIdentifier OR ua.username = @userIdentifier)
              AND (UPPER(menu.menu_type) = 'TRANSACTION' OR menu.parent_id IN (SELECT id FROM sys_menu WHERE name_th = 'รายการเอกสาร' OR name_en ILIKE '%document%' OR name_en ILIKE '%transaction%'))
              AND mu.is_active = true
              AND (mu.is_delete = false OR mu.is_delete IS NULL)
              AND menu.is_active = true
              AND (menu.is_delete = false OR menu.is_delete IS NULL)
            ORDER BY menu.order_no";

        var result = await _connection.QueryAsync<TransactionMenuResponse>(sql, new { userIdentifier = userIdentifier.Trim() });
        return result.ToList();
    }

    public async Task<string> GetDocNumber(DocNumber req)
    {
        string CfgId = "";
        int _curruentNumber = 0;
        string _docNo = "";
        int _year = DateTime.Now.Year;
        int _month = DateTime.Now.Month;
        var _transaction = _connection.BeginTransaction();
        try
        {

            #region find CfgNumber
            string strCfgNumber = "Select * from config_doc_number where menu_id = @menuId ";
            if (!string.IsNullOrEmpty(req.docTypeId))
            {
                strCfgNumber += " and doc_type_id = @docTypeId";
            }
            var resCfg = _connection.QueryFirstOrDefault(strCfgNumber, req);
            #endregion

            if (resCfg == null)
            {
                CfgId = GenGUID();
                string strInsertCfg = @"INSERT INTO config_doc_number (id, menu_id, doc_type_id, prefix, digit, reset_period, pattern) 
                                    VALUES (@id, @menuId,  @docTypeId, @prefix, @digit, @resetPeriod, @Pattern) RETURNING *;";

                var resInsertCfg = await _connection.QuerySingleAsync<ConfigDocNumber>(strInsertCfg, new
                {
                    id = CfgId,
                    menuId = req.menuId,
                    docTypeId = !string.IsNullOrEmpty(req.docTypeId) ? req.docTypeId : null,
                    prefix = GetPrefixByMenu(req.menuId),
                    digit = 5,
                    resetPeriod = "MONTHLY",
                    Pattern = "{{PREFIX}}-{{YY}}{{MM}}{{NUMBER}}"
                });
                _transaction.Commit();

                #region Insert Running Doc
                string strInsertnum = @"INSERT INTO doc_number_running (id, config_id, running_year, running_month, current_number, last_doc_no, update_date) 
                                    VALUES (@id, @configId, @year, @month, @currentNumber, @LastDocNo, @updateDate)";

                _curruentNumber += 1;

                _docNo = resInsertCfg.Pattern
                        .Replace("{{PREFIX}}", resInsertCfg.prefix)
                        .Replace("{{YY}}", _year.ToString())
                        .Replace("{{MM}}", _month.ToString())
                        .Replace("{{NUMBER}}", _curruentNumber.ToString().PadLeft(resInsertCfg.digit, '0'));

                await _connection.QueryAsync(strInsertnum, new
                {
                    id = GenGUID(),
                    configId = CfgId,
                    year = _year,
                    month = _month,
                    currentNumber = _curruentNumber,
                    LastDocNo = _docNo,
                    updateDate = DateTime.Now
                });
                _transaction.Commit();
                #endregion
            }
            else
            {
                string strDocRunning = "SELECT * from doc_number_running where config_id = @configId and running_year = @year and running_month = @month";
                var resDocRunning = await _connection.QueryFirstOrDefaultAsync<DocNumberRunning>(strDocRunning, new { configId = resCfg.id, year = _year, month = _month });
                if (resDocRunning != null)
                {
                    _curruentNumber = (resDocRunning.CurrentNumber + 1);
                    _docNo = resCfg.Pattern
                            .Replace("{{PREFIX}}", resCfg.prefix)
                            .Replace("{{YY}}", resDocRunning.RunningYear.ToString())
                            .Replace("{{MM}}", resDocRunning.RunningMonth.ToString())
                            .Replace("{{NUMBER}}", _curruentNumber.ToString().PadLeft(resCfg.digit, '0'));

                    string updateDocRunning = "UPDATE doc_number_running SET current_number = @currentNumber, last_doc_no = @docNo where id = @id";

                    await _connection.QueryAsync(updateDocRunning, new { currentNumber = _curruentNumber, docNo = _docNo, id = resDocRunning.Id });
                    _transaction.Commit();
                }
            }
        }
        catch (Exception ex)
        {
            _transaction.Rollback();
            return "";
        }
        return _docNo;
    }



    public string GetPrefixByMenu(string menuId)
    {
        string prefix = "";
        switch (menuId)
        {
            case "00E03A994280450ABF7E2EE62A7AD4D6": // Receive
                prefix = "GR";
                break;
            case "85D356B7877647C49B61B71FB047E0B6": // Issue
                prefix = "GI";
                break;
            case "322D946D544B4A179EB9E09613CAC5DC": // Return
                prefix = "RT";
                break;
            case "680AB9E0D80D4806AD119476FA511EB4": // Stock Disposal
                prefix = "SD";
                break;
            case "92AEEB580AD14B848F4AB6CD294A17C5": // Trans-in
                prefix = "TF-IN";
                break;
            case "268A2969A63F4D448AEFB1C8CE64C527": // Trans-out
                prefix = "TF-OUT";
                break;
            default:
                prefix = "DOC";
                break;
        }
        return prefix;
    }
}