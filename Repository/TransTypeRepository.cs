using Dapper;
using System.Data;
using stock_api.Interfaces;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;
using Microsoft.AspNetCore.Mvc;

namespace stock_api.Repositories.Dapper;

public class TransTypeRepository : ITransTypeRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public TransTypeRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastTransTypeResponse>> list(MastTransTypeRequest req)
    {
        var sql = @"SELECT 
                        id,
                        id as menuId,
                        name_th as nameTh,
                        name_en as nameEn,
                        concat(name_th, '(', name_en, ')') as menuName,
                        is_active as isActive,
                        is_delete as isDelete
                    FROM sys_menu 
                    WHERE menu_type = 'TRANSACTION' AND is_active = true AND is_delete = false
                    ORDER BY order_no";
        return (await _connection.QueryAsync<MastTransTypeResponse>(sql, req, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> create(MastTransTypeRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_trans_type(id, name_th, name_en, create_by, update_by) VALUES (@id, @nameTh, @nameEn, @createBy, @UpdateBy)";
            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastTransType Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastTransType Failed", error = "" });

        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<MastTransTypeResponse?> GetByName(bool check, string name)
    {
        var sql = "";
        if (check == true)
            sql = "SELECT * FROM mast_trans_type WHERE name_th = @name AND is_active = true AND is_delete = false";
        else
            sql = "SELECT * FROM mast_trans_type WHERE name_th like @name AND is_active = true AND is_delete = false";

        return await _connection.QueryFirstOrDefaultAsync<MastTransTypeResponse>(sql, new { name = check ? name.Trim() : $"%{name.Trim()}%" }, transaction: _transaction);
    }
}