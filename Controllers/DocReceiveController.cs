using Microsoft.AspNetCore.Mvc;
using System.Data;
using stock_api.Repositories;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static stock_api.Request.TransactionRequest;
using static stock_api.Response.TransactionResponse;
using System.Dynamic;

namespace stock_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DocReceiveController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;


    public DocReceiveController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListDocReceive")]
    public async Task<ActionResult<List<DocReceiveDetailResponse>>> List([FromBody] DocReceiveDetailRequest req)
    {
        try
        {
            req ??= new DocReceiveDetailRequest();
            req.isDelete = false;

            List<DocReceiveDetailResponse> res = await _unitOfWork.DocReceives.List(req);
            return return200(true, res, "Get ItemReceive list success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpGet("{id}", Name = "getDocReceive")]
    public async Task<ActionResult<List<DocReceiveDetailResponse>>> getReceive(string id)
    {
        try
        {
            dynamic data = new ExpandoObject();
            if (string.IsNullOrEmpty(id))
                return return200(false, null, "Can't Load Data", "");

            var resHeader = await _unitOfWork.DocTrans.GetById(id);
            var resDetail = await _unitOfWork.DocReceives.getListById(id);

            if (resHeader != null && resDetail != null)
            {
                List<DocReceiveDetailResponse> details = new List<DocReceiveDetailResponse>();
                foreach (var item in resDetail)
                {
                    details.Add(new DocReceiveDetailResponse
                    {
                        id = item.id,
                        docId = item.docId,
                        itemId = item.itemId,
                        itemCode = item.itemCode,
                        itemName = item.itemName,
                        lotNo = item.lotNo,
                        serialNo = item.serialNo,
                        locationId = item.locationId,
                        locationName = item.locationName,
                        isLotNo = item.isLotNo,
                        isSerialNo = item.isSerialNo,
                        uomId = item.uomId,
                        uomName = item.uomName,
                        mfgDate = item.mfgDate,
                        expireDate = item.expireDate,
                        itemQty = item.itemQty,
                        putawayQty = item.putawayQty,
                        itemPrice = item.itemPrice,
                        trackingCode = item.trackingCode
                    });
                }

                data.id = resHeader.id;
                data.docNo = resHeader.docNo;
                data.docDate = resHeader.docDate;
                data.docTypeId = resHeader.docTypeId;
                data.statusCode = resHeader.statusCode;
                data.refDocNo = resHeader.refDocNo;
                data.empId = resHeader.empId;
                data.empName = resHeader.empName;
                data.vendId = resHeader.vendId;
                data.vendName = resHeader.vendName;
                data.custId = resHeader.custId;
                data.custName = resHeader.custName;
                data.warehouseId = resHeader.warehouseId;
                data.warehouseName = resHeader.warehouseName;
                data.bookingId = resHeader.bookingId;
                data.docIssueReturnId = resHeader.docIssueReturnId;
                data.transferFromWarehouseId = resHeader.transferFromWarehouseId;
                data.transferFromWarehouseName = resHeader.transferFromWarehouseName;
                data.transferToWarehouseId = resHeader.transferToWarehouseId;
                data.transferToWarehouseName = resHeader.transferToWarehouseName;
                data.remark = resHeader.remark;
                data.isQualityControl = resHeader.isQualityControl;
                data.isActive = resHeader.isActive;
                data.isDelete = resHeader.isDelete;
                data.details = details;
            }
            else
            {
                return return200(false, null, "โหลดเอกสารไม่สมบูรณ์", "");
            }


            return return200(true, data, "Get ItemReceive list success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPut("{id}", Name = "UpdateDocReceiveDetail")]
    public async Task<ActionResult> Update(string id, [FromBody] DocReceiveRequest req)
    {
        try
        {
            if (string.IsNullOrEmpty(id) || req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data. Document ID is required." });
            }

            string userId = _systemService.GetUserId() ?? "system";

            // ดึงเอกสารเดิมมาตรวจสอบว่ามี docNo หรือไม่
            var existingDoc = await _unitOfWork.DocTrans.GetById(id);
            if (existingDoc == null)
            {
                return return200(false, null, "ไม่พบข้อมูลเอกสาร", "Document not found");
            }

            // ถ้ามี docNo เดิมอยู่แล้ว ให้ใช้ docNo เดิมเสมอ (ไม่สร้างซ้ำ)
            if (!string.IsNullOrWhiteSpace(existingDoc.docNo))
            {
                req.docNo = existingDoc.docNo;
            }
            if (string.IsNullOrWhiteSpace(req.menuId))
            {
                req.menuId = existingDoc.menuId;
            }
            if (string.IsNullOrWhiteSpace(req.docTypeId))
            {
                req.docTypeId = existingDoc.docTypeId;
            }

            var docTrans = new DocTransUpdateRequest
            {
                id = id,
                docDate = req.docDate,
                docNo = req.docNo,
                empId = req.empId,
                vendId = req.vendId,
                refDocNo = req.refDocNo,
                docTypeId = req.docTypeId,
                statusCode = req.statusCode?.ToUpper() ?? existingDoc.statusCode,
                remark = req.remark,
                updateBy = userId,
                warehouseId = req.warehouseId,
            };

            List<DocReceiveDetailRequest> details = new List<DocReceiveDetailRequest>();
            if (req.details != null)
            {
                foreach (var item in req.details)
                {
                    details.Add(new DocReceiveDetailRequest
                    {
                        id = string.IsNullOrEmpty(item.id) ? _systemService.GenGUID() : item.id,
                        docId = id,
                        itemId = item.itemId,
                        itemName = item.itemName,
                        lotNo = !string.IsNullOrEmpty(item.lotNo) ? item.lotNo.Trim() : null,
                        serialNo = !string.IsNullOrEmpty(item.serialNo) ? item.serialNo.Trim() : null,
                        locationId = item.locationId,
                        uomId = item.uomId,
                        mfgDate = item.mfgDate,
                        expireDate = item.expireDate,
                        itemQty = item.itemQty,
                        itemPrice = item.itemPrice
                    });
                }
            }

            var resHeader = await _unitOfWork.DocTrans.Update(docTrans);
            var resDetail = await _unitOfWork.DocReceives.Update(id, details);

            if (resHeader is OkObjectResult && resDetail is OkObjectResult)
            {
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
                        await _unitOfWork.DocTrans.UpdateDocNo(id, req.docNo);
                    }
                }

                await _unitOfWork.CompleteAsync();
                return return200(true, new { id, docNo = req.docNo }, "Update DocTrans Success", "");
            }
            else if (resHeader is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update DocTrans Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else if (resDetail is BadRequestObjectResult badDetailResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update DocReceives Detail Failed", badDetailResult.Value?.ToString() ?? "");
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

    [HttpPost("{id}/confirm", Name = "ConfirmDocReceive")]
    public async Task<ActionResult> ConfirmDocReceive(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return return200(false, null, "Invalid request", "menuId and docTypeId are required");
            }

            var updateStatus = await _unitOfWork.DocReceives.Confirm(id);
            if (updateStatus is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, new { id }, "ยืนยันเอกสาารเสร็จสิ้น", "");
            }
            else
            {
                return return200(false, null, "มีปัญหาการบันทึกเอกสาร", "");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = false, message = "An error occurred while generating DocNumber", error = ex.Message });
        }
    }
    [HttpPost("{id}/putaway", Name = "PutawayDocReceive")]
    public async Task<ActionResult> PutawayDocReceive(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return return200(false, null, "Invalid request", "DocId are required");
            }

            string userId = _systemService.GetUserId() ?? "system";
            var updateStatus = await _unitOfWork.DocReceives.Putaway(id, userId);
            if (updateStatus is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, new { id }, "Putaway เอกสารเสร็จสิ้น", "");
            }
            else if (updateStatus is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                var msg = badRequestResult.Value != null ? badRequestResult.Value.ToString() : "มีปัญหาการบันทึกเอกสาร";
                return return200(false, null, "Putaway DocReceive Failed", msg ?? "มีปัญหาการบันทึกเอกสาร");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "มีปัญหาการบันทึกเอกสาร", "");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while Putaway", error = ex.Message });
        }
    }

    // [HttpPost("create", Name = "CreateDocTrans")]
    // public async Task<ActionResult> Create([FromBody] DocTransCreateRequest req)
    // {
    //     try
    //     {
    //         if (req == null)
    //         {
    //             return BadRequest(new { status = false, message = "Invalid request data" });
    //         }

    //         if (string.IsNullOrEmpty(req.menuId))
    //         {
    //             return return200(false, null, "Validation Error", "menuId is required");
    //         }
    //         string userId = _systemService.GetUserId() ?? "system";
    //         req.id = _systemService.GenGUID();
    //         req.createBy = userId;
    //         req.updateBy = userId;

    //         if (string.IsNullOrWhiteSpace(req.statusCode))
    //         {
    //             req.statusCode = "DRAFT";
    //         }

    //         if (req.statusCode.ToUpper() == "SAVE")
    //         {
    //             req.docNo = await _systemService.GetDocNumber(new DocNumber
    //             {
    //                 menuId = req.menuId,
    //                 docTypeId = req.docTypeId
    //             });
    //         }

    //         var result = await _unitOfWork.DocTrans.Create(req);
    //         if (result is OkObjectResult)
    //         {
    //             if (req.menuId == "00E03A994280450ABF7E2EE62A7AD4D6") // Receive
    //             {
    //                 List<DocReceiveDetailRequest> list = new List<DocReceiveDetailRequest>();
    //                 foreach (var item in req.details)
    //                 {
    //                     list.Add(new DocReceiveDetailRequest
    //                     {
    //                         id = _systemService.GenGUID(),
    //                         docId = req.id,
    //                         itemId = item.itemId,
    //                         itemName = item.itemName,
    //                         lotNo = item.lotNo ?? null,
    //                         serialNo = item.serialNo ?? null,
    //                         locationId = item.locationId,
    //                         uomId = item.uomId,
    //                         mfgDate = item.mfgDate ?? null,
    //                         expireDate = item.expireDate ?? null,
    //                         itemQty = item.itemQty ?? 0,
    //                         itemPrice = item.itemPrice ?? 0,
    //                         putawayQty = 0,
    //                         createBy = userId,
    //                         updateBy = userId,
    //                     });
    //                 }
    //                 var resDetail = await _unitOfWork.DocReceives.CreateList(list);
    //                 if (resDetail is not OkObjectResult)
    //                 {
    //                     throw new Exception($"Error Insert Detail ");
    //                 }
    //             }
    //             else if (req.menuId == "85D356B7877647C49B61B71FB047E0B6") // Issue
    //             {

    //             }
    //             else if (req.menuId == "322D946D544B4A179EB9E09613CAC5DC") // Return
    //             {

    //             }
    //             else if (req.menuId == "680AB9E0D80D4806AD119476FA511EB4") // Stock Disposal
    //             {

    //             }
    //             else if (req.menuId == "92AEEB580AD14B848F4AB6CD294A17C5") // Trans-in
    //             {

    //             }
    //             else if (req.menuId == "268A2969A63F4D448AEFB1C8CE64C527") // Trans-out
    //             {

    //             }
    //             else
    //             {
    //                 return return200(false, null, "มีปัญหาการเพิ่มการเพิ่มรายการสินค้า กรุณาติดต่อทีมงาน", "");
    //             }

    //             await _unitOfWork.CompleteAsync();
    //             return return200(true, new { id = req.id, docNo = req.docNo }, "Create DocTrans Success", "");
    //         }
    //         else if (result is BadRequestObjectResult badRequestResult)
    //         {
    //             _unitOfWork.Dispose();
    //             return return200(false, null, "Create DocTrans Failed", badRequestResult.Value?.ToString() ?? "");
    //         }
    //         else
    //         {
    //             _unitOfWork.Dispose();
    //             return return200(false, null, "Create DocTrans Failed", "Unknown error occurred");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         _unitOfWork.Dispose();
    //         return StatusCode(500, new { status = false, message = "An error occurred while creating DocTrans", error = ex.Message });
    //     }
    // }

    // [HttpGet("getById", Name = "GetDocTransById")]
    // public async Task<ActionResult> GetById([FromQuery] string id)
    // {
    //     try
    //     {
    //         if (string.IsNullOrEmpty(id))
    //         {
    //             return return200(false, null, "Invalid request", "Document ID is required");
    //         }

    //         DocTransResponse? res = await _unitOfWork.DocTrans.GetById(id);
    //         if (res != null)
    //         {
    //             return return200(true, res, "Get DocTrans success", "");
    //         }
    //         else
    //         {
    //             return return200(false, null, "DocTrans not found", "");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
    //     }
    // }

    // [HttpGet("getByDocNo", Name = "GetDocTransByDocNo")]
    // public async Task<ActionResult> GetByDocNo([FromQuery] string docNo)
    // {
    //     try
    //     {
    //         if (string.IsNullOrEmpty(docNo))
    //         {
    //             return return200(false, null, "Invalid request", "Document number is required");
    //         }

    //         DocTransResponse? res = await _unitOfWork.DocTrans.GetByDocNo(docNo);
    //         if (res != null)
    //         {
    //             return return200(true, res, "Get DocTrans success", "");
    //         }
    //         else
    //         {
    //             return return200(false, null, "DocTrans not found", "");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
    //     }
    // }

    // [HttpGet("getLogs", Name = "GetDocTransLogs")]
    // public async Task<ActionResult> GetLogs([FromQuery] string docId)
    // {
    //     try
    //     {
    //         if (string.IsNullOrEmpty(docId))
    //         {
    //             return return200(false, null, "Invalid request", "DocId is required");
    //         }

    //         List<DocTransLogResponse> logs = await _unitOfWork.DocTrans.GetLogs(docId);
    //         return return200(true, logs, "Get DocTrans logs success", "");
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
    //     }
    // }

    // [HttpPost("updateStatus", Name = "UpdateDocTransStatus")]
    // public async Task<ActionResult> UpdateStatus([FromBody] DocTransUpdateStatusRequest req)
    // {
    //     try
    //     {
    //         if (req == null || string.IsNullOrEmpty(req.docId) || string.IsNullOrEmpty(req.statusCode))
    //         {
    //             return BadRequest(new { status = false, message = "docId and statusCode are required." });
    //         }

    //         string userId = _systemService.GetUserId() ?? "system";

    //         var result = await _unitOfWork.DocTrans.UpdateStatus(req, userId);
    //         if (result is OkObjectResult)
    //         {
    //             await _unitOfWork.CompleteAsync();
    //             return return200(true, req.docId, $"Update status to {req.statusCode} Success", "");
    //         }
    //         else if (result is BadRequestObjectResult badRequestResult)
    //         {
    //             _unitOfWork.Dispose();
    //             return return200(false, null, "Update Status Failed", badRequestResult.Value?.ToString() ?? "");
    //         }
    //         else
    //         {
    //             _unitOfWork.Dispose();
    //             return return200(false, null, "Update Status Failed", "Unknown error occurred");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         _unitOfWork.Dispose();
    //         return StatusCode(500, new { status = false, message = "An error occurred while updating status", error = ex.Message });
    //     }
    // }

    // [HttpPost("delete", Name = "DeleteDocTrans")]
    // public async Task<ActionResult> Delete([FromBody] DocTransDeleteRequest req)
    // {
    //     try
    //     {
    //         if (req == null || string.IsNullOrEmpty(req.id))
    //         {
    //             return BadRequest(new { status = false, message = "Document ID is required." });
    //         }

    //         string userId = _systemService.GetUserId() ?? "system";

    //         var result = await _unitOfWork.DocTrans.Delete(req.id, userId);
    //         if (result is OkObjectResult)
    //         {
    //             await _unitOfWork.CompleteAsync();
    //             return return200(true, req.id, "Delete DocTrans Success", "");
    //         }
    //         else if (result is BadRequestObjectResult badRequestResult)
    //         {
    //             _unitOfWork.Dispose();
    //             return return200(false, null, "Delete DocTrans Failed", badRequestResult.Value?.ToString() ?? "");
    //         }
    //         else
    //         {
    //             _unitOfWork.Dispose();
    //             return return200(false, null, "Delete DocTrans Failed", "Unknown error occurred");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         _unitOfWork.Dispose();
    //         return StatusCode(500, new { status = false, message = "An error occurred while deleting DocTrans", error = ex.Message });
    //     }
    // }

    // [HttpPost("generateDocNo", Name = "GenerateDocNo")]
    // public async Task<ActionResult> GenerateDocNo([FromBody] DocNumber req)
    // {
    //     try
    //     {
    //         if (req == null || string.IsNullOrEmpty(req.menuId) || string.IsNullOrEmpty(req.docTypeId))
    //         {
    //             return return200(false, null, "Invalid request", "menuId and docTypeId are required");
    //         }

    //         string docNo = await _systemService.GetDocNumber(req);
    //         return return200(true, new { docNo }, "Generate DocNumber Success", "");
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, new { status = false, message = "An error occurred while generating DocNumber", error = ex.Message });
    //     }
    // }

    #region Private Methods
    [NonAction]
    public ObjectResult return200(bool success = true, object? results = null, string message = "", string error = "")
    {
        return StatusCode(200, new { success = success, results = results, message = message, error = error });
    }
    #endregion
}

