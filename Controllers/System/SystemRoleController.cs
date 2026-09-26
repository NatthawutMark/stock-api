using Microsoft.AspNetCore.Mvc;

namespace stock_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemRoleController : ControllerBase
{

    [HttpGet("getId")]
    public List<string> Get()
    {
        List<string> listGuid = new List<string>();
        for(int i = 0; i < 10; i++)
        {
            Guid guid = Guid.NewGuid();
            listGuid.Add(guid.ToString().ToUpper().Replace("-", ""));
        }
        return listGuid.ToList();
    }

    public string GenGUID()
    {
        Guid guid = Guid.NewGuid();
        return guid.ToString().ToUpper().Replace("-", "");
    }

    
}
