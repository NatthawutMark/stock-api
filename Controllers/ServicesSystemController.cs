using Microsoft.AspNetCore.Mvc;

namespace stock_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesSystemController : ControllerBase
{

    [HttpGet("getId",Name = "GetGUIDList")]
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
}
