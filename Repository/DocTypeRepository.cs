using Dapper;
using System.Data;
using stock_api.Interfaces;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;
using Microsoft.AspNetCore.Mvc;

namespace stock_api.Repositories.Dapper;

public class DocTypeRepository : IDocTypeRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DocTypeRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastDocTypeResponse>> list(MastDocTypeRequest req)
    {

        var sql = @"select 
	                    docType.id,
	                    menu.id as menuId,
	                    concat(menu.name_th ,'(',menu.name_En,')') as MenuName,
	                    docType.name as DocTypeName,
	                    docType.is_active,
	                    docType.is_delete
                    FROM mast_doc_type docType
                    inner join sys_menu menu on docType.menu_id  = menu.id 
                    WHERE menu.is_active = true AND ";
        if (req != null && req.isActive.HasValue)
        {
            sql += @"docType.is_active = @isActive AND ";
        }
        if (req != null && !string.IsNullOrEmpty(req.menuId))
        {
            sql += @"docType.menu_id = @menuId AND ";
        }
        if (req != null && !string.IsNullOrEmpty(req.docTypeName))
        {
            sql += @"docType.name ILIKE @namePattern AND ";
        }
        sql += @"docType.is_delete = @isDelete";

        var param = new
        {
            isActive = req?.isActive,
            isDelete = req?.isDelete ?? false,
            menuId = req?.menuId,
            namePattern = req != null && !string.IsNullOrEmpty(req.docTypeName) ? $"%{req.docTypeName.Trim()}%" : null
        };
        return (await _connection.QueryAsync<MastDocTypeResponse>(sql, param, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> create(MastDocTypeRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_doc_type(
                        id, 
                        menu_id,
                        name,
                        create_by, 
                        update_by) VALUES (@id, @menuId, @name, @createBy, @UpdateBy)";
            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastDocType Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastDocType Failed", error = "" });

        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }
    public async Task<ActionResult> update(MastDocTypeRequest req)
    {
        try
        {
            var sql = @"UPDATE mast_doc_type 
                        SET name = @name, 
                            menu_id = COALESCE(@menuId, menu_id), 
                            update_by = @UpdateBy, 
                            update_date = @UpdateDate, 
                            is_active = @isActive 
                        WHERE id = @id";
            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Update MastDocType Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Update MastDocType Failed", error = "" });

        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<MastDocTypeResponse?> GetByName(bool check, string nameTh, string menuId)
    {
        var sql = "";
        if (string.IsNullOrEmpty(menuId))
        {
            if (check == true)
                sql = "SELECT * FROM mast_doc_type WHERE name = @nameTh AND is_delete = false";
            else
                sql = "SELECT * FROM mast_doc_type WHERE name like @nameTh AND is_delete = false";
        }
        else
        {
            if (check == true)
                sql = "SELECT * FROM mast_doc_type WHERE name = @nameTh AND menu_id = @menuId AND is_delete = false";
            else
                sql = "SELECT * FROM mast_doc_type WHERE name like @nameTh AND menu_id like @menuId AND is_delete = false";
        }

        return await _connection.QueryFirstOrDefaultAsync<MastDocTypeResponse>(sql, new { nameTh = check ? nameTh.Trim() : $"%{nameTh.Trim()}%", menuId = check ? menuId : $"%{menuId}%" }, transaction: _transaction);
    }
}

