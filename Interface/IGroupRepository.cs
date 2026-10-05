using Microsoft.AspNetCore.Mvc;

using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Interfaces;

public interface IGroupRepository
{
    Task<List<MastGroupResponse>> list(MastGroupRequest req);

    Task<ActionResult> create(MastGroupRequest req);
    Task<MastGroupResponse?> GetByName(bool check, string nameTh);
}