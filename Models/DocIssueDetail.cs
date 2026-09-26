using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class DocIssueDetail
{
    public string Id { get; set; } = null!;

    public string? DocId { get; set; }

    public string? InvId { get; set; }

    public decimal? ItemQty { get; set; }

    public decimal? PickingQty { get; set; }

    public decimal? ReturnQty { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual DocTran? Doc { get; set; }

    public virtual ICollection<DocIssuePickingLog> DocIssuePickingLogs { get; set; } = new List<DocIssuePickingLog>();

    public virtual Inventory? Inv { get; set; }
}
