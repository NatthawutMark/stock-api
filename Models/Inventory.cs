using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class Inventory
{
    public string Id { get; set; } = null!;

    public string ItemId { get; set; } = null!;

    public string? LotNo { get; set; }

    public string? SerialNo { get; set; }

    public decimal? ItemQty { get; set; }

    public decimal? ItemPrice { get; set; }

    public string? LocationId { get; set; }

    public string? UomId { get; set; }

    public DateOnly? MfgDate { get; set; }

    public DateOnly? ExpireDate { get; set; }

    public string? WarehouseId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual ICollection<DocIssueDetail> DocIssueDetails { get; set; } = new List<DocIssueDetail>();

    public virtual ICollection<DocReturnDetail> DocReturnDetails { get; set; } = new List<DocReturnDetail>();

    public virtual ICollection<DocTransferFromDetail> DocTransferFromDetails { get; set; } = new List<DocTransferFromDetail>();

    public virtual ICollection<DocTransferToDetail> DocTransferToDetails { get; set; } = new List<DocTransferToDetail>();

    public virtual ICollection<InventoryMoveLog> InventoryMoveLogs { get; set; } = new List<InventoryMoveLog>();

    public virtual MastItem Item { get; set; } = null!;

    public virtual MastLocation? Location { get; set; }

    public virtual MastUom? Uom { get; set; }

    public virtual MastWarehouse? Warehouse { get; set; }
}
