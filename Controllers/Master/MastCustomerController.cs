
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
public class MastCustomerController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;


    public MastCustomerController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastCustomer")]
    public async Task<ActionResult<List<MastCustomerResponse>>> Get(MastCustomerRequest req)
    {
        try
        {
            req.isActive = true;
            req.isDelete = false;

            List<MastCustomerResponse> res = await _unitOfWork.Customers.list(req);

            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = true, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("create", Name = "CreateMastCustomer")]
    public async Task<ActionResult> Create([FromBody] MastCustomerRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.custCode) && string.IsNullOrEmpty(req.custName))
            {
                return return200(false, null, "No data To Create", "Customer code or name is empty");
            }

            MastCustomerResponse? checkDuplicate = await _unitOfWork.Customers.GetByCode(true, req.custCode);
            if (checkDuplicate != null)
            {
                return return200(false, null, "Duplicate Data", "Customer code is already exists");
            }
            string userId = _systemService.GetUserId();

            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.UpdateBy = userId;

            var result = await _unitOfWork.Customers.create(req);
            if (result is OkObjectResult okResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Create MastCustomer Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastCustomer Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastCustomer Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastCustomer", error = ex.Message });
        }
    }


    [HttpGet("getByCode", Name = "GetMastCustomerByCode")]
    public async Task<ActionResult> GetByCode([FromQuery] string Code)
    {
        try
        {
            if (string.IsNullOrEmpty(Code))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            MastCustomerResponse? res = await _unitOfWork.Customers.GetByCode(false, Code);
            if (res != null)
            {
                return return200(true, res, "Get MastCustomer Success", "");
            }
            else
            {
                return return200(false, null, "MastCustomer Not Found", "");
            }

        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while fetching the MastCustomer", error = ex.Message });
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
