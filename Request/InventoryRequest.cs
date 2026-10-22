
using System;
namespace stock_api.Request;

public class InventoryRequest
{
    public string? id { get; set; }
    public string? itemId { get; set; }
    public string? itemCode { get; set; }
    public string? lotNo { get; set; }
    public string? serialNo { get; set; }
    public string? status { get; set; }
    public string? warehouseId { get; set; }
    public string? locationId { get; set; }
    public bool? isActive { get; set; }
    public bool? isDelete { get; set; }
    public int? pageNumber { get; set; }
    public int? size { get; set; }
    public int? page { get; set; } = 1;
    public int? pageSize { get; set; } = 10;
    public int? limit { get; set; }

    // คำนวณจำนวนแถวที่ต้องข้าม (Offset)
    public int? Offset => (page - 1) * pageSize;

    public class IssueInventoryRequest
    {
        public string? itemId { get; set; }
        public string? menuId { get; set; }
        public string? docId { get; set; }
        public string? warehouseId { get; set; }
        public decimal? currentPicking { get; set; }
        public string? search { get; set; }
    }
}
