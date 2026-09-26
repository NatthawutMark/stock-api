using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class DocReturnDetail
{
    public string Id { get; set; } = null!;

    public string? DocId { get; set; }

    public string? InvId { get; set; }

    public decimal? ItemQty { get; set; }

    public string? ReasonId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual DocTran? Doc { get; set; }

    public virtual Inventory? Inv { get; set; }

    public virtual MastReason? Reason { get; set; }
}
