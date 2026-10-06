using Microsoft.AspNetCore.Mvc;
using stock_api.Interfaces;

using stock_api.Repositories.Dapper;
using stock_api.Request;
using stock_api.Services;
using static stock_api.Request.SystemMenuRequest;

namespace stock_api.Controllers;

[ApiController]
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
            return StatusCode(200, new { message = "Failed to add menu" });
        }
    }
}
