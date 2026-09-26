using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class InventoryMoveLog
{
    public string Id { get; set; } = null!;

    public string? InvId { get; set; }

    public string? OldLocationId { get; set; }

    public string? NewLocationId { get; set; }

    public decimal? ItemQty { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual Inventory? Inv { get; set; }

    public virtual MastLocation? NewLocation { get; set; }

    public virtual MastLocation? OldLocation { get; set; }
}
