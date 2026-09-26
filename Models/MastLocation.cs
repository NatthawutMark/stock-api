using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class MastLocation
{
    public string Id { get; set; } = null!;

    public string? Code { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual ICollection<DocReceiveDetail> DocReceiveDetails { get; set; } = new List<DocReceiveDetail>();

    public virtual ICollection<DocTransferToDetail> DocTransferToDetails { get; set; } = new List<DocTransferToDetail>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<InventoryDamageLog> InventoryDamageLogs { get; set; } = new List<InventoryDamageLog>();

    public virtual ICollection<InventoryDamage> InventoryDamages { get; set; } = new List<InventoryDamage>();

    public virtual ICollection<InventoryMoveLog> InventoryMoveLogNewLocations { get; set; } = new List<InventoryMoveLog>();

    public virtual ICollection<InventoryMoveLog> InventoryMoveLogOldLocations { get; set; } = new List<InventoryMoveLog>();

    public virtual ICollection<MastItem> MastItems { get; set; } = new List<MastItem>();
}
