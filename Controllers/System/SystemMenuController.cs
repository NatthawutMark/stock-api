using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stock_api.Interfaces;

using stock_api.Repositories.Dapper;
using stock_api.Request;
using stock_api.Services;
using static stock_api.Request.SystemMenuRequest;

namespace stock_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SystemMenuController : ControllerBase
{

    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;

    public SystemMenuController(UnitOfWork unitOfWork, ISystemService systemService)
    {
        _unitOfWork = unitOfWork;
        _systemService = systemService;
    }

    [HttpPost("addMenu", Name = "AddMenu")]
    public async Task<ActionResult> AddMenu([FromBody] SystemMenuRequest menu)
    {
        try
        {
            if (menu == null)
            {
                return StatusCode(200, new { message = "Invalid menu data" });
            }

            var newMenu = new SystemMenuRequest
            {
                Id = _systemService.GenGUID(),
                ParentId = menu.ParentId ?? null,
                NameTh = menu.NameTh,
                NameEn = menu.NameEn ?? null,
                CreateBy = menu.CreateBy,
                UpdateBy = menu.UpdateBy,
                UpdateDate = DateTime.Now
            };

            await _unitOfWork.SystemMenus.AddAsync(newMenu);
            await _unitOfWork.CompleteAsync();
            return StatusCode(200, new { message = "Menu added successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { message = "Failed to add menu", error = ex.Message });
        }
    }

    [HttpGet("getMenuByUser", Name = "GetMenuByUser")]
    public async Task<ActionResult> GetMenuByUser([FromQuery] string? userId)
    {
        try
        {
            var menus = await _systemService.GetMenuByUser(userId);
            return StatusCode(200, new { success = true, results = menus, message = "Get menu by user successful" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Failed to get menu", error = ex.Message });
        }
    }

    [HttpGet("getTransactionMenu", Name = "GetTransactionMenu")]
    public async Task<ActionResult> GetTransactionMenu()
    {
        try
        {
            var menus = await _systemService.GetTransactionMenuByUser(_systemService.GetUserId());
            return StatusCode(200, new { success = true, results = menus, message = "Get transaction menu successful" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Failed to get transaction menu", error = ex.Message });
        }
    }
}
