using Dapper;
using System.Data;
using stock_api.Interfaces;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;
using Microsoft.AspNetCore.Mvc;

namespace stock_api.Repositories.Dapper;

public class ReasonRepository : IReasonRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public ReasonRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastReasonResponse>> list(MastReasonRequest req)
    {
        var sql = @"SELECT * FROM mast_reason WHERE is_active = @isActive AND is_delete = @isDelete";
        return (await _connection.QueryAsync<MastReasonResponse>(sql, req, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> create(MastReasonRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_reason(
                        id, 
                        name,
                        create_by, 
                        update_by) VALUES (@id, @name, @createBy, @UpdateBy)";
            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastReason Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastReason Failed", error = "" });

        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<MastReasonResponse?> GetByCode(bool check, string reasonCode)
    {
        var sql = "";
        if (check == true)
            sql = "SELECT * FROM mast_reason WHERE name = @reasonCode AND is_active = true AND is_delete = false";
        else
            sql = "SELECT * FROM mast_reason WHERE name like @reasonCode AND is_active = true AND is_delete = false";

        return await _connection.QueryFirstOrDefaultAsync<MastReasonResponse>(sql, new { reasonCode = check ? reasonCode.Trim() : $"%{reasonCode.Trim()}%" }, transaction: _transaction);
    }

}