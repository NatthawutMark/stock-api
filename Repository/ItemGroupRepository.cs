using Dapper;
using System.Data;
using stock_api.Interfaces;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;
using Microsoft.AspNetCore.Mvc;

namespace stock_api.Repositories.Dapper;

public class ItemGroupRepository : IItemGroupRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public ItemGroupRepository(IDbConnection connection, IDbTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<List<MastItemGroupResponse>> list(MastItemGroupRequest req)
    {
        var sql = @"SELECT * FROM mast_item_group WHERE is_active = @isActive AND is_delete = @isDelete";
        return (await _connection.QueryAsync<MastItemGroupResponse>(sql, req, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> create(MastItemGroupRequest req)
    {
        try
        {
            var sql = @"INSERT INTO mast_item_group(
                        id, 
                        item_id,
                        group_id,
                        create_by, 
                        update_by) VALUES (@id, @itemId, @groupId, @createBy, @UpdateBy)";
            return await _connection.ExecuteAsync(sql, req, transaction: _transaction) > 0
                ? new OkObjectResult(new { success = true, results = "", message = "Create MastItemGroup Success", error = "" })
                : new BadRequestObjectResult(new { success = false, results = "", message = "Create MastItemGroup Failed", error = "" });

        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    public async Task<MastItemGroupResponse?> GetByItemIdAndGroupId(string itemId, string groupId)
    {
        var sql = "SELECT * FROM mast_item_group WHERE item_id = @itemId AND group_id = @groupId AND is_active = true AND is_delete = false";
        return await _connection.QueryFirstOrDefaultAsync<MastItemGroupResponse>(sql, new { itemId = itemId.Trim(), groupId = groupId.Trim() }, transaction: _transaction);
    }

    public async Task<List<MastItemGroupResponse>> GetByItemId(string itemId)
    {
        var sql = "SELECT * FROM mast_item_group WHERE item_id = @itemId AND is_active = true AND is_delete = false";
        return (await _connection.QueryAsync<MastItemGroupResponse>(sql, new { itemId = itemId.Trim() }, transaction: _transaction)).ToList();
    }

    public async Task<ActionResult> SyncItemGroups(MastItemGroupSyncRequest req, string userId)
    {
        try
        {
            var deleteSql = "DELETE FROM mast_item_group WHERE item_id = @itemId";
            await _connection.ExecuteAsync(deleteSql, new { itemId = req.itemId }, transaction: _transaction);

            if (req.groupIds != null && req.groupIds.Any())
            {
                var insertSql = @"INSERT INTO mast_item_group(id, item_id, group_id, create_by, update_by) 
                                  VALUES (@id, @itemId, @groupId, @createBy, @updateBy)";
                var insertData = req.groupIds.Select(g => new {
                    id = Guid.NewGuid().ToString().ToUpper().Replace("-", ""),
                    itemId = req.itemId,
                    groupId = g,
                    createBy = userId,
                    updateBy = userId
                }).ToList();

                await _connection.ExecuteAsync(insertSql, insertData, transaction: _transaction);
            }
            return new OkObjectResult(new { success = true, results = "", message = "Sync ItemGroup Success", error = "" });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { success = false, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }
}

