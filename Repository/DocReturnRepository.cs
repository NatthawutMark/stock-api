using Dapper;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using stock_api.Common;
using stock_api.Interfaces;
using static stock_api.Request.TransactionRequest;
using static stock_api.Response.TransactionResponse;
namespace stock_api.Repositories;

public class DocReturnRepository : IDocReturnRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DocReturnRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    private const string baseSql = @"select 
                                    ret.id ,
                                    ret.doc_id as docId,
                                    ret.iss_detail_id as issDetailId,
                                    ret.inv_id as invId,
                                    ret.item_id as itemId,
                                    ret.item_code as itemCode,
                                    ret.item_name as itemName,
                                    ret.lot_no as lotNo,
                                    ret.serial_no as serialNo,
                                    ret.uom_id as uomId,
                                    uom.name as uomName,
                                    ret.item_qty as itemQty,
                                    ret.item_price as itemPrice,
                                    ret.mfg_date as mfgDate,
                                    ret.expire_date as expireDate,
                                    mr.id as reasonId,
                                    mr.name as reasonName
                                    FROM 
                                    doc_return_detail ret 
                                    LEFT JOIN mast_uom uom on ret.uom_id = uom.id 
                                    LEFT join mast_reason mr on ret.reason_id = mr.id ";

    public async Task<List<DocReturnDetailResponse>> List(DocReturnDetailRequest req)
    {
        try
        {
            string sql = baseSql + "where ret.is_delete = @isDelete";
            if (!string.IsNullOrEmpty(req.docId))
                sql += " and ret.doc_id = @docId";
            var param = new
            {
                docId = req.docId,
                isDelete = req.isDelete
            };
            var res = await _connection.QueryAsync<DocReturnDetailResponse>(sql, param, transaction: _transaction);
            return res.ToList();
        }
        catch (Exception ex)
        {

            throw new Exception($"Error : {ex.Message}, {ex.InnerException?.Message}");
        }
    }
    public async Task<List<DocReturnDetailResponse>> getListById(string _id)
    {
        try
        {
            string sql = baseSql + "where ret.doc_id = @id";
            var res = await _connection.QueryAsync<DocReturnDetailResponse>(sql, new { id = _id }, transaction: _transaction);
            return res.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error : {ex.Message}, {ex.InnerException?.Message}");
        }
    }
    public async Task<ActionResult> CreateList(List<DocReturnDetailRequest> req)
    {
        try
        {
            const string insertDocReturnSql = @"
                INSERT INTO public.doc_return_detail(
                id, doc_id, iss_detail_id, inv_id, item_id, item_code, item_name, lot_no, serial_no,
                uom_id, mfg_date, expire_date, item_qty, item_price, reason_id,
                create_by, update_by)
                VALUES (
                @id, @docId, @issDetailId, @invId, @itemId, @itemCode, @itemName, @lotNo, @serialNo,
                @uomId, @mfgDate, @expireDate,@itemQty, @itemPrice, @reasonId,
                @createBy, @updateBy
                )";

            var rows = await _connection.ExecuteAsync(insertDocReturnSql, req, transaction: _transaction);

            return new OkObjectResult(new { success = false, results = "", message = "Create DocReturn Failed", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> Update(string id, List<DocReturnDetailRequest> req)
    {
        try
        {
            string delItems = @"DELETE FROM doc_return_detail where doc_id = @id";
            await _connection.QueryAsync(delItems, new { id }, _transaction);

            const string updateSql = @"
                INSERT INTO public.doc_return_detail(
                id, doc_id, iss_detail_id, inv_id, item_id,item_code, item_name, lot_no, serial_no,
                uom_id, mfg_date, expire_date, item_qty, item_price, reason_id,
                create_by, update_by)
                VALUES (
                @id, @docId, @issDetailId, @invId, @itemId, @itemCode, @itemName, @lotNo, @serialNo,
                @uomId, @mfgDate, @expireDate,@itemQty, @itemPrice, @reasonId,
                @createBy, @updateBy
                )";

            var rows = await _connection.ExecuteAsync(updateSql, req, transaction: _transaction);
            return rows > 0
                ? new OkObjectResult(new { success = true, results = id, message = "Update DocReturn Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Update DocReturn Failed", error = "Document not found or deleted" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }
    public async Task<ActionResult> Confirm(string id, string? userId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestObjectResult(new { success = false, results = "", message = "Document ID is required", error = "id is required" });
            }

            string user = !string.IsNullOrWhiteSpace(userId) ? userId : "system";

            // 1. ตรวจสอบเอกสาร doc_trans และดึง warehouse_id รวมถึงสถานะปัจจุบัน
            var docTrans = await _connection.QueryFirstOrDefaultAsync<DocTransResponse>(
                @"SELECT id, warehouse_id AS warehouseId, status_id AS statusId, status_code AS statusCode
                  FROM doc_trans
                  WHERE id = @id AND (is_delete IS NULL OR is_delete = false)",
                new { id },
                transaction: _transaction);

            if (docTrans == null)
            {
                return new BadRequestObjectResult(new { success = false, results = "", message = "Document not found", error = "Document not found or deleted" });
            }

            if (string.IsNullOrWhiteSpace(docTrans.warehouseId))
            {
                return new BadRequestObjectResult(new { success = false, results = "", message = "Warehouse not specified in document", error = "warehouse_id is required in doc_trans" });
            }

            if (string.Equals(docTrans.statusCode, DocStatusConstant.Completed, StringComparison.OrdinalIgnoreCase)
                || string.Equals(docTrans.statusCode, "COMPLETE", StringComparison.OrdinalIgnoreCase))
            {
                return new BadRequestObjectResult(new { success = false, results = "", message = "Document already completed", error = "Document has already been confirmed" });
            }

            // 2. ตรวจสอบว่ามีรายการใน doc_return_detail
            const string countDetailSql = @"
                SELECT COUNT(1)
                FROM doc_return_detail ret
                WHERE ret.doc_id = @id
                  AND (ret.is_delete IS NULL OR ret.is_delete = false)";

            var detailCount = await _connection.ExecuteScalarAsync<int>(countDetailSql, new { id }, transaction: _transaction);
            if (detailCount == 0)
            {
                return new BadRequestObjectResult(new { success = false, results = "", message = "Can't Find Item", error = "No return items found" });
            }

            // 3. ดึง warehouse_id จาก doc_trans ร่วมกับรายการใน doc_return_detail (แยกตาม reason_id ของแต่ละรายการ) แล้ว Insert ลง inventory_damage
            const string insertDamageSql = @"
                INSERT INTO inventory_damage (
                    id,
                    doc_id,
                    inv_id,
                    item_id,
                    item_code,
                    item_name,
                    lot_no,
                    serial_no,
                    uom_id,
                    mfg_date,
                    expire_date,
                    item_qty,
                    item_price,
                    reason_id,
                    warehouse_id,
                    is_active,
                    is_delete,
                    create_at,
                    create_by,
                    update_at,
                    update_by
                )
                SELECT
                    UPPER(REPLACE(gen_random_uuid()::text, '-', '')),
                    tran.id,
                    ret.inv_id,
                    ret.item_id,
                    ret.item_code,
                    ret.item_name,
                    ret.lot_no,
                    ret.serial_no,
                    ret.uom_id,
                    ret.mfg_date,
                    ret.expire_date,
                    COALESCE(ret.item_qty, 0)::numeric(10,2),
                    NULLIF(ret.item_price::text, '')::numeric(10,2),
                    ret.reason_id,
                    tran.warehouse_id,
                    true,
                    false,
                    CURRENT_TIMESTAMP,
                    @userId,
                    CURRENT_TIMESTAMP,
                    @userId
                FROM doc_return_detail ret
                INNER JOIN doc_trans tran ON ret.doc_id = tran.id
                WHERE tran.id = @id
                  AND (tran.is_delete IS NULL OR tran.is_delete = false)
                  AND (ret.is_delete IS NULL OR ret.is_delete = false);";

            await _connection.ExecuteAsync(insertDamageSql, new { id, userId = user }, transaction: _transaction);

            // 4. อัปเดตสถานะของ doc_trans เป็น COMPLETED
            const string targetStatusCode = DocStatusConstant.Completed;
            var targetStatusId = await _connection.QueryFirstOrDefaultAsync<string>(
                "SELECT id FROM mast_status WHERE code = @statusCode AND is_delete = false LIMIT 1",
                new { statusCode = targetStatusCode },
                transaction: _transaction);

            const string updateSql = @"
                UPDATE doc_trans
                SET status_id = COALESCE(@statusId, status_id),
                    status_code = @statusCode,
                    update_by = @userId,
                    update_at = CURRENT_TIMESTAMP
                WHERE id = @id
                  AND (is_delete IS NULL OR is_delete = false)";

            var rows = await _connection.ExecuteAsync(updateSql, new
            {
                id,
                statusId = targetStatusId,
                statusCode = targetStatusCode,
                userId = user
            }, transaction: _transaction);

            if (rows > 0)
            {
                // 5. บันทึก Transaction Log
                const string insertLogSql = @"
                    INSERT INTO doc_trans_log (
                        id, doc_id, from_status_id, to_status_id, from_user_id, to_user_id, remark,
                        is_active, is_delete, create_at, create_by, update_at, update_by
                    ) VALUES (
                        @logId, @docId, @fromStatusId, @toStatusId, @userId, @userId, @remark,
                        true, false, CURRENT_TIMESTAMP, @userId, CURRENT_TIMESTAMP, @userId
                    );";

                await _connection.ExecuteAsync(insertLogSql, new
                {
                    logId = Guid.NewGuid().ToString().ToUpper().Replace("-", ""),
                    docId = id,
                    fromStatusId = docTrans.statusId,
                    toStatusId = targetStatusId,
                    userId = user,
                    remark = "Confirm return - recorded to inventory_damage"
                }, transaction: _transaction);

                return new OkObjectResult(new { success = true, results = id, message = "Confirm DocReturn Success", error = "" });
            }

            return new BadRequestObjectResult(new { success = false, results = "", message = "Confirm DocReturn Failed", error = "Document not found or deleted" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }
    
    public async Task<ActionResult> Putaway(string _id, string? userId = null)
    {
        try
        {
            string user = !string.IsNullOrEmpty(userId) ? userId : "system";

            // 1. ตรวจสอบเอกสาร doc_trans และดึง warehouse_id รวมถึงสถานะปัจจุบัน
            var docTrans = await _connection.QueryFirstOrDefaultAsync<DocTransResponse>(
                "SELECT id, warehouse_id AS warehouseId, status_id AS statusId, status_code AS statusCode FROM doc_trans WHERE id = @id AND (is_delete IS NULL OR is_delete = false)",
                new { id = _id },
                transaction: _transaction);

            if (docTrans == null)
            {
                return new BadRequestObjectResult(new { success = false, results = "", message = "Document not found", error = "Document not found or deleted" });
            }

            if (string.IsNullOrEmpty(docTrans.warehouseId))
            {
                return new BadRequestObjectResult(new { success = false, results = "", message = "Warehouse not specified in document", error = "warehouse_id is required in doc_trans" });
            }

            // 2. ตรวจสอบรายการที่ต้องทำการ Putaway (item_qty > putaway_qty)
            string sqlCheckItems = baseSql + "WHERE ret.doc_id = @id AND (ret.is_delete IS NULL OR ret.is_delete = false) AND (ret.item_qty - COALESCE(ret.putaway_qty, 0)) > 0";
            var res = await _connection.QueryAsync<DocReturnDetailResponse>(sqlCheckItems, new { id = _id }, transaction: _transaction);
            if (res == null || !res.Any())
            {
                return new BadRequestObjectResult(new { success = false, results = "", message = "Can't Find Item", error = "No items left to putaway" });
            }

            // 3. ดำเนินการ MERGE เข้าสู่ตาราง inventory
            // นำ warehouse_id จาก doc_trans มาร่วมเป็น source
            const string sqlPutaway = @"
                MERGE INTO inventory AS target
                USING (
                    SELECT 
                        ret.item_id,
                        ret.lot_no,
                        ret.serial_no,
                        ret.location_id,
                        dt.warehouse_id,
                        ret.uom_id,
                        ret.mfg_date,
                        ret.expire_date,
                        NULLIF(ret.item_price, '')::numeric AS item_price,
                        ret.tracking_code,
                        SUM(ret.item_qty - COALESCE(ret.putaway_qty, 0)) AS qty_to_putaway
                    FROM doc_return_detail rec
                    INNER JOIN doc_trans dt ON ret.doc_id = dt.id
                    WHERE ret.doc_id = @id 
                      AND (ret.is_delete IS NULL OR ret.is_delete = false)
                      AND (ret.item_qty - COALESCE(ret.putaway_qty, 0)) > 0
                    GROUP BY 
                        ret.item_id,
                        ret.lot_no,
                        ret.serial_no,
                        ret.location_id,
                        dt.warehouse_id,
                        ret.uom_id,
                        ret.mfg_date,
                        ret.expire_date,
                        NULLIF(ret.item_price, '')::numeric,
                        ret.tracking_code
                ) AS source
                ON target.item_id = source.item_id
                   AND target.warehouse_id = source.warehouse_id
                   AND target.location_id IS NOT DISTINCT FROM source.location_id
                   AND target.lot_no IS NOT DISTINCT FROM source.lot_no
                   AND target.serial_no IS NOT DISTINCT FROM source.serial_no
                   AND target.mfg_date IS NOT DISTINCT FROM source.mfg_date
                   AND target.expire_date IS NOT DISTINCT FROM source.expire_date
                   AND target.item_price IS NOT DISTINCT FROM source.item_price
                   AND (target.is_delete IS NULL OR target.is_delete = false)
                WHEN MATCHED THEN
                    UPDATE SET 
                        item_qty = COALESCE(target.item_qty, 0) + source.qty_to_putaway,
                        update_at = CURRENT_TIMESTAMP,
                        update_by = @userId
                WHEN NOT MATCHED THEN
                    INSERT (
                        id,
                        warehouse_id,
                        location_id,
                        item_id,
                        lot_no,
                        serial_no,
                        uom_id,
                        item_price,
                        item_qty,
                        mfg_date,
                        expire_date,
                        tracking_code,
                        is_active,
                        is_delete,
                        create_at,
                        create_by,
                        update_at,
                        update_by
                    ) VALUES (
                        UPPER(REPLACE(gen_random_uuid()::text, '-', '')),
                        source.warehouse_id,
                        source.location_id,
                        source.item_id,
                        source.lot_no,
                        source.serial_no,
                        source.uom_id,
                        source.item_price,
                        source.qty_to_putaway,
                        source.mfg_date,
                        source.expire_date,
                        source.tracking_code,
                        true,
                        false,
                        CURRENT_TIMESTAMP,
                        @userId,
                        CURRENT_TIMESTAMP,
                        @userId
                    );
            ";

            await _connection.ExecuteAsync(sqlPutaway, new { id = _id, userId = user }, transaction: _transaction);

            // 4. ปรับปรุง putaway_qty ใน doc_return_detail ให้เท่ากับ item_qty
            const string updateDetailSql = @"
                UPDATE doc_return_detail 
                SET putaway_qty = item_qty,
                    update_at = CURRENT_TIMESTAMP,
                    update_by = @userId
                WHERE doc_id = @id 
                  AND (is_delete IS NULL OR is_delete = false);";

            await _connection.ExecuteAsync(updateDetailSql, new { id = _id, userId = user }, transaction: _transaction);

            // 5. ปรับปรุงสถานะของ doc_trans เป็น COMPLETED
            const string updateTransSql = @"
                UPDATE doc_trans 
                SET status_id = (SELECT id FROM mast_status WHERE code = 'COMPLETED' AND is_delete = false LIMIT 1),
                    status_code = 'COMPLETED',
                    update_at = CURRENT_TIMESTAMP,
                    update_by = @userId
                WHERE id = @id 
                  AND (is_delete IS NULL OR is_delete = false);";

            var rows = await _connection.ExecuteAsync(updateTransSql, new { id = _id, userId = user }, transaction: _transaction);

            if (rows > 0)
            {
                var completedStatusId = await _connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT id FROM mast_status WHERE code = 'COMPLETED' AND is_delete = false LIMIT 1",
                    transaction: _transaction);

                // 6. บันทึก Transaction Log
                const string insertLogSql = @"
                    INSERT INTO doc_trans_log (
                        id, doc_id, from_status_id, to_status_id, from_user_id, to_user_id, remark,
                        is_active, is_delete, create_at, create_by, update_at, update_by
                    ) VALUES (
                        @logId, @docId, @fromStatusId, @toStatusId, @userId, @userId, @remark,
                        true, false, CURRENT_TIMESTAMP, @userId, CURRENT_TIMESTAMP, @userId
                    );";

                await _connection.ExecuteAsync(insertLogSql, new
                {
                    logId = Guid.NewGuid().ToString().ToUpper().Replace("-", ""),
                    docId = _id,
                    fromStatusId = docTrans.statusId,
                    toStatusId = completedStatusId,
                    userId = user,
                    remark = "Putaway completed - stock updated to inventory"
                }, transaction: _transaction);

                return new OkObjectResult(new { success = true, results = _id, message = "Putaway DocReturn Success", error = "" });
            }

            return new BadRequestObjectResult(new { success = false, results = "", message = "Putaway DocReturn Failed", error = "Document not found or update failed" });
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
            var currentStatus = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                "SELECT dt.status_id as statusid, dt.status_code as statuscode FROM doc_trans dt WHERE dt.id = @docId AND dt.is_delete = false",
                new { req.docId }, transaction: _transaction);

            if (currentStatus == null)
            {
                return new BadRequestObjectResult(new { success = false, results = "", message = "Document not found", error = "" });
            }

            string? currentStatusId = (string?)currentStatus.statusid;

            const string updateSql = @"
                UPDATE doc_trans 
                SET status_id = COALESCE(@statusId, (SELECT id FROM mast_status WHERE code = @statusCode AND is_delete = false LIMIT 1), status_id),
                    status_code = COALESCE(@statusCode, (SELECT code FROM mast_status WHERE id = @statusId AND is_delete = false LIMIT 1), status_code),
                    update_at = CURRENT_TIMESTAMP, 
                    update_by = @userId 
                WHERE id = @docId AND is_delete = false";

            var rows = await _connection.ExecuteAsync(updateSql, new { req.statusId, req.statusCode, userId, req.docId }, transaction: _transaction);

            if (rows > 0)
            {
                var newStatus = await _connection.QueryFirstOrDefaultAsync<dynamic>(
                    "SELECT dt.status_id as statusid, dt.status_code as statuscode, ms.name_th as nameth FROM doc_trans dt LEFT JOIN mast_status ms ON dt.status_id = ms.id WHERE dt.id = @docId",
                    new { req.docId }, transaction: _transaction);

                string? newStatusId = (string?)newStatus?.statusid;
                string? newStatusCode = (string?)newStatus?.statuscode ?? req.statusCode;
                string? newStatusName = (string?)newStatus?.nameth;

                // บันทึก Log เปลี่ยนสถานะ
                const string insertLogSql = @"
                    INSERT INTO doc_trans_log (
                        id, doc_id, from_status_id, to_status_id, from_user_id, to_user_id, remark,
                        is_active, is_delete, create_at, create_by, update_at, update_by
                    ) VALUES (
                        @logId, @docId, @fromStatusId, @toStatusId, @userId, @userId, @remark,
                        true, false, CURRENT_TIMESTAMP, @userId, CURRENT_TIMESTAMP, @userId
                    )";

                await _connection.ExecuteAsync(insertLogSql, new
                {
                    logId = Guid.NewGuid().ToString().ToUpper().Replace("-", ""),
                    docId = req.docId,
                    fromStatusId = currentStatusId,
                    toStatusId = newStatusId,
                    userId = userId,
                    remark = req.remark ?? $"เปลี่ยนสถานะเป็น {newStatusName ?? newStatusCode}"
                }, transaction: _transaction);

                return new OkObjectResult(new { success = true, results = req.docId, message = $"Update status to {newStatusCode} Success", error = "" });
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
                    update_at = CURRENT_TIMESTAMP, 
                    update_by = @userId 
                WHERE id = @id AND is_delete = false";

            var rows = await _connection.ExecuteAsync(sql, new { id, userId }, transaction: _transaction);
            return rows > 0
                ? new OkObjectResult(new { success = true, results = id, message = "Delete DocReturn Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Delete DocReturn Failed", error = "Document not found" });
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
                st_from.code AS fromStatusCode,
                st_from.name_th AS fromStatusName,
                dtl.to_status_id AS toStatusId,
                st_to.code AS toStatusCode,
                st_to.name_th AS toStatusName,
                dtl.from_user_id AS fromUserId,
                dtl.to_user_id AS toUserId,
                dtl.remark,
                dtl.create_at AS createAt,
                dtl.create_by AS createBy
            FROM doc_trans_log dtl
            LEFT JOIN mast_status st_from ON (dtl.from_status_id = st_from.id OR dtl.from_status_id = st_from.code)
            LEFT JOIN mast_status st_to ON (dtl.to_status_id = st_to.id OR dtl.to_status_id = st_to.code)
            WHERE dtl.doc_id = @docId AND dtl.is_delete = false
            ORDER BY dtl.create_at ASC";

        var logs = await _connection.QueryAsync<DocTransLogResponse>(sql, new { docId }, transaction: _transaction);
        return logs.ToList();
    }
}
