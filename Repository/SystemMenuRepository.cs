using Dapper;
using System.Data;
using stock_api.Interfaces;
using stock_api.Request;

namespace stock_api.Repositories.Dapper;

public class SystemMenuRepository : ISystemMenuRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public SystemMenuRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<IEnumerable<SystemMenuRequest>> GetAllAsync()
    {
        var sql = "SELECT * FROM sys_menu";
        return await _connection.QueryAsync<SystemMenuRequest>(sql, transaction: _transaction);
    }

    public async Task<SystemMenuRequest?> GetByIdAsync(object id)
    {
        var sql = "SELECT * FROM sys_menu WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<SystemMenuRequest>(sql, new { Id = id }, transaction: _transaction);
    }

    public async Task<SystemMenuRequest?> GetByCodeAsync(object code)
    {
        var sql = "SELECT * FROM sys_menu WHERE code = @Code";
        return await _connection.QueryFirstOrDefaultAsync<SystemMenuRequest>(sql, new { Code = code }, transaction: _transaction);
    }

    public async Task AddAsync(SystemMenuRequest entity)
    {
        var sql = @"INSERT INTO sys_menu (id, parent_id, name_th, name_en, create_by, update_by, update_date) 
                    VALUES (@Id, @ParentId, @NameTh, @NameEn, @CreateBy, @UpdateBy, @UpdateDate)";
        await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
    }

    public void Update(SystemMenuRequest entity)
    {
        var sql = @"UPDATE sys_menu SET name_th = @NameTh, name_en = @NameEn, is_active = @IsActive WHERE id = @Id";
        _connection.Execute(sql, entity, transaction: _transaction);
    }

    public void Remove(SystemMenuRequest entity)
    {
        var sql = @"DELETE FROM sys_menu WHERE id = @Id";
        _connection.Execute(sql, entity, transaction: _transaction);
    }
}