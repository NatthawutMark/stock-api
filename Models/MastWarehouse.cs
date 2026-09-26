using System;
using System.Collections.Generic;

namespace stock_api.Models;

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

    public virtual ICollection<DocTran> DocTrans { get; set; } = new List<DocTran>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<InventoryDamage> InventoryDamages { get; set; } = new List<InventoryDamage>();

    public virtual ICollection<MastItemWarehouse> MastItemWarehouses { get; set; } = new List<MastItemWarehouse>();
}
