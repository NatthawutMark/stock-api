using Dapper;
using System.Data;
using stock_api.Interfaces;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;
using Microsoft.AspNetCore.Mvc;

namespace stock_api.Repositories.Dapper;

public class VendorRepository : IVendorRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public VendorRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastVendorResponse>> list(MastVendorRequest req)
    {
        var sql = @"SELECT * FROM mast_vendor WHERE is_active = @isActive AND is_delete = @isDelete";
        return (await _connection.QueryAsync<MastVendorResponse>(sql, req, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> create(MastVendorRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_vendor(
                        id, 
                        vend_code, 
                        vend_name, 
                        contact_name, 
                        tel, 
                        address, 
                        remark, 
                        create_by, 
                        update_by) VALUES (@id, @vendCode, @vendName, @contactName, @tel, @address, @remark, @createBy, @UpdateBy)";
            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastVendor Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastVendor Failed", error = "" });

        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> update(MastVendorRequest req)
    {
        try
        {
            var sql = @"UPDATE mast_vendor 
                        SET vend_code = COALESCE(@vendCode, vend_code),
                            vend_name = @vendName, 
                            contact_name = @contactName, 
                            tel = @tel, 
                            address = @address, 
                            remark = @remark, 
                            is_active = COALESCE(@isActive, is_active), 
                            update_by = COALESCE(@UpdateBy, update_by), 
                            update_date = CURRENT_TIMESTAMP 
                        WHERE id = @id OR vend_code = @vendCode";
            var affected = await _connection.ExecuteAsync(sql, req, transaction: _transaction);
            return affected > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Update MastVendor Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Update MastVendor Failed", error = "Vendor not found" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<ActionResult> delete(string id)
    {
        try
        {
            var sql = @"UPDATE mast_vendor SET is_delete = true, update_date = CURRENT_TIMESTAMP WHERE id = @id OR vend_code = @id";
            var affected = await _connection.ExecuteAsync(sql, new { id }, transaction: _transaction);
            return affected > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Delete MastVendor Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Delete MastVendor Failed", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<MastVendorResponse?> GetByCode(bool check, string vendorCode)
    {
        var sql = "";
        if (check == true)
            sql = "SELECT * FROM mast_vendor WHERE vend_code = @vendorCode AND is_active = true AND is_delete = false";
        else
            sql = "SELECT * FROM mast_vendor WHERE vend_code like @vendorCode AND is_active = true AND is_delete = false";

        return await _connection.QueryFirstOrDefaultAsync<MastVendorResponse>(sql, new { vendorCode = check ? vendorCode.Trim() : $"%{vendorCode.Trim()}%" }, transaction: _transaction);
    }

}