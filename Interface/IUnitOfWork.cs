namespace stock_api.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IWarehouseRepository Warehouses { get; }
    ISystemMenuRepository SystemMenus { get; }
    IAuthRepository Auths { get; }
    IItemRepository Items { get; }
    ICustomerRepository Customers { get; }
    IBrandRepository Brands { get; }
    ILocationRepository Locations { get; }
    IGroupRepository Groups { get; }
    IVendorRepository Vendors { get; }
    IUomRepository Uoms { get; }
    ITransTypeRepository TransTypes { get; }
    IReasonRepository Reasons { get; }
    IEmployeeRepository Employees { get; }
    IStatusRepository Statuses { get; }
    IDocTypeRepository DocTypes { get; }
    IItemGroupRepository ItemGroups { get; }
    IItemWarehouseRepository ItemWarehouses { get; }
    // สามารถเพิ่ม Repository อื่นๆ ตรงนี้ได้
    Task<int> CompleteAsync();
}