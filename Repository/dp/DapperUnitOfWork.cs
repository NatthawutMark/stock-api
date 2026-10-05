using System.Data;
using stock_api.Interfaces;

namespace stock_api.Repositories.Dapper;

public class DapperUnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;

    public IBrandRepository Brands { get; private set; }
    public IWarehouseRepository Warehouses { get; private set; }
    public ISystemMenuRepository SystemMenus { get; private set; }
    public IAuthRepository Auths { get; private set; }
    public IItemRepository Items { get; private set; }
    public ILocationRepository Locations { get; private set; }
    public IGroupRepository Groups { get; private set; }
    public DapperUnitOfWork(IDbConnection context, ISystemService systemService)
    {
        _connection = context;
        _connection.Open();
        _transaction = _connection.BeginTransaction();

        Brands = new DapperBrandRepository(_connection, _transaction);
        Warehouses = new DapperWarehouseRepository(_connection, _transaction);
        SystemMenus = new DapperSystemMenuRepository(_connection, _transaction);
        Auths = new DapperAuthRepository(_connection, _transaction);
        Items = new DapperItemRepository(_connection, _transaction);
        Locations = new DapperLocationRepository(_connection, _transaction);
        Groups = new DapperGroupRepository(_connection, _transaction);
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