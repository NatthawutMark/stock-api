using Dapper;
using System.Data;
using stock_api.Interfaces;
using static stock_api.Response.AuthRes;
using System.Dynamic;
using stock_api.Models;

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
                           ua.username,
                           up.f_name AS fName, 
                           up.l_name AS lName, 
                           up.tel, 
                           up.email, 
                           ua.is_active AS IsActive   
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
    #region Refresh Token
    public async Task<int> AddRefreshTokenAsync(Refreshtoken token)
    {
        var sql = @"INSERT INTO refreshtoken (id, user_id, token, expiry_date, is_revoked, create_date)
                    VALUES (@Id, @UserId, @Token, @ExpiryDate, @IsRevoked, @CreateDate)";

        return await _connection.ExecuteAsync(sql, token, transaction: _transaction);
    }

    public async Task<Refreshtoken?> GetRefreshTokenAsync(string token)
    {
        var sql = @"SELECT id            AS Id,
                           user_id       AS UserId,
                           token         AS Token,
                           expiry_date   AS ExpiryDate,
                           is_revoked    AS IsRevoked,
                           create_date   AS CreateDate,
                           revoked_date  AS RevokedDate,
                           replaced_token AS ReplacedToken
                    FROM refreshtoken
                    WHERE token = @token";

        return await _connection.QueryFirstOrDefaultAsync<Refreshtoken>(sql, new { token }, transaction: _transaction);
    }

    /// <summary>ยกเลิก Token ใบเดียว (ใช้ตอน Rotation) พร้อมเก็บว่าถูกแทนที่ด้วย Token ใบไหน</summary>
    public async Task<int> RevokeRefreshTokenAsync(string token, string? replacedToken = null)
    {
        var sql = @"UPDATE refreshtoken
                    SET is_revoked = true,
                        revoked_date = @revokedDate,
                        replaced_token = @replacedToken
                    WHERE token = @token";

        return await _connection.ExecuteAsync(sql, new { token, replacedToken, revokedDate = DateTime.Now }, transaction: _transaction);
    }

    /// <summary>ยกเลิก Token ที่ยังใช้งานได้ทั้งหมดของ User (ใช้ตอนตรวจพบการนำ Token เก่ามาใช้ซ้ำ)</summary>
    public async Task<int> RevokeAllRefreshTokensByUserAsync(string userId)
    {
        var sql = @"UPDATE refreshtoken
                    SET is_revoked = true,
                        revoked_date = @revokedDate
                    WHERE user_id = @userId AND is_revoked = false";

        return await _connection.ExecuteAsync(sql, new { userId, revokedDate = DateTime.Now }, transaction: _transaction);
    }

    /// <summary>ลบ Token ที่หมดอายุแล้ว หรือถูกยกเลิกไปนานเกินจำนวนวันที่กำหนด (ใช้ใน TokenCleanupService)</summary>
    public async Task<int> DeleteExpiredRefreshTokensAsync(int revokedOlderThanDays = 3)
    {
        var sql = @"DELETE FROM refreshtoken
                    WHERE expiry_date < @now
                       OR (is_revoked = true AND create_date < @oldDate)";

        var now = DateTime.Now;
        return await _connection.ExecuteAsync(sql, new { now, oldDate = now.AddDays(-revokedOlderThanDays) }, transaction: _transaction);
    }
    #endregion
}