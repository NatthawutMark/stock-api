using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using stock_api.Interfaces;
using static stock_api.Request.TransactionRequest;
using static stock_api.Response.TransactionResponse;

namespace stock_api.Repositories.Dapper;

public class DocReceiveRepository : IDocReceiveRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DocReceiveRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    private const string BaseSelectSql = @"
        SELECT 
            dt.id,
            dt.menu_id AS menuId,
            CONCAT(sm.name_th, '(', sm.name_en, ')') AS menuName,
            dt.status_code AS statusCode,
            st.name_th AS statusNameTh,
            st.name_en AS statusNameEn,
            dt.doc_type_id AS docTypeId,
            mdt.name AS docTypeName,
            dt.doc_no AS docNo,
            dt.doc_date AS docDate,
            dt.ref_doc_no AS refDocNo,
            dt.emp_id AS empId,
            CONCAT(emp.f_name, ' ', emp.l_name) AS empName,
            dt.vend_id AS vendId,
            vend.name_th AS vendName,
            dt.cust_id AS custId,
            cust.name_th AS custName,
            dt.warehouse_id AS warehouseId,
            wh.name_th AS warehouseName,
            dt.booking_id AS bookingId,
            dt.doc_issue_return_id AS docIssueReturnId,
            dt.transfer_from_warehouse_id AS transferFromWarehouseId,
            wh_from.name_th AS transferFromWarehouseName,
            dt.transfer_to_warehouse_id AS transferToWarehouseId,
            wh_to.name_th AS transferToWarehouseName,
            dt.remark,
            dt.is_quality_control AS isQualityControl,
            dt.is_active AS isActive,
            dt.is_delete AS isDelete,
            dt.create_date AS createDate,
            dt.create_by AS createBy,
            dt.update_date AS updateDate,
            dt.update_by AS updateBy
        FROM doc_trans dt
        LEFT JOIN sys_menu sm ON dt.menu_id = sm.id
        LEFT JOIN mast_status st ON dt.status_code = st.code
        LEFT JOIN mast_doc_type mdt ON dt.doc_type_id = mdt.id
        LEFT JOIN mast_employee emp ON dt.emp_id = emp.id
        LEFT JOIN mast_vendor vend ON dt.vend_id = vend.id
        LEFT JOIN mast_customer cust ON dt.cust_id = cust.id
        LEFT JOIN mast_warehouse wh ON dt.warehouse_id = wh.id
        LEFT JOIN mast_warehouse wh_from ON dt.transfer_from_warehouse_id = wh_from.id
        LEFT JOIN mast_warehouse wh_to ON dt.transfer_to_warehouse_id = wh_to.id";

    public async Task<List<DocTransResponse>> List(DocTransListRequest req)
    {
        var sql = BaseSelectSql + " WHERE dt.is_delete = @isDelete";

        if (req != null && req.isActive.HasValue)
        {
            sql += " AND dt.is_active = @isActive";
        }
        if (req != null && !string.IsNullOrEmpty(req.menuId))
        {
            sql += " AND dt.menu_id = @menuId";
        }
        if (req != null && !string.IsNullOrEmpty(req.docTypeId))
        {
            sql += " AND dt.doc_type_id = @docTypeId";
        }
        if (req != null && !string.IsNullOrEmpty(req.statusCode))
        {
            sql += " AND dt.status_code = @statusCode";
        }
        if (req != null && !string.IsNullOrEmpty(req.docNo))
        {
            sql += " AND (dt.doc_no ILIKE @docNoPattern OR dt.ref_doc_no ILIKE @docNoPattern)";
        }
        if (req != null && !string.IsNullOrEmpty(req.empId))
        {
            sql += " AND dt.emp_id = @empId";
        }
        if (req != null && !string.IsNullOrEmpty(req.vendId))
        {
            sql += " AND dt.vend_id = @vendId";
        }
        if (req != null && !string.IsNullOrEmpty(req.custId))
        {
            sql += " AND dt.cust_id = @custId";
        }
        if (req != null && !string.IsNullOrEmpty(req.warehouseId))
        {
            sql += " AND dt.warehouse_id = @warehouseId";
        }
        if (req != null && req.startDate.HasValue)
        {
            sql += " AND dt.doc_date >= @startDate";
        }
        if (req != null && req.endDate.HasValue)
        {
            sql += " AND dt.doc_date <= @endDate";
        }

        sql += " ORDER BY dt.doc_date DESC, dt.create_date DESC";

        var param = new
        {
            isDelete = req?.isDelete ?? false,
            isActive = req?.isActive,
            menuId = req?.menuId,
            docTypeId = req?.docTypeId,
            statusCode = req?.statusCode,
            docNoPattern = !string.IsNullOrEmpty(req?.docNo) ? $"%{req.docNo.Trim()}%" : null,
            empId = req?.empId,
            vendId = req?.vendId,
            custId = req?.custId,
            warehouseId = req?.warehouseId,
            startDate = req?.startDate,
            endDate = req?.endDate
        };

        var result = await _connection.QueryAsync<DocTransResponse>(sql, param, transaction: _transaction);
        return result.ToList();
    }

    public async Task<DocTransResponse?> GetById(string id)
    {
        var sql = BaseSelectSql + " WHERE dt.id = @id AND dt.is_delete = false";
        var doc = await _connection.QueryFirstOrDefaultAsync<DocTransResponse>(sql, new { id }, transaction: _transaction);
        if (doc != null)
        {
            doc.logs = await GetLogs(id);
        }
        return doc;
    }

    public async Task<DocTransResponse?> GetByDocNo(string docNo)
    {
        var sql = BaseSelectSql + " WHERE dt.doc_no = @docNo AND dt.is_delete = false";
        var doc = await _connection.QueryFirstOrDefaultAsync<DocTransResponse>(sql, new { docNo = docNo.Trim() }, transaction: _transaction);
        if (doc != null)
        {
            doc.logs = await GetLogs(doc.id);
        }
        return doc;
    }

    public async Task<ActionResult> Create(DocTransCreateRequest req)
    {
        try
        {
            const string insertDocSql = @"
                INSERT INTO doc_trans (
                    id, menu_id, status_code, doc_type_id, doc_no, doc_date, ref_doc_no,
                    emp_id, vend_id, cust_id, warehouse_id, booking_id, doc_issue_return_id,
                    transfer_from_warehouse_id, transfer_to_warehouse_id, remark,
                    is_quality_control, is_active, is_delete, create_date, create_by, update_date, update_by
                ) VALUES (
                    @id, @menuId, @statusCode, @docTypeId, @docNo, @docDate, @refDocNo,
                    @empId, @vendId, @custId, @warehouseId, @bookingId, @docIssueReturnId,
                    @transferFromWarehouseId, @transferToWarehouseId, @remark,
                    @isQualityControl, true, false, CURRENT_TIMESTAMP, @createBy, CURRENT_TIMESTAMP, @updateBy
                )";

            var rows = await _connection.ExecuteAsync(insertDocSql, new
            {
                req.id,
                req.menuId,
                statusCode = string.IsNullOrEmpty(req.statusCode) ? "DRAFT" : req.statusCode.ToUpper(),
                req.docTypeId,
                req.docNo,
                docDate = req.docDate ?? DateTime.Now,
                req.refDocNo,
                req.empId,
                req.vendId,
                req.custId,
                req.warehouseId,
                req.bookingId,
                req.docIssueReturnId,
                req.transferFromWarehouseId,
                req.transferToWarehouseId,
                req.remark,
                req.isQualityControl,
                req.createBy,
                updateBy = req.createBy
            }, transaction: _transaction);

            if (rows > 0)
            {
                // บันทึก Log สร้างเอกสาร
                const string insertLogSql = @"
                    INSERT INTO doc_trans_log (
                        id, doc_id, from_status_id, to_status_id, from_user_id, to_user_id, remark,
                        is_active, is_delete, create_date, create_by, update_date, update_by
                    ) VALUES (
                        @logId, @docId, NULL, @toStatusId, @userId, @userId, 'สร้างเอกสาร',
                        true, false, CURRENT_TIMESTAMP, @userId, CURRENT_TIMESTAMP, @userId
                    )";

                await _connection.ExecuteAsync(insertLogSql, new
                {
                    logId = Guid.NewGuid().ToString().ToUpper().Replace("-", ""),
                    docId = req.id,
                    toStatusId = string.IsNullOrEmpty(req.statusCode) ? "DRAFT" : req.statusCode,
                    userId = req.createBy
                }, transaction: _transaction);

                return new OkObjectResult(new { success = true, results = req.id, message = "Create DocReceive Success", error = "" });
            }

            return new BadRequestObjectResult(new { success = false, results = "", message = "Create DocReceive Failed", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> Update(DocTransUpdateRequest req)
    {
        try
        {
            const string updateSql = @"
                UPDATE doc_trans SET
                    doc_type_id = COALESCE(@docTypeId, doc_type_id),
                    doc_date = COALESCE(@docDate, doc_date),
                    ref_doc_no = @refDocNo,
                    emp_id = @empId,
                    vend_id = @vendId,
                    cust_id = @custId,
                    warehouse_id = @warehouseId,
                    booking_id = @bookingId,
                    doc_issue_return_id = @docIssueReturnId,
                    transfer_from_warehouse_id = @transferFromWarehouseId,
                    transfer_to_warehouse_id = @transferToWarehouseId,
                    remark = @remark,
                    is_quality_control = COALESCE(@isQualityControl, is_quality_control),
                    is_active = COALESCE(@isActive, is_active),
                    update_date = CURRENT_TIMESTAMP,
                    update_by = @updateBy
                WHERE id = @id AND is_delete = false";

            var rows = await _connection.ExecuteAsync(updateSql, req, transaction: _transaction);
            return rows > 0
                ? new OkObjectResult(new { success = true, results = req.id, message = "Update DocReceive Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Update DocReceive Failed", error = "Document not found or deleted" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> UpdateStatus(DocTransUpdateStatusRequest req, string userId)
    {
        try
        {
            // ดึงสถานะปัจจุบัน
            var currentStatus = await _connection.QueryFirstOrDefaultAsync<string>(
                "SELECT status_code FROM doc_trans WHERE id = @docId AND is_delete = false",
                new { req.docId }, transaction: _transaction);

            if (currentStatus == null)
            {
                return new BadRequestObjectResult(new { success = false, results = "", message = "Document not found", error = "" });
            }

            const string updateSql = @"
                UPDATE doc_trans 
                SET status_code = @statusCode, 
                    update_date = CURRENT_TIMESTAMP, 
                    update_by = @userId 
                WHERE id = @docId AND is_delete = false";

            var rows = await _connection.ExecuteAsync(updateSql, new { req.statusCode, userId, req.docId }, transaction: _transaction);

            if (rows > 0)
            {
                // บันทึก Log เปลี่ยนสถานะ
                const string insertLogSql = @"
                    INSERT INTO doc_trans_log (
                        id, doc_id, from_status_id, to_status_id, from_user_id, to_user_id, remark,
                        is_active, is_delete, create_date, create_by, update_date, update_by
                    ) VALUES (
                        @logId, @docId, @fromStatusId, @toStatusId, @userId, @userId, @remark,
                        true, false, CURRENT_TIMESTAMP, @userId, CURRENT_TIMESTAMP, @userId
                    )";

                await _connection.ExecuteAsync(insertLogSql, new
                {
                    logId = Guid.NewGuid().ToString().ToUpper().Replace("-", ""),
                    docId = req.docId,
                    fromStatusId = currentStatus,
                    toStatusId = req.statusCode,
                    userId = userId,
                    remark = req.remark ?? $"เปลี่ยนสถานะเป็น {req.statusCode}"
                }, transaction: _transaction);

                return new OkObjectResult(new { success = true, results = req.docId, message = $"Update status to {req.statusCode} Success", error = "" });
            }

            return new BadRequestObjectResult(new { success = false, results = "", message = "Update status Failed", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> Delete(string id, string userId)
    {
        try
        {
            const string sql = @"
                UPDATE doc_trans 
                SET is_delete = true, 
                    update_date = CURRENT_TIMESTAMP, 
                    update_by = @userId 
                WHERE id = @id AND is_delete = false";

            var rows = await _connection.ExecuteAsync(sql, new { id, userId }, transaction: _transaction);
            return rows > 0
                ? new OkObjectResult(new { success = true, results = id, message = "Delete DocReceive Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Delete DocReceive Failed", error = "Document not found" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<List<DocTransLogResponse>> GetLogs(string docId)
    {
        const string sql = @"
            SELECT 
                dtl.id,
                dtl.doc_id AS docId,
                dtl.from_status_id AS fromStatusId,
                st_from.name_th AS fromStatusName,
                dtl.to_status_id AS toStatusId,
                st_to.name_th AS toStatusName,
                dtl.from_user_id AS fromUserId,
                dtl.to_user_id AS toUserId,
                dtl.remark,
                dtl.create_date AS createDate,
                dtl.create_by AS createBy
            FROM doc_trans_log dtl
            LEFT JOIN mast_status st_from ON dtl.from_status_id = st_from.code
            LEFT JOIN mast_status st_to ON dtl.to_status_id = st_to.code
            WHERE dtl.doc_id = @docId AND dtl.is_delete = false
            ORDER BY dtl.create_date ASC";

        var logs = await _connection.QueryAsync<DocTransLogResponse>(sql, new { docId }, transaction: _transaction);
        return logs.ToList();
    }
}
