using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IItemGroupRepository
{
    Task<List<MastItemGroupResponse>> list(MastItemGroupRequest req);
    Task<ActionResult> create(MastItemGroupRequest req);
    Task<ActionResult> SyncItemGroups(MastItemGroupSyncRequest req, string userId);
    Task<List<MastItemGroupResponse>> GetByItemId(string itemId);
    Task<MastItemGroupResponse?> GetByItemIdAndGroupId(string itemId, string groupId);
}

