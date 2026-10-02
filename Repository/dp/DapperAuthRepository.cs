using Dapper;
using System.Data;
using stock_api.Interfaces;
using static stock_api.Response.AuthRes;
using System.Dynamic;

namespace stock_api.Repositories.Dapper;

public class DapperAuthRepository : IAuthRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DapperAuthRepository(IDbConnection connection, IDbTransaction? transaction = null)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<userLogin?> LoginAsync(ExpandoObject login)
    {
        var sql = @"SELECT ua.user_id AS userid,
        ua.username, up.f_name AS fName, up.l_name AS lName, up.tel, up.email, ua.is_active AS IsActive   
                    FROM user_authen ua
                    inner join user_profile up on ua.user_id = up.id
                    WHERE ua.username = @username AND ua.password = @password";

        return await _connection.QueryFirstOrDefaultAsync<userLogin>(sql, login, transaction: _transaction);
    }
    public async Task<userLogin?> GetUserByIdAsync(string userId)
    {
        var sql = @"SELECT ua.user_id AS userid, 
                    ua.username,
                    up.f_name AS fName, 
                    up.l_name AS lName, 
                    up.tel, 
                    up.email, 
                    ua.is_active AS IsActive   
                    FROM user_authen ua
                    inner join user_profile up on ua.user_id = up.id
                    WHERE ua.user_Id = @userId";

        return await _connection.QueryFirstOrDefaultAsync<userLogin>(sql, new { userId }, transaction: _transaction);
    }
    public async Task<roles?> GetListRole(string userid)
    {
        var sql = @"select sr.id as roleId,name_th as roleTh,name_en as roleEn from sys_role sr
                    inner join sys_role_user sru on sr.id = sru.role_id  
                    where sru.user_id  = @userid";

        return await _connection.QueryFirstOrDefaultAsync<roles>(sql, new { userid }, transaction: _transaction);
    }
    public async Task<IEnumerable<Menus>> GetMenuByUserId(string userid)
    {
        try
        {
            var sql = @"select menu.id as menuID, 
                        menu.parent_Id as parentID,
                        menu.name_th as NameTh,
                        menu.name_en as NameEn,
                        menu.url,
                        menu.order_no as orderNo,
                        menu.is_active as isActive
                    from sys_menu_user mu
                    inner join sys_menu menu on mu.menu_id = menu.id
                    where mu.user_id = @userid and mu.is_active = true";

            return await _connection.QueryAsync<Menus>(sql, new { userid }, transaction: _transaction);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving menus for user {userid}: {ex.Message}", ex);
        }
    }

}