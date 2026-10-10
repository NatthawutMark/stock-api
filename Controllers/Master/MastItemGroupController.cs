using Microsoft.AspNetCore.Mvc;
using System.Data;
using stock_api.Repositories.Dapper;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MastItemGroupController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;

    public MastItemGroupController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastItemGroup")]
    public async Task<ActionResult<List<MastItemGroupResponse>>> Get(MastItemGroupRequest req)
    {
        try
        {
            req.isActive = true;
            req.isDelete = false;

            List<MastItemGroupResponse> res = await _unitOfWork.ItemGroups.list(req);

            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = true, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    [HttpPost("create", Name = "CreateMastItemGroup")]
    public async Task<ActionResult> Create([FromBody] MastItemGroupRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            if (string.IsNullOrEmpty(req.itemId) || string.IsNullOrEmpty(req.groupId))
            {
                return return200(false, null, "No data To Create", "itemId or groupId is empty");
            }

            MastItemGroupResponse? checkDuplicate = await _unitOfWork.ItemGroups.GetByItemIdAndGroupId(req.itemId, req.groupId);
            if (checkDuplicate != null)
            {
                return return200(false, null, "Duplicate Data", "Item Group mapping already exists");
            }
            string userId = _systemService.GetUserId();

            req.id = _systemService.GenGUID();
            req.createBy = userId;
            req.UpdateBy = userId;

            var result = await _unitOfWork.ItemGroups.create(req);
            if (result is OkObjectResult okResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Create MastItemGroup Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastItemGroup Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Create MastItemGroup Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastItemGroup", error = ex.Message });
        }
    }

    [HttpGet("getByItemIdAndGroupId", Name = "GetMastItemGroupByItemIdAndGroupId")]
    public async Task<ActionResult> GetByItemIdAndGroupId([FromQuery] string itemId, [FromQuery] string groupId)
    {
        try
        {
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(groupId))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            MastItemGroupResponse? res = await _unitOfWork.ItemGroups.GetByItemIdAndGroupId(itemId, groupId);
            if (res != null)
            {
                return return200(true, res, "Get MastItemGroup Success", "");
            }
            else
            {
                return return200(false, null, "MastItemGroup Not Found", "");
            }

        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while fetching the MastItemGroup", error = ex.Message });
        }
    }

    [HttpPost("sync", Name = "SyncMastItemGroup")]
    public async Task<ActionResult> Sync([FromBody] MastItemGroupSyncRequest req)
    {
        try
        {
            if (req == null || string.IsNullOrEmpty(req.itemId))
            {
                return BadRequest(new { status = false, message = "Invalid request data or missing itemId" });
            }

            string userId = _systemService.GetUserId();

            var result = await _unitOfWork.ItemGroups.SyncItemGroups(req, userId);
            if (result is OkObjectResult okResult)
            {
                await _unitOfWork.CompleteAsync();
                return return200(true, null, "Sync MastItemGroup Success", "");
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Sync MastItemGroup Failed", badRequestResult.Value?.ToString() ?? "");
            }
            else
            {
                _unitOfWork.Dispose();
                return return200(false, null, "Sync MastItemGroup Failed", "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while syncing the MastItemGroup", error = ex.Message });
        }
    }

    [HttpGet("getByItemId", Name = "GetMastItemGroupByItemId")]
    public async Task<ActionResult<List<MastItemGroupResponse>>> GetByItemId([FromQuery] string itemId)
    {
        try
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            var res = await _unitOfWork.ItemGroups.GetByItemId(itemId);
            return StatusCode(200, new { success = true, results = res, message = "", error = "" });
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while fetching the MastItemGroup", error = ex.Message });
        }
    }

    #region Private Methods
    [NonAction]
    public ObjectResult return200(bool success = true, object results = null, string message = "", string error = "")
    {
        return StatusCode(200, new { success = success, results = results, message = message, error = error });
    }
    #endregion
}

