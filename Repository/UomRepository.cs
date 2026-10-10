using Dapper;
using System.Data;
using stock_api.Interfaces;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;
using Microsoft.AspNetCore.Mvc;

namespace stock_api.Repositories.Dapper;

public class UomRepository : IUomRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public UomRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastUomResponse>> list(MastUomRequest req)
    {
        var sql = @"SELECT * FROM mast_uom WHERE is_active = COALESCE(@isActive, is_active) AND is_delete = @isDelete";
        return (await _connection.QueryAsync<MastUomResponse>(sql, req, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> create(MastUomRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_uom(id, name, create_by, update_by, is_active, is_delete) VALUES (@id, @name, @createBy, @UpdateBy, COALESCE(@isActive, true), false)";
            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastUom Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastUom Failed", error = "" });

        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> update(MastUomRequest req)
    {
        try
        {
            var sql = @"UPDATE mast_uom 
                        SET name = @name, 
                            is_active = COALESCE(@isActive, is_active), 
                            update_by = COALESCE(@UpdateBy, update_by), 
                            update_date = CURRENT_TIMESTAMP 
                        WHERE id = @id";
            var affected = await _connection.ExecuteAsync(sql, req, transaction: _transaction);
            return affected > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Update MastUom Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Update MastUom Failed", error = "UOM not found" });
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
            var sql = @"UPDATE mast_uom SET is_delete = true, update_date = CURRENT_TIMESTAMP WHERE id = @id";
            var affected = await _connection.ExecuteAsync(sql, new { id }, transaction: _transaction);
            return affected > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Delete MastUom Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Delete MastUom Failed", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<MastUomResponse?> GetByName(bool check, string name)
    {
        var sql = "";
        if (check == true)
            sql = "SELECT * FROM mast_uom WHERE name = @name AND is_active = true AND is_delete = false";
        else
            sql = "SELECT * FROM mast_uom WHERE name like @name AND is_active = true AND is_delete = false";

        return await _connection.QueryFirstOrDefaultAsync<MastUomResponse>(sql, new { name = check ? name.Trim() : $"%{name.Trim()}%" }, transaction: _transaction);
    }
}
