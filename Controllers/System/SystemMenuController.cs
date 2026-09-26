using Microsoft.AspNetCore.Mvc;
using stock_api.Interfaces;
using stock_api.Models;
using stock_api.Repositories.Dapper;
using stock_api.Request;
using stock_api.Services;

namespace stock_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemMenuController : ControllerBase
{

    private readonly DapperUnitOfWork _dpUnitOfWork;
    private readonly ISystemService _systemService;

    public SystemMenuController(DapperUnitOfWork dpUnitOfWork, ISystemService systemService)
    {
        _dpUnitOfWork = dpUnitOfWork;
        _systemService = systemService;
    }

    [HttpPost("addMenu", Name = "AddMenu")]
    public async Task<ActionResult> AddMenu([FromBody] SystemMenuRequest.reqFields menu)
    {
        try
        {
            if (menu == null)
            {
                return StatusCode(200, new { message = "Invalid menu data" });
            }

            var newMenu = new SysMenu
            {
                Id = _systemService.GenGUID(),
                ParentId = menu.parentId ?? null,
                NameTh = menu.nameTh,
                NameEn = menu.nameEN ?? null,
                CreateBy = menu.createBy,
                UpdateBy = menu.UpdateBy,
                UpdateDate = DateTime.UtcNow
            };

            await _dpUnitOfWork.SystemMenus.AddAsync(newMenu);
            await _dpUnitOfWork.CompleteAsync();
            return StatusCode(200, new { message = "Menu added successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { message = "Failed to add menu" });
        }
    }
}
