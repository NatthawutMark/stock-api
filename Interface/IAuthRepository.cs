
using static stock_api.Response.AuthRes;
using System.Dynamic;
using stock_api.Models;

namespace stock_api.Interfaces;

public interface IAuthRepository
{
    Task<userLogin?> LoginAsync(ExpandoObject login);
    Task<userLogin?> GetUserByIdAsync(string userId);
    Task<roles?> GetListRole(string userid);
    Task<IEnumerable<Menus>> GetMenuByUserId(string userid);

    // Refresh Token
    Task<int> AddRefreshTokenAsync(Refreshtoken token);
    Task<Refreshtoken?> GetRefreshTokenAsync(string token);
    Task<int> RevokeRefreshTokenAsync(string token, string? replacedToken = null);
    Task<int> RevokeAllRefreshTokensByUserAsync(string userId);
    Task<int> DeleteExpiredRefreshTokensAsync(int revokedOlderThanDays = 3);
}