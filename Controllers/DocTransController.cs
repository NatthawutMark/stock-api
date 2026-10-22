using Microsoft.AspNetCore.Mvc;
using System.Data;
using stock_api.Repositories;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static stock_api.Request.TransactionRequest;
using static stock_api.Response.TransactionResponse;
using Microsoft.AspNetCore.Server.HttpSys;

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
            int total = await _unitOfWork.DocTrans.GetTotal(req.menuId);
            return return200(true, new { data = res, total }, "Get DocTrans list success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpGet("getTotal", Name = "GetDocTransTotal")]
    public async Task<ActionResult> GetTotal([FromQuery] string? menuId = null)
    {
        try
        {
            int total = await _unitOfWork.DocTrans.GetTotal(menuId);
            return return200(true, new { total }, "Get DocTrans total success", "");
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
            string userId = _systemService.GetUserId() ?? "system";
            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.updateBy = userId;

            if (string.IsNullOrWhiteSpace(req.statusCode))
            {
                req.statusCode = "DRAFT";
            }

            var result = await _unitOfWork.DocTrans.Create(req);
            if (result is OkObjectResult)
            {
                if (req.menuId == "00E03A994280450ABF7E2EE62A7AD4D6") // Receive
                {
                    List<DocReceiveDetailRequest> list = new List<DocReceiveDetailRequest>();
                    if (req.details != null)
                    {
                        foreach (var item in req.details)
                        {
                            list.Add(new DocReceiveDetailRequest
                            {
                                id = _systemService.GenGUID(),
                                docId = req.id,
                                itemId = item.itemId,
                                itemName = item.itemName,
                                lotNo = !string.IsNullOrEmpty(item.lotNo) ? item.lotNo.Trim() : null,
                                serialNo = !string.IsNullOrEmpty(item.serialNo) ? item.serialNo.Trim() : null,
                                locationId = item.locationId,
                                uomId = item.uomId,
                                mfgDate = item.mfgDate ?? null,
                                expireDate = item.expireDate ?? null,
                                itemQty = item.itemQty ?? 0,
                                itemPrice = item.itemPrice ?? 0,
                                putawayQty = 0,
                                createBy = userId,
                                updateBy = userId,
                            });
                        }
                    }
                    var resDetail = await _unitOfWork.DocReceives.CreateList(list);
                    if (resDetail is not OkObjectResult)
                    {
                        throw new Exception($"Error Insert Detail ");
                    }
                }
                else if (req.menuId == "85D356B7877647C49B61B71FB047E0B6") // Issue
                {
                    List<DocIssueDetailRequest> list = new List<DocIssueDetailRequest>();
                    if (req.details != null)
                    {
                        foreach (var item in req.details)
                        {
                            list.Add(new DocIssueDetailRequest
                            {
                                id = _systemService.GenGUID(),
                                docId = req.id,
                                invId = item.invId,
                                itemQty = item.itemQty ?? 0,
                                pickingQty = 0,
                                returnQty = 0,
                                createBy = userId,
                                updateBy = userId,
                            });
                        }
                    }
                    var resDetail = await _unitOfWork.DocIssue.CreateList(list);
                    if (resDetail is not OkObjectResult)
                    {
                        throw new Exception($"Error Insert Detail ");
                    }
                }
                else if (req.menuId == "322D946D544B4A179EB9E09613CAC5DC") // Return
                {

                }
                else if (req.menuId == "680AB9E0D80D4806AD119476FA511EB4") // Stock Disposal
                {

                }
                else if (req.menuId == "92AEEB580AD14B848F4AB6CD294A17C5") // Trans-in
                {

                }
                else if (req.menuId == "268A2969A63F4D448AEFB1C8CE64C527") // Trans-out
                {

                }
                else
                {
                    _unitOfWork.Dispose();
                    return return200(false, null, "มีปัญหาการเพิ่มการเพิ่มรายการสินค้า กรุณาติดต่อทีมงาน", "");
                }

                // เงื่อนไข: Transaction บันทึกข้อมูลสมบูรณ์แล้ว
                // ถ้าสถานะเป็น SAVE และยังไม่มี DocNo จึงจะสร้าง DocNo
                if (req.statusCode?.ToUpper() == "SAVE" && string.IsNullOrWhiteSpace(req.docNo))
                {
                    req.docNo = await _systemService.GetDocNumber(new DocNumber
                    {
                        menuId = req.menuId,
                        docTypeId = req.docTypeId
                    });

                    if (!string.IsNullOrEmpty(req.docNo))
                    {
                        await _unitOfWork.DocTrans.UpdateDocNo(req.id, req.docNo);
                    }
                }

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

            // ดึงข้อมูลเอกสารเดิมมาตรวจสอบ
            var existingDoc = await _unitOfWork.DocTrans.GetById(req.id);
            if (existingDoc == null)
            {
                return return200(false, null, "ไม่พบข้อมูลเอกสาร", "Document not found");
            }

            // ถ้ามี docNo เดิมอยู่แล้ว ให้รักษา docNo เดิมไว้เสมอ
            if (!string.IsNullOrWhiteSpace(existingDoc.docNo))
            {
                req.docNo = existingDoc.docNo;
            }

            var result = await _unitOfWork.DocTrans.Update(req);
            if (result is OkObjectResult)
            {
                // เงื่อนไข: Transaction บันทึกข้อมูลสมบูรณ์แล้ว
                // ถ้าสถานะเป็น SAVE และยังไม่มี DocNo ให้สร้าง DocNo
                if (req.statusCode?.ToUpper() == "SAVE" && string.IsNullOrWhiteSpace(req.docNo))
                {
                    req.docNo = await _systemService.GetDocNumber(new DocNumber
                    {
                        menuId = existingDoc.menuId,
                        docTypeId = req.docTypeId ?? existingDoc.docTypeId
                    });

                    if (!string.IsNullOrEmpty(req.docNo))
                    {
                        await _unitOfWork.DocTrans.UpdateDocNo(req.id, req.docNo);
                    }
                }

                await _unitOfWork.CompleteAsync();
                return return200(true, new { id = req.id, docNo = req.docNo }, "Update DocTrans Success", "");
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
                string? docNo = null;

                // เงื่อนไข: Transaction บันทึกเปลี่ยนสถานะสำเร็จแล้ว
                // ถ้าเปลี่ยนสถานะเป็น SAVE และเอกสารยังไม่มี DocNo จึงจะสร้าง DocNo
                if (req.statusCode?.ToUpper() == "SAVE")
                {
                    var existingDoc = await _unitOfWork.DocTrans.GetById(req.docId);
                    if (existingDoc != null && string.IsNullOrWhiteSpace(existingDoc.docNo))
                    {
                        docNo = await _systemService.GetDocNumber(new DocNumber
                        {
                            menuId = existingDoc.menuId,
                            docTypeId = existingDoc.docTypeId
                        });

                        if (!string.IsNullOrEmpty(docNo))
                        {
                            await _unitOfWork.DocTrans.UpdateDocNo(req.docId, docNo);
                        }
                    }
                    else
                    {
                        docNo = existingDoc?.docNo;
                    }
                }

                await _unitOfWork.CompleteAsync();
                return return200(true, new { id = req.docId, docNo }, $"Update status to {req.statusCode} Success", "");
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

