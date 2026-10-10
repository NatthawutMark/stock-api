using Dapper;
using System.Data;
using stock_api.Interfaces;
using static stock_api.response.MasterResponse;
using static stock_api.Request.MasterRequest;
using Microsoft.AspNetCore.Mvc;

namespace stock_api.Repositories.Dapper;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public WarehouseRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<IEnumerable<MastWarehouseResponse>> GetAllAsync()
    {
        var sql = "SELECT * FROM mast_warehouse WHERE is_delete = false ORDER BY code ASC";
        return await _connection.QueryAsync<MastWarehouseResponse>(sql, transaction: _transaction);
    }

    public async Task<List<MastWarehouseResponse>> list(MastWarehouseRequest req)
    {
        var sql = "SELECT * FROM mast_warehouse WHERE is_delete = false";
        if (req != null && req.isActive.HasValue)
        {
            sql += " AND is_active = @isActive";
        }
        if (!string.IsNullOrEmpty(req?.Code))
        {
            sql += " AND (code ILIKE @Search OR warehouse_name ILIKE @Search)";
        }
        sql += " ORDER BY code ASC";
        return (await _connection.QueryAsync<MastWarehouseResponse>(sql, new {
            isActive = req?.isActive,
            Search = $"%{req?.Code}%"
        }, transaction: _transaction)).ToList();
    }

    public async Task<MastWarehouseResponse?> GetByIdAsync(object id)
    {
        var sql = "SELECT * FROM mast_warehouse WHERE id = @Id AND is_delete = false";
        return await _connection.QueryFirstOrDefaultAsync<MastWarehouseResponse>(sql, new { Id = id }, transaction: _transaction);
    }

    public async Task<MastWarehouseResponse?> GetByCodeAsync(object code)
    {
        var sql = "SELECT * FROM mast_warehouse WHERE code = @Code AND is_delete = false";
        return await _connection.QueryFirstOrDefaultAsync<MastWarehouseResponse>(sql, new { Code = code }, transaction: _transaction);
    }

    public async Task<IEnumerable<MastWarehouseResponse>> GetActiveWarehousesAsync()
    {
        var sql = "SELECT * FROM mast_warehouse WHERE is_active = true AND is_delete = false ORDER BY code ASC";
        return await _connection.QueryAsync<MastWarehouseResponse>(sql, transaction: _transaction);
    }

    public async Task AddAsync(MastWarehouseRequest entity)
    {
        var sql = @"INSERT INTO mast_warehouse (id, code, warehouse_name, description, is_active, is_delete, create_by, update_by, update_date) 
                    VALUES (@id, @Code, @WarehouseName, @description, COALESCE(@isActive, true), COALESCE(@isDelete, false), @createBy, @UpdateBy, CURRENT_TIMESTAMP)";
        await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
    }

    public async Task<ActionResult> create(MastWarehouseRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_warehouse (id, code, warehouse_name, description, is_active, is_delete, create_by, update_by, update_date) 
                        VALUES (@id, @Code, @WarehouseName, @description, COALESCE(@isActive, true), false, @createBy, @UpdateBy, CURRENT_TIMESTAMP)";
            var affected = await _connection.ExecuteAsync(sql, req, transaction: _transaction);
            return affected > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastWarehouse Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastWarehouse Failed", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> update(MastWarehouseRequest req)
    {
        try
        {
            var sql = @"UPDATE mast_warehouse 
                        SET 
                            warehouse_name = @WarehouseName, 
                            description = @description, 
                            is_active = COALESCE(@isActive, is_active), 
                            update_by = @UpdateBy, 
                            update_date = CURRENT_TIMESTAMP 
                        WHERE id = @id";
            var affected = await _connection.ExecuteAsync(sql, req, transaction: _transaction);
            return affected > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Update MastWarehouse Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Update MastWarehouse Failed", error = "Warehouse not found" });
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
            var sql = @"UPDATE mast_warehouse SET is_delete = true, update_date = CURRENT_TIMESTAMP WHERE id = @id";
            var affected = await _connection.ExecuteAsync(sql, new { id }, transaction: _transaction);
            return affected > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Delete MastWarehouse Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Delete MastWarehouse Failed", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public void Update(MastWarehouseRequest entity)
    {
        var sql = @"UPDATE mast_warehouse 
                    SET warehouse_name = @WarehouseName, description = @description, is_active = @isActive, update_by = @UpdateBy, update_date = CURRENT_TIMESTAMP 
                    WHERE id = @id";
        _connection.Execute(sql, entity, transaction: _transaction);
    }

    public void Remove(MastWarehouseRequest entity)
    {
        var sql = @"UPDATE mast_warehouse SET is_delete = true, update_date = CURRENT_TIMESTAMP WHERE id = @id";
        _connection.Execute(sql, entity, transaction: _transaction);
    }
}
