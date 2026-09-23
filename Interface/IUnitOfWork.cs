namespace back_stock.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBrandRepository Brands { get; }
    IWarehouseRepository Warehouses { get; }
    // สามารถเพิ่ม Repository อื่นๆ ตรงนี้ได้
    Task<int> CompleteAsync();
}