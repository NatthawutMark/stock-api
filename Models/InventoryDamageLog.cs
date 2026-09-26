using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class InventoryDamageLog
{
    public string Id { get; set; } = null!;

    public string InvDmgId { get; set; } = null!;

    public decimal? ItemQty { get; set; }

    public string? MoveLocationId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual InventoryDamage InvDmg { get; set; } = null!;

    public virtual MastLocation? MoveLocation { get; set; }
}
