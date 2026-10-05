using Dapper;
using System.Data;
using stock_api.Interfaces;
using static stock_api.response.MasterResponse;
using static stock_api.Request.MasterRequest;

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
        var sql = "SELECT * FROM mast_warehouse WHERE is_delete = false";
        return await _connection.QueryAsync<MastWarehouseResponse>(sql, transaction: _transaction);
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
        var sql = "SELECT * FROM mast_warehouse WHERE is_active = true AND is_delete = false";
        return await _connection.QueryAsync<MastWarehouseResponse>(sql, transaction: _transaction);
    }

    public async Task AddAsync(MastWarehouseRequest entity)
    {
        var sql = @"INSERT INTO mast_warehouse (id, code, warehouse_name, description, is_active, is_delete, create_by, update_by, update_date) 
                    VALUES (@id, @Code, @WarehouseName, @description, COALESCE(@isActive, true), COALESCE(@isDelete, false), @createBy, @UpdateBy, CURRENT_TIMESTAMP)";
        await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
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