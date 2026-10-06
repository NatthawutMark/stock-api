using Microsoft.AspNetCore.Mvc;
using System.Data;
using stock_api.Repositories.Dapper;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Controllers;

[ApiController]
// [Authorize]
[Route("api/[controller]")]
public class MastVendorController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;


    public MastVendorController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastVendor")]
    public async Task<ActionResult<List<MastVendorResponse>>> Get(MastVendorRequest req)
    {
        try
        {
            req.isActive = true;
            req.isDelete = false;

            List<MastVendorResponse> res = await _unitOfWork.Vendors.list(req);

            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = true, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("create", Name = "CreateMastVendor")]
    public async Task<ActionResult> Create([FromBody] MastVendorRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.vendCode) && string.IsNullOrEmpty(req.vendName))
            {
                return return200(false, null, "No data To Create", "Vendor code or name is empty");
            }

            MastVendorResponse? checkDuplicate = await _unitOfWork.Vendors.GetByCode(true, req.vendCode);
            if (checkDuplicate != null)
            {
                return return200(false, null, "Duplicate Data", "Vendor code is already exists");
            }
            string userId = _systemService.GetUserId();

            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.UpdateBy = userId;

            var result = await _unitOfWork.Vendors.create(req);
            if (result is OkObjectResult okResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Create MastVendor Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastVendor Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastVendor Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastVendor", error = ex.Message });
        }
    }


    [HttpGet("getByCode", Name = "GetMastVendorByCode")]
    public async Task<ActionResult> GetByCode([FromQuery] string Code)
    {
        try
        {
            if (string.IsNullOrEmpty(Code))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            MastVendorResponse? res = await _unitOfWork.Vendors.GetByCode(false, Code);
            if (res != null)
            {
                return return200(true, res, "Get MastVendor Success", "");
            }
            else
            {
                return return200(false, null, "MastVendor Not Found", "");
            }

        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while fetching the MastVendor", error = ex.Message });
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
