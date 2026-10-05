using System.Security.Claims;
using stock_api.Interfaces;

namespace stock_api.Services;

public class SystemService : ISystemService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SystemService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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
}