using System.Data;
using back_stock.Interfaces;

namespace back_stock.Repositories.Dapper;

public class DapperUnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;

    public IBrandRepository Brands { get; private set; }
    public IWarehouseRepository Warehouses { get; private set; }
    public DapperUnitOfWork(IDbConnection context)
    {
        _connection = context;
        _connection.Open();
        _transaction = _connection.BeginTransaction();

        Brands = new DapperBrandRepository(_connection, _transaction);
        Warehouses = new DapperWarehouseRepository(_connection, _transaction);
    }

    public async Task<int> CompleteAsync()
    {
        try
        {
            _transaction.Commit();
            return await Task.FromResult(1);
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
    }
}