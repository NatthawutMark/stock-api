
using static stock_api.Response.AuthRes;
using System.Dynamic;

namespace stock_api.Interfaces;

public interface IAuthRepository
{
    Task<userLogin?> LoginAsync(ExpandoObject login);
    Task<userLogin?> GetUserByIdAsync(string userId);

    Task<roles?> GetListRole(string userid);
    Task<IEnumerable<Menus>> GetMenuByUserId(string userid);
}