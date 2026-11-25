using Microsoft.AspNetCore.Mvc;
using System.Data;
using stock_api.Repositories;
using stock_api.Interfaces;
using stock_api.Common;
using Microsoft.AspNetCore.Authorization;
using static stock_api.Request.TransactionRequest;
using static stock_api.Response.TransactionResponse;
using System.Dynamic;

namespace stock_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DocDisposalController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;

    public DocDisposalController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListDocDisposal")]
    public async Task<ActionResult<List<DocDisposalDetailResponse>>> List([FromBody] DocDisposalDetailRequest req)
    {
        try
        {
            req ??= new DocDisposalDetailRequest();
            req.isDelete = false;

            List<DocDisposalDetailResponse> res = await _unitOfWork.DocDisposals.List(req);
            return return200(true, res, "Get ItemDisposal list success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpGet("{id}", Name = "getDocDisposal")]
    public async Task<ActionResult> getDisposal(string id)
    {
        try
        {
            dynamic data = new ExpandoObject();
            if (string.IsNullOrEmpty(id))
                return return200(false, null, "Can't Load Data", "");

            var resHeader = await _unitOfWork.DocTrans.GetById(id);
            var resDetail = await _unitOfWork.DocDisposals.getListById(id);

            if (resHeader != null && resDetail != null)
            {
                List<DocDisposalDetailResponse> details = new List<DocDisposalDetailResponse>();
                foreach (var item in resDetail)
                {
                    details.Add(new DocDisposalDetailResponse
                    {
                        id = item.id,
                        docId = item.docId,
                        invDmgId = item.invDmgId,
                        itemId = item.itemId,
                        itemCode = item.itemCode,
                        itemName = item.itemName,
                        itemQty = item.itemQty,
                        isActive = item.isActive,
                        isDelete = item.isDelete,
                        lotNo = item.lotNo,
                        serialNo = item.serialNo,
                        uomId = item.uomId,
                        mfgDate = item.mfgDate,
                        expireDate = item.expireDate
                    });
                }

                data.id = resHeader.id;
                data.docNo = resHeader.docNo;
                data.docDate = resHeader.docDate;
                data.docTypeId = resHeader.docTypeId;
                data.statusId = resHeader.statusId;
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

            return return200(true, data, "Get ItemDisposal list success", "");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, results = (object?)null, message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPut("{id}", Name = "UpdateDocDisposalDetail")]
    public async Task<ActionResult> Update(string id, [FromBody] DocDisposalRequest req)
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

            // Resolve statusId and statusCode
            string? targetStatusId = req.statusId;
            string? targetStatusCode = req.statusCode?.ToUpper();
            if (!string.IsNullOrWhiteSpace(targetStatusId) && string.IsNullOrWhiteSpace(targetStatusCode))
            {
                var st = await _unitOfWork.Statuses.GetById(targetStatusId);
                targetStatusCode = st?.code?.ToUpper();
            }
            else if (!string.IsNullOrWhiteSpace(targetStatusCode) && string.IsNullOrWhiteSpace(targetStatusId))
            {
                var st = await _unitOfWork.Statuses.GetByCode(true, targetStatusCode);
                targetStatusId = st?.id;
            }
            targetStatusId ??= existingDoc.statusId;
            targetStatusCode ??= existingDoc.statusCode;

            var docTrans = new DocTransUpdateRequest
            {
                id = id,
                docDate = req.docDate,
                docNo = req.docNo,
                empId = req.empId,
                refDocNo = req.refDocNo,
                docTypeId = req.docTypeId,
                statusId = targetStatusId,
                statusCode = targetStatusCode,
                remark = req.remark,
                updateBy = userId,
                warehouseId = req.warehouseId,
            };

            List<DocDisposalDetailRequest> details = new List<DocDisposalDetailRequest>();
            if (req.details != null)
            {
                foreach (var item in req.details)
                {
                    details.Add(new DocDisposalDetailRequest
                    {
                        id = _systemService.GenGUID(),
                        docId = id,
                        invDmgId = item.invDmgId,
                        itemId = item.itemId,
                        lotNo = !string.IsNullOrEmpty(item.lotNo) ? item.lotNo.Trim() : null,
                        serialNo = !string.IsNullOrEmpty(item.serialNo) ? item.serialNo.Trim() : null,
                        uomId = item.uomId,
                        itemPrice = item.itemPrice,
                        mfgDate = item.mfgDate,
                        expireDate = item.expireDate,
                        itemQty = item.itemQty,
                        createBy = userId,
                        updateBy = userId
                    });
                }
            }

            var resHeader = await _unitOfWork.DocTrans.Update(docTrans);
            var resDetail = await _unitOfWork.DocDisposals.Update(id, details);

            if (resHeader is OkObjectResult && resDetail is OkObjectResult)
            {
                // เงื่อนไข: Transaction บันทึกข้อมูลสมบูรณ์แล้ว
                // ถ้าสถานะเป็น SAVE และยังไม่มี DocNo จึงจะสร้าง DocNo
                if (string.Equals(targetStatusCode, DocStatusConstant.Save, StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(req.docNo))
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
                return return200(true, new { id, docNo = req.docNo, statusId = targetStatusId, statusCode = targetStatusCode }, "Update DocTrans Success", "");
            }
            else if (resHeader is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update DocTrans Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else if (resDetail is BadRequestObjectResult badDetailResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Update DocDisposal Detail Failed", badDetailResult.Value?.ToString() ?? "");
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

    [HttpPost("{id}/confirm", Name = "ConfirmDocDisposal")]
    public async Task<ActionResult> ConfirmDocDisposal(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return return200(false, null, "Invalid request", "DocId is required");
            }

            var updateStatus = await _unitOfWork.DocDisposals.Confirm(id);
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

    #region Private Methods
    [NonAction]
    public ObjectResult return200(bool success = true, object? results = null, string message = "", string error = "")
    {
        return StatusCode(200, new { success = success, results = results, message = message, error = error });
    }
    #endregion
}

