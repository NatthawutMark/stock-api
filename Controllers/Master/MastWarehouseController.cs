using stock_api.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using stock_api.Repositories.Dapper;
using stock_api.Request;
using stock_api.Interfaces;

namespace stock_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MastWarehouseController : ControllerBase
{
    private readonly DbContexts _context;
    private readonly IDbConnection _dbConnection;
    private readonly DapperUnitOfWork _dpUnitOfWork;
    private readonly ISystemService _systemService;
    public MastWarehouseController(DbContexts context, IDbConnection dbConnection, DapperUnitOfWork dpUnitOfWork, ISystemService systemService)
    {
        _context = context;
        _dbConnection = dbConnection;
        _dpUnitOfWork = dpUnitOfWork;
        _systemService = systemService;
    }

    [HttpPost("create", Name = "CreatMastWarehouse")]
    public async Task<ActionResult> Create([FromBody] MastWarehouseRequest.reqFields req)
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

            var mastWarehouse = new MastWarehouse
            {
                Id = _systemService.GenGUID(),
                Code = req.Code,
                WarehouseName = req.WarehouseName,
                Description = req.description ?? null,
                CreateBy = req.createBy ?? null,
                UpdateBy = req.UpdateBy ?? null,
                UpdateDate = DateTime.Now,
            };

            await _dpUnitOfWork.Warehouses.AddAsync(mastWarehouse);
            await _dpUnitOfWork.CompleteAsync();
            return StatusCode(201, new { status = true, data = mastWarehouse, message = "MastWarehouse created successfully" });
        }
        catch (Exception ex)
        {
            _dpUnitOfWork.Dispose();
            return StatusCode(500, new { status = false, message = "An error occurred while creating the MastWarehouse", error = ex.Message });
        }
    }

    [HttpGet("getAll", Name = "GETAllMastWarehouse")]
    public ActionResult<List<MastWarehouse>> Get()
    {
        var mastWarehouses = _context.MastWarehouses.ToList();
        return mastWarehouses;
    }

    [HttpGet("{id}", Name = "GETMastWarehouse")]
    public async Task<ActionResult> Get(string id)
    {
        var mastWarehouses = await _dpUnitOfWork.Warehouses.GetByIdAsync(id);
        return StatusCode(200, mastWarehouses);
    }


}
