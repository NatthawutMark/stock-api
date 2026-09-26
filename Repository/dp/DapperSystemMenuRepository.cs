using Dapper;
using System.Data;
using stock_api.Interfaces;
using stock_api.Models;

namespace stock_api.Repositories.Dapper;

public class DapperSystemMenuRepository : ISystemMenuRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DapperSystemMenuRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<IEnumerable<SysMenu>> GetAllAsync()
    {
        var sql = "SELECT * FROM SysMenu";
        return await _connection.QueryAsync<SysMenu>(sql, transaction: _transaction);
    }

    public async Task<SysMenu?> GetByIdAsync(object id)
    {
        var sql = "SELECT * FROM SysMenu WHERE ID = @Id";
        return await _connection.QueryFirstOrDefaultAsync<SysMenu>(sql, new { Id = id }, transaction: _transaction);
    }
    public async Task<SysMenu?> GetByCodeAsync(object code)
    {
        var sql = "SELECT * FROM SysMenu WHERE Code = @Code";
        return await _connection.QueryFirstOrDefaultAsync<SysMenu>(sql, new { Code = code }, transaction: _transaction);
    }

    public async Task<IEnumerable<SysMenu>> GetActiveWarehousesAsync()
    {
        var sql = "SELECT * FROM SysMenu WHERE IsActive = true";
        return await _connection.QueryAsync<SysMenu>(sql, transaction: _transaction);
    }
    
    public async Task AddAsync(SysMenu entity)
    {
        var sql = @"INSERT INTO Sys_Menu (id, parent_id, name_th, name_en, Create_By, Update_By) VALUES (@id, @ParentId, @NameTh, @NameEn, @CreateBy, @UpdateBy)";
        await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
    }

    public void Update(SysMenu entity)
    {
        var sql = @"UPDATE Sys_Menu SET name_th = @NameTh, name_en = @NameEn, IsActive = @IsActive WHERE id = @id";
        _connection.Execute(sql, entity, transaction: _transaction);
    }

    public void Remove(SysMenu entity)
    {
        var sql = @"DELETE FROM Sys_Menu WHERE MenuId = @MenuId";
        _connection.Execute(sql, entity, transaction: _transaction);
    }
}