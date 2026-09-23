using back_stock.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using back_stock.Repositories.Dapper;

namespace back_stock.Controllers;

[ApiController]
[Route("[controller]")]
public class MastWarehouseController : ControllerBase
{
    private readonly DbContexts _context;
    private readonly IDbConnection _dbConnection;
    private readonly DapperUnitOfWork _dpUnitOfWork;
    public MastWarehouseController(DbContexts context, IDbConnection dbConnection, DapperUnitOfWork dpUnitOfWork)
    {
        _context = context;
        _dbConnection = dbConnection;
        _dpUnitOfWork = dpUnitOfWork;
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
