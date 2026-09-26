using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class InventoryDamage
{
    public string Id { get; set; } = null!;

    public string DocId { get; set; } = null!;

    public string InvId { get; set; } = null!;

    public decimal? ItemQty { get; set; }

    public string? LocationId { get; set; }

    public string? UomId { get; set; }

    public string? ReasonId { get; set; }

    public string? WarehouseId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual DocTran Doc { get; set; } = null!;

    public virtual ICollection<DocDisposalDetail> DocDisposalDetails { get; set; } = new List<DocDisposalDetail>();

    public virtual MastItem Inv { get; set; } = null!;

    public virtual ICollection<InventoryDamageLog> InventoryDamageLogs { get; set; } = new List<InventoryDamageLog>();

    public virtual MastLocation? Location { get; set; }

    public virtual MastReason? Reason { get; set; }

    public virtual MastUom? Uom { get; set; }

    public virtual MastWarehouse? Warehouse { get; set; }
}
