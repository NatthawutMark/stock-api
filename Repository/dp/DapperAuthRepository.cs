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
        ua.username, up.f_name AS fname, up.l_name AS lname, up.tel, up.email, ua.is_active AS IsActive   
                    FROM user_authen ua
                    inner join user_profile up on ua.user_id = up.id
                    WHERE ua.username = @username AND ua.password = @password";

        return await _connection.QueryFirstOrDefaultAsync<userLogin>(sql, login, transaction: _transaction);
    }
    public async Task<roles?> GetRole(string userid)
    {
        var sql = @"select sr.id as roleId,name_th as roleTh,name_en as roleEn from sys_role sr
                    inner join sys_role_user sru on sr.id = sru.role_id  
                    where sru.user_id  = @userid";

        return await _connection.QueryFirstOrDefaultAsync<roles>(sql, new { userid }, transaction: _transaction);
    }
}