using Dapper;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using stock_api.Interfaces;
using static stock_api.Request.TransactionRequest;
using static stock_api.Response.TransactionResponse;
namespace stock_api.Repositories;

public class DocReceiveRepository : IDocReceiveRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DocReceiveRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    private const string baseSql = @"select 
                                    rec.id ,
                                    rec.doc_id as docId,
                                    rec.item_id as itemId,
                                    item.item_code as itemCode,
                                    rec.item_name as itemName,
                                    rec.lot_no as lotNo,
                                    rec.serial_no as serialNo,
                                    rec.location_id as locationId,
                                    loc.name as locationName,
                                    rec.uom_id as uomId,
                                    uom.name as uomName,
                                    rec.item_qty as itemQty,
                                    rec.item_price as itemPrice,
                                    item.is_lotno as isLotNo,
                                    item.is_serialNo as isSerialNo,
                                    rec.putaway_qty as putawayQty,
                                    rec.mfg_date as mfgDate,
                                    rec.expire_date as expireDate,
                                    rec.tracking_code as tracking_code
                                    FROM 
                                    doc_receive_detail rec
                                    LEFT JOIN mast_item item on rec.item_id = item.id
                                    LEFT JOIN mast_uom uom on rec.uom_id = uom.id 
                                    LEFT JOIN mast_location loc on rec.location_id = loc.id ";

    public async Task<List<DocReceiveDetailResponse>> List(DocReceiveDetailRequest req)
    {
        try
        {
            string sql = baseSql + "where rec.is_delete = @isDelete";
            if (!string.IsNullOrEmpty(req.docId))
                sql += " and rec.doc_id = @docId";
            var param = new
            {
                docId = req.docId,
                isDelete = req.isDelete
            };
            var res = await _connection.QueryAsync<DocReceiveDetailResponse>(sql, param, transaction: _transaction);
            return res.ToList();
        }
        catch (Exception ex)
        {

            throw new Exception($"Error : {ex.Message}, {ex.InnerException?.Message}");
        }
    }
    public async Task<List<DocReceiveDetailResponse>> getListById(string _id)
    {
        try
        {
            string sql = baseSql + "where rec.doc_id = @id";
            var res = await _connection.QueryAsync<DocReceiveDetailResponse>(sql, new { id = _id }, transaction: _transaction);
            return res.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error : {ex.Message}, {ex.InnerException?.Message}");
        }
    }
    public async Task<ActionResult> CreateList(List<DocReceiveDetailRequest> req)
    {
        try
        {
            const string insertDocReceiveSql = @"
                INSERT INTO public.doc_receive_detail (
                id, doc_id, item_id, item_name, lot_no,
                serial_no, location_id, uom_id, mfg_date, expire_date,
                item_qty,item_price, putaway_qty, create_by, update_by
                ) 
                VALUES (
                @id, @docId, @itemId, @itemName, @lotNo,
                @serialNo, @locationId,@uomId, @mfgDate, @expireDate,
                @itemQty, @itemPrice, @putawayQty, @createBy, @updateBy
                )";

            var rows = await _connection.ExecuteAsync(insertDocReceiveSql, req, transaction: _transaction);

            return new OkObjectResult(new { success = false, results = "", message = "Create DocReceive Failed", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> Update(string id, List<DocReceiveDetailRequest> req)
    {
        try
        {
            string delItems = @"DELETE FROM doc_receive_detail where doc_id = @id";
            await _connection.QueryAsync(delItems, new { id }, _transaction);

            const string updateSql = @"
                INSERT INTO doc_receive_detail (
                id, doc_id, item_id, item_name, lot_no,
                serial_no, location_id, uom_id, mfg_date, expire_date,
                item_qty,item_price, putaway_qty, create_by, update_by
                ) 
                VALUES (
                @id, @docId, @itemId, @itemName, coalesce(@lotNo,NULL),
                @serialNo, @locationId,@uomId, @mfgDate, @expireDate,
                @itemQty, @itemPrice, @putawayQty, @createBy, @updateBy
                )";

            var rows = await _connection.ExecuteAsync(updateSql, req, transaction: _transaction);
            return rows > 0
                ? new OkObjectResult(new { success = true, results = id, message = "Update DocReceive Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Update DocReceive Failed", error = "Document not found or deleted" });
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
            string updateDetail = @"UPDATE doc_receive_detail set putaway_qty = 0 where doc_id = @id";
            await _connection.ExecuteAsync(updateDetail, new { id }, _transaction);

            string updateSql = @"UPDATE doc_trans set STATUS_CODE = 'CONFIRM' where id = @id";

            var rows = await _connection.ExecuteAsync(updateSql, new { id }, transaction: _transaction);
            return rows > 0
                ? new OkObjectResult(new { success = true, results = id, message = "Update DocReceive Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Update DocReceive Failed", error = "Document not found or deleted" });
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
                "SELECT id, warehouse_id AS warehouseId, status_code AS statusCode FROM doc_trans WHERE id = @id AND (is_delete IS NULL OR is_delete = false)",
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
            string sqlCheckItems = baseSql + "WHERE rec.doc_id = @id AND (rec.is_delete IS NULL OR rec.is_delete = false) AND (rec.item_qty - COALESCE(rec.putaway_qty, 0)) > 0";
            var res = await _connection.QueryAsync<DocReceiveDetailResponse>(sqlCheckItems, new { id = _id }, transaction: _transaction);
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
                        rec.item_id,
                        rec.lot_no,
                        rec.serial_no,
                        rec.location_id,
                        dt.warehouse_id,
                        rec.uom_id,
                        rec.mfg_date,
                        rec.expire_date,
                        NULLIF(rec.item_price, '')::numeric AS item_price,
                        rec.tracking_code,
                        SUM(rec.item_qty - COALESCE(rec.putaway_qty, 0)) AS qty_to_putaway
                    FROM doc_receive_detail rec
                    INNER JOIN doc_trans dt ON rec.doc_id = dt.id
                    WHERE rec.doc_id = @id 
                      AND (rec.is_delete IS NULL OR rec.is_delete = false)
                      AND (rec.item_qty - COALESCE(rec.putaway_qty, 0)) > 0
                    GROUP BY 
                        rec.item_id,
                        rec.lot_no,
                        rec.serial_no,
                        rec.location_id,
                        dt.warehouse_id,
                        rec.uom_id,
                        rec.mfg_date,
                        rec.expire_date,
                        NULLIF(rec.item_price, '')::numeric,
                        rec.tracking_code
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
                        update_date = CURRENT_TIMESTAMP,
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
                        create_date,
                        create_by,
                        update_date,
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

            // 4. ปรับปรุง putaway_qty ใน doc_receive_detail ให้เท่ากับ item_qty
            const string updateDetailSql = @"
                UPDATE doc_receive_detail 
                SET putaway_qty = item_qty,
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
                    remark = "Putaway completed - stock updated to inventory"
                }, transaction: _transaction);

                return new OkObjectResult(new { success = true, results = _id, message = "Putaway DocReceive Success", error = "" });
            }

            return new BadRequestObjectResult(new { success = false, results = "", message = "Putaway DocReceive Failed", error = "Document not found or update failed" });
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
