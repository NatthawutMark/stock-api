
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
    private readonly UnitOfWork _unitOfWork;
    private readonly ISystemService _systemService;
    public MastWarehouseController(IDbConnection dbConnection, UnitOfWork unitOfWork, ISystemService systemService)
    {
        _dbConnection = dbConnection;
        _unitOfWork = unitOfWork;
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
            var existingWarehouse = await _unitOfWork.Warehouses.GetByCodeAsync(req.Code);
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

            await _unitOfWork.Warehouses.AddAsync(mastWarehouse);
            await _unitOfWork.CompleteAsync();
            return StatusCode(201, new { success = true, data = mastWarehouse, message = "MastWarehouse created successfully" });
        }
        catch (Exception ex)
        {
            _unitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastWarehouse", error = ex.Message });
        }
    }

    [HttpGet("list", Name = "GetAllMastWarehouse")]
    public async Task<ActionResult> Get()
    {
        var mastWarehouses = await _unitOfWork.Warehouses.GetAllAsync();
        return StatusCode(200, mastWarehouses);
    }

    [HttpGet("{id}", Name = "GETMastWarehouse")]
    public async Task<ActionResult> Get(string id)
    {
        var mastWarehouses = await _unitOfWork.Warehouses.GetByIdAsync(id);
        return StatusCode(200, mastWarehouses);
    }


}
