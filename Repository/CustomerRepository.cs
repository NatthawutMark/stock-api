using Dapper;
using System.Data;
using stock_api.Interfaces;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;
using Microsoft.AspNetCore.Mvc;

namespace stock_api.Repositories.Dapper;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public CustomerRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastCustomerResponse>> list(MastCustomerRequest req)
    {
        var sql = @"SELECT * FROM mast_customer WHERE is_active = @isActive AND is_delete = @isDelete";
        return (await _connection.QueryAsync<MastCustomerResponse>(sql, req, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> create(MastCustomerRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_customer(
                        id, 
                        cust_code, 
                        cust_name, 
                        contact_name, 
                        tel, 
                        address, 
                        remark, 
                        create_by, 
                        update_by) VALUES (@id, @custCode, @custName, @contactName, @tel, @address, @remark, @createBy, @UpdateBy)";
            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastCustomer Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastCustomer Failed", error = "" });

        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<MastCustomerResponse?> GetByCode(bool check, string custCode)
    {
        var sql = "";
        if (check == true)
            sql = "SELECT * FROM mast_customer WHERE cust_code = @custCode AND is_active = true AND is_delete = false";
        else
            sql = "SELECT * FROM mast_customer WHERE cust_code like @custCode AND is_active = true AND is_delete = false";

        return await _connection.QueryFirstOrDefaultAsync<MastCustomerResponse>(sql, new { custCode = check ? custCode.Trim() : $"%{custCode.Trim()}%" }, transaction: _transaction);
    }

}