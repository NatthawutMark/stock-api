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

namespace stock_api.Repositories;

public class DocIssueRepository : IDocIssueRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DocIssueRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    private const string baseSql = @"
        SELECT 
            iss.id,
            iss.doc_id AS docId,
            iss.inv_id AS invId,
            iss.item_qty AS itemQty,
            iss.picking_qty AS pickingQty,
            iss.return_qty AS returnQty,
            iss.tracking_code AS trackingCode,
            iss.is_active AS isActive,
            iss.is_delete AS isDelete,
            inv.item_id AS itemId,
            item.item_code AS itemCode,
            item.item_name AS itemName,
            inv.lot_no AS lotNo,
            inv.serial_no AS serialNo,
            inv.location_id AS locationId,
            loc.name AS locationName,
            inv.uom_id AS uomId,
            uom.name AS uomName,
            inv.item_price AS itemPrice,
            inv.item_qty AS availableQty,
            inv.mfg_date AS mfgDate,
            inv.expire_date AS expireDate,
            inv.warehouse_id AS warehouseId,
            wh.warehouse_name AS warehouseName
        FROM doc_issue_detail iss
        LEFT JOIN inventory inv ON iss.inv_id = inv.id
        LEFT JOIN mast_item item ON inv.item_id = item.id
        LEFT JOIN mast_uom uom ON inv.uom_id = uom.id 
        LEFT JOIN mast_location loc ON inv.location_id = loc.id
        LEFT JOIN mast_warehouse wh ON inv.warehouse_id = wh.id ";

    public async Task<List<DocIssueDetailResponse>> List(DocIssueDetailRequest req)
    {
        try
        {
            string sql = baseSql + " WHERE (iss.is_delete IS NULL OR iss.is_delete = @isDelete)";
            if (!string.IsNullOrEmpty(req.docId))
                sql += " AND iss.doc_id = @docId";

            var param = new
            {
                docId = req.docId,
                isDelete = req.isDelete ?? false
            };
            var res = await _connection.QueryAsync<DocIssueDetailResponse>(sql, param, transaction: _transaction);
            return res.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error : {ex.Message}, {ex.InnerException?.Message}");
        }
    }

    public async Task<List<DocIssueDetailResponse>> getListById(string _id)
    {
        try
        {
            string sql = baseSql + " WHERE iss.doc_id = @id AND (iss.is_delete IS NULL OR iss.is_delete = false)";
            var res = await _connection.QueryAsync<DocIssueDetailResponse>(sql, new { id = _id }, transaction: _transaction);
            return res.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error : {ex.Message}, {ex.InnerException?.Message}");
        }
    }

    public async Task<ActionResult> CreateList(List<DocIssueDetailRequest> req)
    {
        try
        {
            const string insertSql = @"
                INSERT INTO doc_issue_detail (
                    id, doc_id, inv_id, item_qty, picking_qty, return_qty,
                    tracking_code, is_active, is_delete, create_date, create_by, update_date, update_by
                ) 
                VALUES (
                    @id, @docId, @invId, @itemQty, COALESCE(@pickingQty, 0), COALESCE(@returnQty, 0),
                    @trackingCode, true, false, CURRENT_TIMESTAMP, @createBy, CURRENT_TIMESTAMP, @updateBy
                )";

            var rows = await _connection.ExecuteAsync(insertSql, req, transaction: _transaction);

            return rows > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create DocIssue Detail Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create DocIssue Detail Failed", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> Update(string id, List<DocIssueDetailRequest> req)
    {
        try
        {
            string delItems = @"DELETE FROM doc_issue_detail WHERE doc_id = @id";
            await _connection.ExecuteAsync(delItems, new { id }, transaction: _transaction);

            if (req != null && req.Any())
            {
                const string insertSql = @"
                    INSERT INTO doc_issue_detail (
                        id, doc_id, inv_id, item_qty, picking_qty, return_qty,
                        tracking_code, is_active, is_delete, create_date, create_by, update_date, update_by
                    ) 
                    VALUES (
                        @id, @docId, @invId, @itemQty, COALESCE(@pickingQty, 0), COALESCE(@returnQty, 0),
                        @trackingCode, true, false, CURRENT_TIMESTAMP, @createBy, CURRENT_TIMESTAMP, @updateBy
                    )";

                var rows = await _connection.ExecuteAsync(insertSql, req, transaction: _transaction);
                return rows > 0
                    ? new OkObjectResult(new { success = true, results = id, message = "Update DocIssue Detail Success", error = "" })
                    : new BadRequestObjectResult(new { success = false, results = "", message = "Update DocIssue Detail Failed", error = "No rows inserted" });
            }

            return new OkObjectResult(new { success = true, results = id, message = "Update DocIssue Detail Success", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> Confirm(string id)
    {
        try
        {
            string updateDetail = @"UPDATE doc_issue_detail SET picking_qty = 0 WHERE doc_id = @id";
            await _connection.ExecuteAsync(updateDetail, new { id }, transaction: _transaction);

            string updateSql = @"UPDATE doc_trans SET status_code = 'CONFIRM', update_date = CURRENT_TIMESTAMP WHERE id = @id";
            var rows = await _connection.ExecuteAsync(updateSql, new { id }, transaction: _transaction);

            return rows > 0
                ? new OkObjectResult(new { success = true, results = id, message = "Confirm DocIssue Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Confirm DocIssue Failed", error = "Document not found or deleted" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> Picking(string _id, string? userId = null)
    {
        try
        {
            string user = !string.IsNullOrEmpty(userId) ? userId : "system";

            // 1. ตรวจสอบเอกสาร doc_trans
            var docTrans = await _connection.QueryFirstOrDefaultAsync<DocTransResponse>(
                "SELECT id, warehouse_id AS warehouseId, status_code AS statusCode FROM doc_trans WHERE id = @id AND (is_delete IS NULL OR is_delete = false)",
                new { id = _id },
                transaction: _transaction);

            if (docTrans == null)
            {
                return new BadRequestObjectResult(new { success = false, results = "", message = "Document not found", error = "Document not found or deleted" });
            }

            // 2. ตรวจสอบรายการที่ต้องทำการ Picking (item_qty > picking_qty)
            string sqlCheckItems = @"
                SELECT COUNT(1) 
                FROM doc_issue_detail iss 
                WHERE iss.doc_id = @id 
                  AND (iss.is_delete IS NULL OR iss.is_delete = false) 
                  AND (iss.item_qty - COALESCE(iss.picking_qty, 0)) > 0";

            var count = await _connection.ExecuteScalarAsync<int>(sqlCheckItems, new { id = _id }, transaction: _transaction);
            if (count == 0)
            {
                return new BadRequestObjectResult(new { success = false, results = "", message = "Can't Find Item", error = "No items left to pick" });
            }

            // 3. ปรับปรุงตัดสต็อกออกจากตาราง inventory
            const string sqlPicking = @"
                MERGE INTO inventory AS target
                USING (
                    SELECT 
                        iss.inv_id,
                        SUM(iss.item_qty - COALESCE(iss.picking_qty, 0)) AS qty_to_pick
                    FROM doc_issue_detail iss
                    WHERE iss.doc_id = @id 
                      AND (iss.is_delete IS NULL OR iss.is_delete = false)
                      AND (iss.item_qty - COALESCE(iss.picking_qty, 0)) > 0
                    GROUP BY iss.inv_id
                ) AS source
                ON target.id = source.inv_id
                WHEN MATCHED THEN
                    UPDATE SET 
                        item_qty = GREATEST(0, COALESCE(target.item_qty, 0) - source.qty_to_pick),
                        update_date = CURRENT_TIMESTAMP,
                        update_by = @userId;";

            await _connection.ExecuteAsync(sqlPicking, new { id = _id, userId = user }, transaction: _transaction);

            // 4. ปรับปรุง picking_qty ใน doc_issue_detail ให้เท่ากับ item_qty
            const string updateDetailSql = @"
                UPDATE doc_issue_detail 
                SET picking_qty = item_qty,
                    update_date = CURRENT_TIMESTAMP,
                    update_by = @userId
                WHERE doc_id = @id 
                  AND (is_delete IS NULL OR is_delete = false);";

            await _connection.ExecuteAsync(updateDetailSql, new { id = _id, userId = user }, transaction: _transaction);

            // 5. ปรับปรุงสถานะของ doc_trans เป็น COMPLETED
            const string updateTransSql = @"
                UPDATE doc_trans 
                SET status_code = 'COMPLETED',
                    update_date = CURRENT_TIMESTAMP,
                    update_by = @userId
                WHERE id = @id 
                  AND (is_delete IS NULL OR is_delete = false);";

            var rows = await _connection.ExecuteAsync(updateTransSql, new { id = _id, userId = user }, transaction: _transaction);

            if (rows > 0)
            {
                // 6. บันทึก Transaction Log
                const string insertLogSql = @"
                    INSERT INTO doc_trans_log (
                        id, doc_id, from_status_id, to_status_id, from_user_id, to_user_id, remark,
                        is_active, is_delete, create_date, create_by, update_date, update_by
                    ) VALUES (
                        @logId, @docId, @fromStatusId, @toStatusId, @userId, @userId, @remark,
                        true, false, CURRENT_TIMESTAMP, @userId, CURRENT_TIMESTAMP, @userId
                    );";

                await _connection.ExecuteAsync(insertLogSql, new
                {
                    logId = Guid.NewGuid().ToString().ToUpper().Replace("-", ""),
                    docId = _id,
                    fromStatusId = docTrans.statusCode,
                    toStatusId = "COMPLETED",
                    userId = user,
                    remark = "Picking completed - stock deducted from inventory"
                }, transaction: _transaction);

                return new OkObjectResult(new { success = true, results = _id, message = "Picking DocIssue Success", error = "" });
            }

            return new BadRequestObjectResult(new { success = false, results = "", message = "Picking DocIssue Failed", error = "Document not found or update failed" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }
}

