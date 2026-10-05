namespace stock_api.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IWarehouseRepository Warehouses { get; }
    ISystemMenuRepository SystemMenus { get; }
    IAuthRepository Auths { get; }
    IItemRepository Items { get; }

    // สามารถเพิ่ม Repository อื่นๆ ตรงนี้ได้
    Task<int> CompleteAsync();
}