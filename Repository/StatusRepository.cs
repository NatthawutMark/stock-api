using Dapper;
using System.Data;
using stock_api.Interfaces;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;
using Microsoft.AspNetCore.Mvc;

namespace stock_api.Repositories.Dapper;

public class StatusRepository : IStatusRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public StatusRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastStatusResponse>> list(MastStatusRequest req)
    {
        var sql = @"SELECT * FROM mast_status WHERE is_active = @isActive AND is_delete = @isDelete";
        return (await _connection.QueryAsync<MastStatusResponse>(sql, req, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> create(MastStatusRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_status(
                        id, 
                        trans_type_id,
                        code,
                        name_th,
                        name_en,
                        order_no,
                        create_by, 
                        update_by) VALUES (@id, @transTypeId, @code, @nameTh, @nameEn, @orderNo, @createBy, @UpdateBy)";
            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastStatus Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastStatus Failed", error = "" });

        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<MastStatusResponse?> GetByCode(bool check, string code)
    {
        var sql = "";
        if (check == true)
            sql = "SELECT * FROM mast_status WHERE code = @code AND is_active = true AND is_delete = false";
        else
            sql = "SELECT * FROM mast_status WHERE code like @code AND is_active = true AND is_delete = false";

        return await _connection.QueryFirstOrDefaultAsync<MastStatusResponse>(sql, new { code = check ? code.Trim() : $"%{code.Trim()}%" }, transaction: _transaction);
    }
}

