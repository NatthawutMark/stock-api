using Dapper;
using System.Data;
using stock_api.Interfaces;
using stock_api.Models;
using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;
using Microsoft.AspNetCore.Mvc;

namespace stock_api.Repositories.Dapper;

public class DapperLocationRepository : ILocationRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DapperLocationRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastLocationResponse>> list(MastLocationRequest req)
    {
        var sql = @"SELECT * FROM mast_location WHERE is_active = @isActive AND is_delete = @isDelete";
        return (await _connection.QueryAsync<MastLocationResponse>(sql, req, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> create(MastLocationRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_location(id, code, name, create_by, update_by) VALUES (@id, @code, @name, @createBy, @UpdateBy)";
            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastLocation Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastLocation Failed", error = "" });

        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<MastLocationResponse?> GetByCode(bool check, string code)
    {
        var sql = "";
        if (check == true)
            sql = "SELECT * FROM mast_location WHERE code = @code AND is_active = true AND is_delete = false";
        else
            sql = "SELECT * FROM mast_location WHERE (code like @code) AND is_active = true AND is_delete = false";

        return await _connection.QueryFirstOrDefaultAsync<MastLocationResponse>(sql, new { code = check ? code.Trim() : $"%{code.Trim()}%" }, transaction: _transaction);
    }

    // public async Task<MastBrand?> GetByIdAsync(object id)
    // {
    //     var sql = "SELECT * FROM \"MastBrands\" WHERE \"BrandId\" = @Id";
    //     return await _connection.QueryFirstOrDefaultAsync<MastBrand>(sql, new { Id = id }, transaction: _transaction);
    // }

    // public async Task<IEnumerable<MastBrand>> GetActiveBrandsAsync()
    // {
    //     var sql = "SELECT * FROM \"MastBrands\" WHERE \"IsActive\" = true";
    //     return await _connection.QueryAsync<MastBrand>(sql, transaction: _transaction);
    // }

    // public async Task AddAsync(MastBrand entity)
    // {
    //     var sql = @"INSERT INTO ""MastBrands"" (""BrandName"", ""IsActive"") VALUES (@BrandName, @IsActive)";
    //     await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
    // }

    // public void Update(MastBrand entity)
    // {
    //     var sql = @"UPDATE ""MastBrands"" SET ""BrandName"" = @BrandName, ""IsActive"" = @IsActive WHERE ""BrandId"" = @BrandId";
    //     _connection.Execute(sql, entity, transaction: _transaction);
    // }

    // public void Remove(MastBrand entity)
    // {
    //     var sql = @"DELETE FROM ""MastBrands"" WHERE ""BrandId"" = @BrandId";
    //     _connection.Execute(sql, entity, transaction: _transaction);
    // }
}