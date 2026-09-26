using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class MastItemWarehouse
{
    public string Id { get; set; } = null!;

    public string? ItemId { get; set; }

    public string? WarehouseId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual MastItem? Item { get; set; }

    public virtual MastWarehouse? Warehouse { get; set; }
}
