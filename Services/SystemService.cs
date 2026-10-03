using Microsoft.AspNetCore.Mvc;
using stock_api.Interfaces;

namespace stock_api.Services;

public class SystemService : ISystemService
{
    [NonAction]
    public string GenGUID()
    {
        return Guid.NewGuid().ToString().ToUpper().Replace("-", "");
    }
    [NonAction]
    public List<string> GetGUIDList(int count = 10)
    {
        var listGuid = new List<string>();
        for (int i = 0; i < count; i++)
        {
            listGuid.Add(GenGUID());
        }
        return listGuid;
    }
}