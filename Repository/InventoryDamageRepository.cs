using Dapper;
using System.Data;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using stock_api.Request;
using static stock_api.Response.InventoryResponse;
using static stock_api.Request.InventoryRequest;

namespace stock_api.Repositories;

public class InventoryDamageRepository : IInventoryDamageRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public InventoryDamageRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    const string baseSql = @"select idmg.id ,
                            idmg.item_id as itemId,
                            idmg.inv_id as invId,
                            idmg.doc_id as docId,
                            trans.doc_no as docNo,
                            idmg.item_code as itemCode,
                            idmg.item_name as itemName,
                            idmg.lot_no as lotNo,
                            idmg.serial_no as serialNo,
                            idmg.uom_id as uomId,
                            uom.name as uomName,
                            idmg.item_qty as itemQty,
                            idmg.item_price as itemPrice,
                            idmg.mfg_date as mfgDate,
                            idmg.expire_date as expireDate,
                            idmg.create_at as createAt,
                            idmg.update_at as updateAt,
                            idmg.reason_id as reasonId,
                            rea.name as reasonName,
                            idmg.warehouse_id as warehouseId,
                            wh.warehouse_name as warehouseName
                            from 
                            inventory_damage idmg
                            left join doc_trans trans on idmg.doc_id = trans.id
                            left join mast_uom uom on idmg.uom_id  = uom.id
                            left join mast_reason rea on idmg.reason_id = rea.id
                            left join mast_warehouse wh on idmg.warehouse_id  = wh.id ";

    public async Task<List<InventoryDmgList>> ListAll(InventoryDmgRequest req)
    {
        try
        {
            string sql = baseSql + " Where idmg.is_delete = false ";

            var result = await _connection.QueryAsync<InventoryDmgList>(sql, null, transaction: _transaction);
            return result.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error :{ex.Message}");
        }
    }
    public async Task<List<InventoryDmgList>> List(InventoryDmgRequest req)
    {
        try
        {
            var (whereSql, parameters) = BuildFilter(req);
            string sql = $"{baseSql} {whereSql} ORDER BY idmg.update_at desc, idmg.create_at desc";

            var result = await _connection.QueryAsync<InventoryDmgList>(sql, parameters, transaction: _transaction);
            return result.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error :{ex.Message}");
        }
    }

    public async Task<int> GetTotal(InventoryDmgRequest req)
    {
        var (whereSql, parameters) = BuildFilter(req);
        var sql = $@"SELECT COUNT(1)
                     FROM inventory_damage idmg
                     LEFT JOIN mast_item i ON idmg.item_id = i.id
                     {whereSql}";

        return await _connection.ExecuteScalarAsync<int>(sql, parameters, transaction: _transaction);
    }

    private static (string whereSql, object parameters) BuildFilter(InventoryDmgRequest? req)
    {
        var whereSql = "WHERE (idmg.is_delete IS NULL OR idmg.is_delete = false)";

        if (!string.IsNullOrEmpty(req?.itemCode))
        {
            whereSql += " AND (i.item_code ILIKE @Search OR i.item_name ILIKE @Search)";
        }
        if (!string.IsNullOrEmpty(req?.warehouseId))
        {
            whereSql += " AND idmg.warehouse_id = @warehouseId";
        }
        if (!string.IsNullOrEmpty(req?.locationId))
        {
            whereSql += " AND idmg.location_id = @locationId";
        }
        if (!string.IsNullOrEmpty(req?.lotNo))
        {
            whereSql += " AND idmg.lot_no ILIKE @srcLotno";
        }
        if (!string.IsNullOrEmpty(req?.serialNo))
        {
            whereSql += " AND idmg.serial_no ILIKE @srcSerial";
        }

        var parameters = new
        {
            warehouseId = req?.warehouseId,
            locationId = req?.locationId,
            Search = $"%{req?.itemCode}%",
            srcLotno = $"%{req?.lotNo}%",
            srcSerial = $"%{req?.serialNo}%",
        };

        return (whereSql, parameters);
    }
}
