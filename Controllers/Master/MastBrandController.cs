
using Microsoft.AspNetCore.Mvc;
using System.Data;
using stock_api.Repositories.Dapper;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MastBrandController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _dpUnitOfWork;
    private readonly ISystemService _systemService;


    public MastBrandController(IDbConnection dbConnection, UnitOfWork dpUnitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _dpUnitOfWork = dpUnitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastBrand")]
    public async Task<ActionResult<List<MastBrandResponse>>> Get(MastBrandRequest req)
    {
        try
        {
            req.isActive = true;
            req.isDelete = false;

            List<MastBrandResponse> res = await _dpUnitOfWork.Brands.list(req);

            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = true, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("create", Name = "CreateMastBrand")]
    public async Task<ActionResult> Create([FromBody] MastBrandRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.nameTh) && string.IsNullOrEmpty(req.nameEn))
            {
                return return200(false, null, "No data To Create", "NameTh and NameEn is empty");
            }

            MastBrandResponse? checkDuplicate = await _dpUnitOfWork.Brands.GetByName(true, req.nameTh);
            if (checkDuplicate != null)
            {
                return return200(false, null, "Duplicate Data", "NameTh is already exists");
            }

            req.id = _systemService.GenGUID();
            req.createBy = _systemService.GetUserId();
            req.UpdateBy = _systemService.GetUserId();

            var result = await _dpUnitOfWork.Brands.create(req);
            if (result is OkObjectResult okResult)
            {
                await _dpUnitOfWork.CompleteAsync();
                return return200(true, null, "Create MastBrand Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _dpUnitOfWork.Dispose();
                return return200(false, null, "Create MastBrand Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _dpUnitOfWork.Dispose();
                return return200(false, null, "Create MastBrand Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _dpUnitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastBrand", error = ex.Message });
        }
    }


    [HttpGet("getByName", Name = "GetMastBrandByName")]
    public async Task<ActionResult> GetByName([FromQuery] string name)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            MastBrandResponse? res = await _dpUnitOfWork.Brands.GetByName(false, name);
            if (res != null)
            {
                return return200(true, res, "Get MastBrand Success", "");
            }
            else
            {
                return return200(false, null, "MastBrand Not Found", "");
            }

        }
        catch (Exception ex)
        {
            _dpUnitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastBrand", error = ex.Message });
        }
    }

    #region Private Methods
    [NonAction]
    public ObjectResult return200(bool success = true, object results = null, string message = "", string error = "")
    {
        return StatusCode(200, new { success = success, results = results, message = message, error = error });
    }
    #endregion
}
