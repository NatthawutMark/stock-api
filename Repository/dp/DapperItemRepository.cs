using Dapper;
using System.Data;
using stock_api.Interfaces;
using stock_api.Models;
using stock_api.dbo;
using System.ComponentModel;
using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Repositories.Dapper;

public class DapperItemRepository : IItemRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public DapperItemRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastItemResponse>> GetAll(MastItemRequest req)
    {
        try
        {
            var sql = @"select i.id,
                    i.item_code as itemCode ,
                    i.item_name as itemName ,
                    mb.name_th as brandName ,
                    i.min_alter as minAlert ,
                    i.max_alter as maxAlert ,
                    mu.name as uomName ,
                    ml.name as locationName, 
                    COALESCE(i.buy_price,0.00) as buyPrice ,
                    COALESCE(i.sell_price,0.00) as sellPrice ,
                    i.is_lotno as isLotno ,
                    i.is_serialno as isSerialNo,
                    i.is_active as isActive
                    from mast_item i
                    left outer join mast_brand mb on i.brand_id = mb.id
                    inner join mast_location ml on i.location_id = ml.id
                    inner join mast_uom mu on i.uom_id = mu.id
                    where i.is_active = @isActive and i.is_delete = @isDelete";
            var result = await _connection.QueryAsync<MastItemResponse>(sql, req, transaction: _transaction);
            return result.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error :{ex.Message}");
        }
    }

}