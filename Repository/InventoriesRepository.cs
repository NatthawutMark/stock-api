using Dapper;
using System.Data;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using stock_api.Request;
using static stock_api.Response.InventoryResponse;
using static stock_api.Request.InventoryRequest;

namespace stock_api.Repositories;

public class InventoriesRepository : IInventoriesRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public InventoriesRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    const string baseSql = @"SELECT
	                            inv.id,
                                inv.item_id as itemId,
	                            i.item_code AS itemCode,
	                            i.item_name AS itemName,
	                            inv.lot_no AS lotNo,
	                            inv.serial_no AS serialNo,
	                            inv.item_qty as itemQty,
	                            inv.item_price as itemPrice,
	                            inv.mfg_date as mfgDate,
	                            inv.expire_date as expireDate,
	                            inv.uom_id as uomId,
	                            inv.location_id as locationId,
	                            inv.warehouse_id as warehouseId,
	                            uom.name as uomName,
	                            loc.name as locationName,
	                            wh.warehouse_name as warehouseName
                            FROM inventory inv
	                        LEFT JOIN mast_item i ON inv.item_id = i.id
	                        LEFT JOIN mast_uom uom ON inv.uom_id = uom.id
	                        LEFT JOIN mast_location loc ON inv.location_id = loc.id
	                        LEFT JOIN mast_warehouse wh ON inv.warehouse_id = wh.id ";

    public async Task<List<InventoryList>> ListAll(InventoryRequest req)
    {
        try
        {
            string sql = baseSql + " Where inv.is_delete = false ";

            var result = await _connection.QueryAsync<InventoryList>(sql, null, transaction: _transaction);
            return result.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error :{ex.Message}");
        }
    }
    public async Task<List<InventoryList>> List(InventoryRequest req)
    {
        try
        {
            string sql = baseSql + " Where inv.is_delete = false ";
            if (!string.IsNullOrEmpty(req?.itemCode))
            {
                sql += " AND (i.item_code ILIKE @Search OR i.item_name ILIKE @Search)";
            }
            if (req != null && !string.IsNullOrEmpty(req.warehouseId))
            {
                sql += " AND inv.warehouse_id = @warehouseId";
            }
            if (req != null && !string.IsNullOrEmpty(req.locationId))
            {
                sql += " AND inv.location_id = @locationId";
            }
            if (req != null && !string.IsNullOrEmpty(req.lotNo))
            {
                sql += " AND inv.lot_no ILIKE @srcLotno";
            }
            if (req != null && !string.IsNullOrEmpty(req.serialNo))
            {
                sql += " AND inv.lot_no ILIKE @srcSerial";
            }
            sql += " ORDER BY inv.update_date desc,inv.create_date desc";

            var result = await _connection.QueryAsync<InventoryList>(sql, new
            {
                warehouseId = req?.warehouseId,
                Search = $"%{req?.itemCode}%",
                srcLotno = $"{req?.lotNo}",
                serialNo = $"{req?.serialNo}",
            }, transaction: _transaction);
            return result.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error :{ex.Message}");
        }
    }

    public async Task<List<IssueInventoryList>> ListForIssue(IssueInventoryRequest req)
    {
        try
        {
            string sql = @$"
        with inv as ({baseSql}),
        issue as (
        	select 
        	iss.inv_id as invId,
        	iss.item_qty as itemQty,
        	coalesce(iss.picking_qty,0) as pickingQty,
        	coalesce(iss.item_qty,0) - coalesce(iss.picking_qty ,0) as remainQty
        	from doc_trans doc
        	left join doc_issue_detail iss on doc.id  = iss.doc_id 
        	where doc.menu_id = @menuId 
            {(!string.IsNullOrEmpty(req.docId) ? " AND doc.id <> @docId" : "")}
        )

        select 
        	inv.id,
        	inv.itemId,
	        inv.itemCode,
        	inv.ItemName,
        	inv.SerialNo,
        	inv.lotNo,
        	(inv.itemQty - (coalesce(issue.remainQty,0) + coalesce(@currentPicking,0))) as availableQty,
        	inv.itemPrice,
        	inv.mfgDate,
        	inv.expireDate,
        	inv.uomName,
        	inv.locationName,
        	inv.warehouseName
        from inv 
        left outer join issue on inv.id = issue.invId ";


            var result = await _connection.QueryAsync<IssueInventoryList>(sql, new
            {
                req?.menuId,
                req?.currentPicking,
                req?.docId
            }, transaction: _transaction);

            return result.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error :{ex.Message}");
        }
    }


    public async Task<int> GetTotal(InventoryRequest req)
    {
        var sql = "SELECT COUNT(1) FROM inventory inv WHERE (inv.is_delete IS NULL OR inv.is_delete = false)";

        if (!string.IsNullOrEmpty(req?.itemCode))
        {
            sql += " AND (i.item_code ILIKE @Search OR i.item_name ILIKE @Search)";
        }
        if (req != null && !string.IsNullOrEmpty(req.warehouseId))
        {
            sql += " AND inv.warehouse_id = @warehouseId";
        }
        if (req != null && !string.IsNullOrEmpty(req.locationId))
        {
            sql += " AND inv.location_id = @locationId";
        }
        if (req != null && !string.IsNullOrEmpty(req.lotNo))
        {
            sql += " AND inv.lot_no ILIKE @srcLotno";
        }
        if (req != null && !string.IsNullOrEmpty(req.serialNo))
        {
            sql += " AND inv.lot_no ILIKE @srcSerial";
        }

        return await _connection.ExecuteScalarAsync<int>(sql, new
        {
            req?.warehouseId,
            Search = $"%{req?.itemCode}%",
            srcLotno = $"{req?.lotNo}",
            serialNo = $"{req?.serialNo}",
        }, transaction: _transaction);
    }
}
