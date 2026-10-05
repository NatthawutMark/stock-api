using Microsoft.AspNetCore.Mvc;
using stock_api.Interfaces;
using stock_api.Repositories.Dapper;

namespace stock_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class testController : ControllerBase
{
    private readonly UnitOfWork _dpUnitOfwork;

    public testController(UnitOfWork unitOfWork)
    {
        _dpUnitOfwork = unitOfWork;
    }

    [HttpGet("getId", Name = "GetGUIDList")]
    public List<string> Get()
    {
        List<string> listGuid = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            Guid guid = Guid.NewGuid();
            listGuid.Add(guid.ToString().ToUpper().Replace("-", ""));
        }
        return listGuid.ToList();
    }


    [HttpGet("getUserByid/{id}")]
    public async Task<ActionResult> GetUserByid(string id)
    {

        var res = await _dpUnitOfwork.Auths.GetUserByIdAsync(id);

        return StatusCode(200, new { data = res });
    }

}
