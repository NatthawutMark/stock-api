using System.Data;
using System.Security.Claims;
using Dapper;
using Npgsql;
using stock_api.Interfaces;
using static stock_api.Response.AuthRes;

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
}