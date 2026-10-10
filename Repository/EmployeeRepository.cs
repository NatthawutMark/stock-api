using Dapper;
using System.Data;
using stock_api.Interfaces;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;
using Microsoft.AspNetCore.Mvc;

namespace stock_api.Repositories.Dapper;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public EmployeeRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastEmployeeResponse>> list(MastEmployeeRequest req)
    {
        var sql = @"SELECT * FROM mast_employee WHERE is_active = @isActive AND is_delete = @isDelete";
        return (await _connection.QueryAsync<MastEmployeeResponse>(sql, req, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> create(MastEmployeeRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_employee(
                        id, 
                        emp_code,
                        f_name,
                        l_name,
                        tel,
                        create_by, 
                        update_by) VALUES (@id, @empCode, @fName, @lName, @tel, @createBy, @UpdateBy)";
            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastEmployee Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastEmployee Failed", error = "" });

        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<MastEmployeeResponse?> GetByCode(bool check, string empCode)
    {
        var sql = "";
        if (check == true)
            sql = "SELECT * FROM mast_employee WHERE emp_code = @empCode AND is_active = true AND is_delete = false";
        else
            sql = "SELECT * FROM mast_employee WHERE emp_code like @empCode AND is_active = true AND is_delete = false";

        return await _connection.QueryFirstOrDefaultAsync<MastEmployeeResponse>(sql, new { empCode = check ? empCode.Trim() : $"%{empCode.Trim()}%" }, transaction: _transaction);
    }
}

