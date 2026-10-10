using Microsoft.AspNetCore.Mvc;
using System.Data;
using stock_api.Repositories.Dapper;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;
using static stock_api.Response.AuthRes;

namespace stock_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MastDocTypeController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;

    public MastDocTypeController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastDocType")]
    public async Task<ActionResult<List<MastDocTypeResponse>>> Get(MastDocTypeRequest req)
    {
        try
        {
            req.isDelete = false;

            List<MastDocTypeResponse> res = await _unitOfWork.DocTypes.list(req);

            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = true, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("create", Name = "CreateMastDocType")]
    public async Task<ActionResult> Create([FromBody] MastDocTypeRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.docTypeName))
            {
                return return200(false, null, "No data To Create", "DocType name is empty");
            }

            MastDocTypeResponse? checkDuplicate = await _unitOfWork.DocTypes.GetByName(true, req.docTypeName, req.menuId ?? "");
            if (checkDuplicate != null)
            {
                return return200(false, null, "Duplicate Data", "DocType name is already exists");
            }
            string userId = _systemService.GetUserId();

            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.UpdateBy = userId;

            var result = await _unitOfWork.DocTypes.create(req);
            if (result is OkObjectResult okResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Create MastDocType Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastDocType Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastDocType Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastDocType", error = ex.Message });
        }
    }

    [HttpGet("getByName", Name = "GetMastDocTypeByName")]
    public async Task<ActionResult> GetByName([FromQuery] string Name)
    {
        try
        {
            if (string.IsNullOrEmpty(Name))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            MastDocTypeResponse? res = await _unitOfWork.DocTypes.GetByName(false, Name, "");
            if (res != null)
            {
                return return200(true, res, "Get MastDocType Success", "");
            }
            else
            {
                return return200(false, null, "MastDocType Not Found", "");
            }

        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while fetching the MastDocType", error = ex.Message });
        }
    }

    [HttpPost("update", Name = "UpdateMastDocType")]
    public async Task<ActionResult> Update([FromBody] MastDocTypeRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.id))
            {
                return BadRequest(new { status = false, message = "Invalid request data. DocType ID is required." });
            }

            string userId = _systemService.GetUserId();

            req.UpdateBy = userId;
            req.UpdateDate = DateTime.Now;

            var result = await _unitOfWork.DocTypes.update(req);
            if (result is OkObjectResult okResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, " แก้ไขข้อมูลสำเร็จ", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update MastDocType Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update MastDocType Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while updating the MastDocType", error = ex.Message });
        }
    }

    [HttpGet("getTransactionMenu", Name = "GetTransactionMenuForDocType")]
    public async Task<ActionResult> GetTransactionMenu([FromQuery] string? userId)
    {
        try
        {
            var res = await _systemService.GetTransactionMenuByUser(userId);
            return return200(true, res, "Get Transaction Menu Success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = false, message = "An error occurred while fetching Transaction Menu", error = ex.Message });
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

