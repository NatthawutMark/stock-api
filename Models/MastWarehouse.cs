using System;
using System.Collections.Generic;

namespace back_stock.Models;

public partial class MastWarehouse
{
    public string Id { get; set; } = null!;

    public string? Code { get; set; }

    public string? WarehouseName { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual ICollection<MastItemWarehouse> MastItemWarehouses { get; set; } = new List<MastItemWarehouse>();
}
