
using static stock_api.Response.AuthRes;
using System.Dynamic;

namespace stock_api.Interfaces;

public interface IAuthRepository
{
    Task<userLogin?> LoginAsync(ExpandoObject login);

    Task<roles?> GetRole(string userid);
}