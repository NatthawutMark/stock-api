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
public class DocIssueController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;

    public DocIssueController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListDocIssue")]
    public async Task<ActionResult<List<DocIssueDetailResponse>>> List([FromBody] DocIssueDetailRequest req)
    {
        try
        {
            req ??= new DocIssueDetailRequest();
            req.isDelete = false;

            List<DocIssueDetailResponse> res = await _unitOfWork.DocIssue.List(req);
            return return200(true, res, "Get ItemIssue list success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpGet("{id}", Name = "getDocIssue")]
    public async Task<ActionResult> getIssue(string id)
    {
        try
        {
            dynamic data = new ExpandoObject();
            if (string.IsNullOrEmpty(id))
                return return200(false, null, "Can't Load Data", "");

            var resHeader = await _unitOfWork.DocTrans.GetById(id);
            var resDetail = await _unitOfWork.DocIssue.getListById(id);

            if (resHeader != null && resDetail != null)
            {
                List<DocIssueDetailResponse> details = new List<DocIssueDetailResponse>();
                foreach (var item in resDetail)
                {
                    details.Add(new DocIssueDetailResponse
                    {
                        id = item.id,
                        docId = item.docId,
                        invId = item.invId,
                        itemQty = item.itemQty,
                        pickingQty = item.pickingQty,
                        returnQty = item.returnQty,
                        trackingCode = item.trackingCode,
                        isActive = item.isActive,
                        isDelete = item.isDelete,
                        itemId = item.itemId,
                        itemCode = item.itemCode,
                        itemName = item.itemName,
                        lotNo = item.lotNo,
                        serialNo = item.serialNo,
                        locationId = item.locationId,
                        locationName = item.locationName,
                        uomId = item.uomId,
                        uomName = item.uomName,
                        itemPrice = item.itemPrice,
                        availableQty = item.availableQty,
                        mfgDate = item.mfgDate,
                        expireDate = item.expireDate,
                        warehouseId = item.warehouseId,
                        warehouseName = item.warehouseName
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

            return return200(true, data, "Get ItemIssue list success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPut("{id}", Name = "UpdateDocIssueDetail")]
    public async Task<ActionResult> Update(string id, [FromBody] DocIssueRequest req)
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
                custId = req.custId,
                refDocNo = req.refDocNo,
                docTypeId = req.docTypeId,
                statusCode = req.statusCode?.ToUpper() ?? existingDoc.statusCode,
                remark = req.remark,
                updateBy = userId,
                warehouseId = req.warehouseId,
            };

            List<DocIssueDetailRequest> details = new List<DocIssueDetailRequest>();
            if (req.details != null)
            {
                foreach (var item in req.details)
                {
                    details.Add(new DocIssueDetailRequest
                    {
                        id = string.IsNullOrEmpty(item.id) ? _systemService.GenGUID() : item.id,
                        docId = id,
                        invId = item.invId,
                        itemQty = item.itemQty,
                        pickingQty = item.pickingQty,
                        returnQty = item.returnQty,
                        trackingCode = item.trackingCode,
                        createBy = userId,
                        updateBy = userId
                    });
                }
            }

            var resHeader = await _unitOfWork.DocTrans.Update(docTrans);
            var resDetail = await _unitOfWork.DocIssue.Update(id, details);

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
                return return200(false, null, "Update DocIssue Detail Failed", badDetailResult.Value?.ToString() ?? "");
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

    [HttpPost("{id}/confirm", Name = "ConfirmDocIssue")]
    public async Task<ActionResult> ConfirmDocIssue(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return return200(false, null, "Invalid request", "DocId is required");
            }

            var updateStatus = await _unitOfWork.DocIssue.Confirm(id);
            if (updateStatus is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, new { id }, "ยืนยันเอกสารเสร็จสิ้น", "");
            }
            else
            {
                return return200(false, null, "มีปัญหาการบันทึกเอกสาร", "");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = false, message = "An error occurred while confirming", error = ex.Message });
        }
    }

    [HttpPost("{id}/picking", Name = "PickingDocIssue")]
    public async Task<ActionResult> PickingDocIssue(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return return200(false, null, "Invalid request", "DocId is required");
            }

            string userId = _systemService.GetUserId() ?? "system";
            var updateStatus = await _unitOfWork.DocIssue.Picking(id, userId);
            if (updateStatus is OkObjectResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, new { id }, "Picking เอกสารเสร็จสิ้น", "");
            }
            else if (updateStatus is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                var msg = badRequestResult.Value != null ? badRequestResult.Value.ToString() : "มีปัญหาการบันทึกเอกสาร";
                return return200(false, null, "Picking DocIssue Failed", msg ?? "มีปัญหาการบันทึกเอกสาร");
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
            return StatusCode(500, new { status = false, message = "An error occurred while Picking", error = ex.Message });
        }
    }

    [HttpPost("{id}/issue", Name = "IssueDocIssue")]
    public async Task<ActionResult> IssueDocIssue(string id)
    {
        return await PickingDocIssue(id);
    }

    #region Private Methods
    [NonAction]
    public ObjectResult return200(bool success = true, object? results = null, string message = "", string error = "")
    {
        return StatusCode(200, new { success = success, results = results, message = message, error = error });
    }
    #endregion
}

