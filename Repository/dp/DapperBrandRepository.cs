using Dapper;
using System.Data;
using stock_api.Interfaces;
using stock_api.Models;
using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Repositories.Dapper;

public class DapperBrandRepository : IBrandRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DapperBrandRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastBrandList>> list(MastBrandRequest req)
    {
        var sql = @"SELECT * FROM mast_brand WHERE is_active = @isActive AND is_delete = @isDelete";
        return (await _connection.QueryAsync<MastBrandList>(sql, req, transaction: _transaction)).ToList();
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