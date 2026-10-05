using stock_api.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using stock_api.Repositories.Dapper;
using stock_api.Request;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static stock_api.Request.MasterRequest;
using static stock_api.response.MasterResponse;

namespace stock_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MastItemController : ControllerBase
{
    private readonly DbContexts _context;
    private readonly IDbConnection _dbConnection;
    private readonly DapperUnitOfWork _dpUnitOfWork;
    private readonly ISystemService _systemService;


    public MastItemController(DbContexts context, IDbConnection dbConnection, DapperUnitOfWork dpUnitOfWork, ISystemService systemService)
    {
        _context = context;
        _dbConnection = dbConnection;
        _dpUnitOfWork = dpUnitOfWork;
        _systemService = systemService;
    }

    [HttpPost("list", Name = "ListMastItem")]
    public async Task<ActionResult> Get(MastItemRequest req)
    {
        try
        {
            req.isActive = true;
            req.isDelete = false;

            List<MastItemResponse> res = await _dpUnitOfWork.Items.GetAll(req);

            return StatusCode(200, new { success = true, results = res.ToList(), message = "", error = "" });
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { success = true, results = "", message = ex.Message, error = ex.InnerException?.Message });
        }
    }

    // [HttpPost("create", Name = "CreatMastItem")]
    // public async Task<ActionResult> Create([FromBody] MastWarehouseRequest.reqFields req)
    // {
    //     try
    //     {
    //         if (req == null)
    //         {
    //             return BadRequest(new { status = false, message = "Invalid request data" });
    //         }

    //         // Check if the warehouse code already exists
    //         var existingWarehouse = await _dpUnitOfWork.Warehouses.GetByCodeAsync(req.Code);
    //         if (existingWarehouse != null)
    //         {
    //             return Conflict(new { status = false, message = "Warehouse code already exists" });
    //         }

    //         var mastWarehouse = new MastWarehouse
    //         {
    //             Id = _systemService.GenGUID(),
    //             Code = req.Code,
    //             WarehouseName = req.WarehouseName,
    //             Description = req.description ?? null,
    //             CreateBy = req.createBy ?? null,
    //             UpdateBy = req.UpdateBy ?? null,
    //             UpdateDate = DateTime.Now,
    //         };

    //         await _dpUnitOfWork.Warehouses.AddAsync(mastWarehouse);
    //         await _dpUnitOfWork.CompleteAsync();
    //         return StatusCode(201, new { success = true, data = mastWarehouse, message = "MastWarehouse created successfully" });
    //     }
    //     catch (Exception ex)
    //     {
    //         _dpUnitOfWork.Dispose();
    //         return StatusCode(500, new { status = false, message = "An error occurred while creating the MastWarehouse", error = ex.Message });
    //     }
    // }



}
