using Dapper;
using System.Data;
using back_stock.Interfaces;
using back_stock.Models;

namespace back_stock.Repositories.Dapper;

public class DapperWarehouseRepository : IWarehouseRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DapperWarehouseRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<IEnumerable<MastWarehouse>> GetAllAsync()
    {
        var sql = "SELECT * FROM MastWarehouses";
        return await _connection.QueryAsync<MastWarehouse>(sql, transaction: _transaction);
    }

    public async Task<MastWarehouse?> GetByIdAsync(object id)
    {
        var sql = "SELECT * FROM mast_warehouse WHERE ID = @Id";
        return await _connection.QueryFirstOrDefaultAsync<MastWarehouse>(sql, new { Id = id }, transaction: _transaction);
    }

    public async Task<IEnumerable<MastWarehouse>> GetActiveWarehousesAsync()
    {
        var sql = "SELECT * FROM MastWarehouses WHERE IsActive = true";
        return await _connection.QueryAsync<MastWarehouse>(sql, transaction: _transaction);
    }

    public async Task AddAsync(MastWarehouse entity)
    {
        var sql = @"INSERT INTO ""MastWarehouses"" (""WarehouseName"", ""IsActive"") VALUES (@WarehouseName, @IsActive)";
        await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
    }

    public void Update(MastWarehouse entity)
    {
        var sql = @"UPDATE ""MastWarehouses"" SET ""WarehouseName"" = @WarehouseName, ""IsActive"" = @IsActive WHERE ""WarehouseId"" = @WarehouseId";
        _connection.Execute(sql, entity, transaction: _transaction);
    }

    public void Remove(MastWarehouse entity)
    {
        var sql = @"DELETE FROM ""MastBrands"" WHERE ""BrandId"" = @BrandId";
        _connection.Execute(sql, entity, transaction: _transaction);
    }
}