using Microsoft.AspNetCore.Mvc;
using System.Data;
using stock_api.Repositories.Dapper;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static stock_api.Request.TransactionRequest;
using static stock_api.Response.TransactionResponse;

namespace stock_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DocTransController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;

    public DocTransController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListDocTrans")]
    public async Task<ActionResult<List<DocTransResponse>>> List([FromBody] DocTransListRequest req)
    {
        try
        {
            req ??= new DocTransListRequest();
            req.isDelete = false;

            List<DocTransResponse> res = await _unitOfWork.DocTrans.List(req);
            return return200(true, res, "Get DocTrans list success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("create", Name = "CreateDocTrans")]
    public async Task<ActionResult> Create([FromBody] DocTransCreateRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.menuId))
            {
                return return200(false, null, "Validation Error", "menuId is required");
            }

            // if (string.IsNullOrEmpty(req.docTypeId))
            // {
            //     return return200(false, null, "Validation Error", "docTypeId is required");
            // }

            // ถ้าไม่มี docNo ให้สร้างอัตโนมัติจาก GetDocNumber
            // if (string.IsNullOrWhiteSpace(req.docNo))
            // {
            req.docNo = await _systemService.GetDocNumber(new DocNumber
            {
                menuId = req.menuId,
                docTypeId = req.docTypeId
            });
            // }
            // else
            // {
            //     // ตรวจสอบเลขที่เอกสารซ้ำ
            //     var checkDoc = await _unitOfWork.DocTrans.GetByDocNo(req.docNo);
            //     if (checkDoc != null)
            //     {
            //         return return200(false, null, "Duplicate Document Number", $"DocNo '{req.docNo}' already exists");
            //     }
            // }

            string userId = _systemService.GetUserId() ?? "system";

            if (string.IsNullOrWhiteSpace(req.id))
            {
                req.id = _systemService.GenGUID();
            }

            req.createBy = userId;
            req.updateBy = userId;

            if (string.IsNullOrWhiteSpace(req.statusCode))
            {
                req.statusCode = "DRAFT";
            }

            var result = await _unitOfWork.DocTrans.Create(req);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, new { id = req.id, docNo = req.docNo }, "Create DocTrans Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create DocTrans Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create DocTrans Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating DocTrans", error = ex.Message });
        }
    }

    [HttpGet("getById", Name = "GetDocTransById")]
    public async Task<ActionResult> GetById([FromQuery] string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return return200(false, null, "Invalid request", "Document ID is required");
            }

            DocTransResponse? res = await _unitOfWork.DocTrans.GetById(id);
            if (res != null)
            {
                return return200(true, res, "Get DocTrans success", "");
            }
            else
            {
                return return200(false, null, "DocTrans not found", "");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpGet("getByDocNo", Name = "GetDocTransByDocNo")]
    public async Task<ActionResult> GetByDocNo([FromQuery] string docNo)
    {
        try
        {
            if (string.IsNullOrEmpty(docNo))
            {
                return return200(false, null, "Invalid request", "Document number is required");
            }

            DocTransResponse? res = await _unitOfWork.DocTrans.GetByDocNo(docNo);
            if (res != null)
            {
                return return200(true, res, "Get DocTrans success", "");
            }
            else
            {
                return return200(false, null, "DocTrans not found", "");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpGet("getLogs", Name = "GetDocTransLogs")]
    public async Task<ActionResult> GetLogs([FromQuery] string docId)
    {
        try
        {
            if (string.IsNullOrEmpty(docId))
            {
                return return200(false, null, "Invalid request", "DocId is required");
            }

            List<DocTransLogResponse> logs = await _unitOfWork.DocTrans.GetLogs(docId);
            return return200(true, logs, "Get DocTrans logs success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("update", Name = "UpdateDocTrans")]
    public async Task<ActionResult> Update([FromBody] DocTransUpdateRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.id))
            {
                return BadRequest(new { status = false, message = "Invalid request data. Document ID is required." });
            }

            string userId = _systemService.GetUserId() ?? "system";
            req.updateBy = userId;

            var result = await _unitOfWork.DocTrans.Update(req);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, req.id, "Update DocTrans Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update DocTrans Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update DocTrans Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while updating DocTrans", error = ex.Message });
        }
    }

    [HttpPost("updateStatus", Name = "UpdateDocTransStatus")]
    public async Task<ActionResult> UpdateStatus([FromBody] DocTransUpdateStatusRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.docId) || string.IsNullOrEmpty(req.statusCode))
            {
                return BadRequest(new { status = false, message = "docId and statusCode are required." });
            }

            string userId = _systemService.GetUserId() ?? "system";

            var result = await _unitOfWork.DocTrans.UpdateStatus(req, userId);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, req.docId, $"Update status to {req.statusCode} Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update Status Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update Status Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while updating status", error = ex.Message });
        }
    }

    [HttpPost("delete", Name = "DeleteDocTrans")]
    public async Task<ActionResult> Delete([FromBody] DocTransDeleteRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.id))
            {
                return BadRequest(new { status = false, message = "Document ID is required." });
            }

            string userId = _systemService.GetUserId() ?? "system";

            var result = await _unitOfWork.DocTrans.Delete(req.id, userId);
            if (result is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, req.id, "Delete DocTrans Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Delete DocTrans Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Delete DocTrans Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while deleting DocTrans", error = ex.Message });
        }
    }

    [HttpPost("generateDocNo", Name = "GenerateDocNo")]
    public async Task<ActionResult> GenerateDocNo([FromBody] DocNumber req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.menuId) || string.IsNullOrEmpty(req.docTypeId))
            {
                return return200(false, null, "Invalid request", "menuId and docTypeId are required");
            }

            string docNo = await _systemService.GetDocNumber(req);
            return return200(true, new { docNo }, "Generate DocNumber Success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = false, message = "An error occurred while generating DocNumber", error = ex.Message });
        }
    }

    #region Private Methods
    [NonAction]
    public ObjectResult return200(bool success = true, object? results = null, string message = "", string error = "")
    {
        return StatusCode(200, new { success = success, results = results, message = message, error = error });
    }
    #endregion
}

