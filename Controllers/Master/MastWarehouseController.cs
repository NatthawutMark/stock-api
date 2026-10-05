
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using stock_api.Repositories.Dapper;
using stock_api.Request;
using stock_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static stock_api.Request.MasterRequest;

namespace stock_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MastWarehouseController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly UnitOfWork _dpUnitOfWork;
    private readonly ISystemService _systemService;
    public MastWarehouseController(IDbConnection dbConnection, UnitOfWork dpUnitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _dpUnitOfWork = dpUnitOfWork;
        _systemService = systemService;
    }

    [HttpPost("create", Name = "CreatMastWarehouse")]
    public async Task<ActionResult> Create([FromBody] MastWarehouseRequest req)
    {
        try
        {
            if (req == null)
            {
                return BadRequest(new { status = false, message = "Invalid request data" });
            }

            // Check if the warehouse code already exists
            var existingWarehouse = await _dpUnitOfWork.Warehouses.GetByCodeAsync(req.Code);
            if (existingWarehouse != null)
            {
                return Conflict(new { status = false, message = "Warehouse code already exists" });
            }

            var mastWarehouse = new MastWarehouseRequest
            {
                id = _systemService.GenGUID(),
                Code = req.Code,
                WarehouseName = req.WarehouseName,
                description = req.description ?? null,
                createBy = req.createBy ?? null,
                UpdateBy = req.UpdateBy ?? null,
            };

            await _dpUnitOfWork.Warehouses.AddAsync(mastWarehouse);
            await _dpUnitOfWork.CompleteAsync();
            return StatusCode(201, new { success = true, data = mastWarehouse, message = "MastWarehouse created successfully" });
        }
        catch (Exception ex)
        {
            _dpUnitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastWarehouse", error = ex.Message });
        }
    }

    [HttpGet("list", Name = "GetAllMastWarehouse")]
    public async Task<ActionResult> Get()
    {
        var mastWarehouses = await _dpUnitOfWork.Warehouses.GetAllAsync();
        return StatusCode(200, mastWarehouses);
    }

    [HttpGet("{id}", Name = "GETMastWarehouse")]
    public async Task<ActionResult> Get(string id)
    {
        var mastWarehouses = await _dpUnitOfWork.Warehouses.GetByIdAsync(id);
        return StatusCode(200, mastWarehouses);
    }


}
