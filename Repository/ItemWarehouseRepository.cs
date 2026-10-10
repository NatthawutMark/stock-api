using Dapper;
using System.Data;
using stock_api.Interfaces;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;
using Microsoft.AspNetCore.Mvc;

namespace stock_api.Repositories.Dapper;

public class ItemWarehouseRepository : IItemWarehouseRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public ItemWarehouseRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastItemWarehouseResponse>> list(MastItemWarehouseRequest req)
    {
        var sql = @"SELECT * FROM mast_item_warehouse WHERE is_active = @isActive AND is_delete = @isDelete";
        return (await _connection.QueryAsync<MastItemWarehouseResponse>(sql, req, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> create(MastItemWarehouseRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_item_warehouse(
                        id, 
                        item_id,
                        warehouse_id,
                        create_by, 
                        update_by) VALUES (@id, @itemId, @warehouseId, @createBy, @UpdateBy)";
            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastItemWarehouse Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastItemWarehouse Failed", error = "" });

        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<MastItemWarehouseResponse?> GetByItemIdAndWarehouseId(string itemId, string warehouseId)
    {
        var sql = "SELECT * FROM mast_item_warehouse WHERE item_id = @itemId AND warehouse_id = @warehouseId AND is_active = true AND is_delete = false";
        return await _connection.QueryFirstOrDefaultAsync<MastItemWarehouseResponse>(sql, new { itemId = itemId.Trim(), warehouseId = warehouseId.Trim() }, transaction: _transaction);
    }

    public async Task<List<MastItemWarehouseResponse>> GetByItemId(string itemId)
    {
        var sql = "SELECT * FROM mast_item_warehouse WHERE item_id = @itemId AND is_active = true AND is_delete = false";
        return (await _connection.QueryAsync<MastItemWarehouseResponse>(sql, new { itemId = itemId.Trim() }, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> SyncItemWarehouses(MastItemWarehouseSyncRequest req, string userId)
    {
        try
        {
            var deleteSql = "DELETE FROM mast_item_warehouse WHERE item_id = @itemId";
            await _connection.ExecuteAsync(deleteSql, new { itemId = req.itemId }, transaction: _transaction);

            if (req.warehouseIds != null && req.warehouseIds.Any())
            {
                var insertSql = @"INSERT INTO mast_item_warehouse(id, item_id, warehouse_id, create_by, update_by) 
                                  VALUES (@id, @itemId, @warehouseId, @createBy, @updateBy)";
                var insertData = req.warehouseIds.Select(w => new {
                    id = Guid.NewGuid().ToString().ToUpper().Replace("-", ""),
                    itemId = req.itemId,
                    warehouseId = w,
                    createBy = userId,
                    updateBy = userId
                }).ToList();

                await _connection.ExecuteAsync(insertSql, insertData, transaction: _transaction);
            }
            return new OkObjectResult(new { success = true, results = "", message = "Sync ItemWarehouse Success", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }
}

