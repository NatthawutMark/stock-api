using System.Data;
using stock_api.Interfaces;

namespace stock_api.Repositories.Dapper;

public class UnitOfWork : IUnitOfWork
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
    public IUomRepository Uoms { get; private set; }
    public ICustomerRepository Customers { get; private set; }
    public IVendorRepository Vendors { get; private set; }
    public ITransTypeRepository TransTypes { get; private set; }
    public IReasonRepository Reasons { get; private set; }
    public IEmployeeRepository Employees { get; private set; }
    public IStatusRepository Statuses { get; private set; }
    public IDocTypeRepository DocTypes { get; private set; }
    public IItemGroupRepository ItemGroups { get; private set; }
    public IItemWarehouseRepository ItemWarehouses { get; private set; }
    public UnitOfWork(IDbConnection context, ISystemService systemService)
    {
        _connection = context;
        _connection.Open();
        _transaction = _connection.BeginTransaction();

        Brands = new BrandRepository(_connection, _transaction);
        Warehouses = new WarehouseRepository(_connection, _transaction);
        SystemMenus = new SystemMenuRepository(_connection, _transaction);
        Auths = new AuthRepository(_connection, _transaction);
        Items = new ItemRepository(_connection, _transaction);
        Locations = new LocationRepository(_connection, _transaction);
        Groups = new GroupRepository(_connection, _transaction);
        Uoms = new UomRepository(_connection, _transaction);
        Customers = new CustomerRepository(_connection, _transaction);
        Vendors = new VendorRepository(_connection, _transaction);
        TransTypes = new TransTypeRepository(_connection, _transaction);
        Reasons = new ReasonRepository(_connection, _transaction);
        Employees = new EmployeeRepository(_connection, _transaction);
        Statuses = new StatusRepository(_connection, _transaction);
        DocTypes = new DocTypeRepository(_connection, _transaction);
        ItemGroups = new ItemGroupRepository(_connection, _transaction);
        ItemWarehouses = new ItemWarehouseRepository(_connection, _transaction);
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