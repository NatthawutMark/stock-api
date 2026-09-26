namespace stock_api.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBrandRepository Brands { get; }
    IWarehouseRepository Warehouses { get; }
    ISystemMenuRepository SystemMenus { get; }
    IAuthRepository Auths { get; }

    // สามารถเพิ่ม Repository อื่นๆ ตรงนี้ได้
    Task<int> CompleteAsync();
}