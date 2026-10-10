using Dapper;
using System.Data;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Repositories.Dapper;

public class ItemRepository : IItemRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public ItemRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastItemResponse>> GetAll(MastItemRequest req)
    {
        try
        {
            var sql = @"select i.id,
                    i.item_code as itemCode,
                    i.item_name as itemName,
                    i.brand_id as brandId,
                    mb.name_th as brandName,
                    i.min_alter as minAlert,
                    i.max_alter as maxAlert,
                    i.uom_id as uomId,
                    mu.name as uomName,
                    i.location_id as locationId,
                    ml.name as locationName, 
                    COALESCE(i.buy_price, 0.00) as buyPrice,
                    COALESCE(i.sell_price, 0.00) as sellPrice,
                    COALESCE(i.is_lotno, false) as isLotno,
                    COALESCE(i.is_serialno, false) as isSerialNo,
                    COALESCE(i.is_active, true) as isActive
                    from mast_item i
                    left outer join mast_brand mb on i.brand_id = mb.id
                    left outer join mast_location ml on i.location_id = ml.id
                    left outer join mast_uom mu on i.uom_id = mu.id
                    where i.is_delete = false";

            if (req != null && req.isActive.HasValue)
            {
                sql += " AND i.is_active = @isActive";
            }
            if (!string.IsNullOrEmpty(req?.itemCode))
            {
                sql += " AND (i.item_code ILIKE @Search OR i.item_name ILIKE @Search)";
            }
            sql += " ORDER BY i.item_code ASC";

            var result = await _connection.QueryAsync<MastItemResponse>(sql, new {
                isActive = req?.isActive,
                Search = $"%{req?.itemCode}%"
            }, transaction: _transaction);
            return result.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error :{ex.Message}");
        }
    }

    public async Task<MastItemResponse?> GetByCode(string itemCode)
    {
        var sql = @"select i.id,
                i.item_code as itemCode,
                i.item_name as itemName,
                i.brand_id as brandId,
                mb.name_th as brandName,
                i.min_alter as minAlert,
                i.max_alter as maxAlert,
                i.uom_id as uomId,
                mu.name as uomName,
                i.location_id as locationId,
                ml.name as locationName, 
                COALESCE(i.buy_price, 0.00) as buyPrice,
                COALESCE(i.sell_price, 0.00) as sellPrice,
                COALESCE(i.is_lotno, false) as isLotno,
                COALESCE(i.is_serialno, false) as isSerialNo,
                COALESCE(i.is_active, true) as isActive
                from mast_item i
                left outer join mast_brand mb on i.brand_id = mb.id
                left outer join mast_location ml on i.location_id = ml.id
                left outer join mast_uom mu on i.uom_id = mu.id
                where i.item_code = @itemCode and i.is_delete = false";
        return await _connection.QueryFirstOrDefaultAsync<MastItemResponse>(sql, new { itemCode = itemCode.Trim() }, transaction: _transaction);
    }

    public async Task<ActionResult> create(MastItemRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_item (
                id, item_code, item_name, brand_id, min_alter, max_alter,
                uom_id, location_id, buy_price, sell_price, is_lotno, is_serialno,
                is_active, is_delete, create_by, update_by, create_date, update_date
            ) VALUES (
                @id, @itemCode, @itemName, @brandId, @minAlert, @maxAlert,
                @uomId, @locationId, @buyPrice, @sellPrice, COALESCE(@isLotno, false), COALESCE(@isSerialNo, false),
                COALESCE(@isActive, true), false, @createBy, @UpdateBy, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
            )";

            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastItem Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastItem Failed", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> update(MastItemRequest req)
    {
        try
        {
            var sql = @"UPDATE mast_item 
                        SET item_code = @itemCode,
                            item_name = @itemName,
                            brand_id = @brandId,
                            min_alter = @minAlert,
                            max_alter = @maxAlert,
                            uom_id = @uomId,
                            location_id = @locationId,
                            buy_price = @buyPrice,
                            sell_price = @sellPrice,
                            is_lotno = COALESCE(@isLotno, is_lotno),
                            is_serialno = COALESCE(@isSerialNo, is_serialno),
                            is_active = COALESCE(@isActive, is_active),
                            update_by = @UpdateBy,
                            update_date = CURRENT_TIMESTAMP
                        WHERE id = @id";
            var affected = await _connection.ExecuteAsync(sql, req, transaction: _transaction);
            return affected > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Update MastItem Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Update MastItem Failed", error = "Item not found" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> delete(string id)
    {
        try
        {
            var sql = @"UPDATE mast_item SET is_delete = true, update_date = CURRENT_TIMESTAMP WHERE id = @id";
            var affected = await _connection.ExecuteAsync(sql, new { id }, transaction: _transaction);
            return affected > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Delete MastItem Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Delete MastItem Failed", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }
}
