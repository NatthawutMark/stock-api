using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class DocBookingDetail
{
    public string Id { get; set; } = null!;

    public string DocId { get; set; } = null!;

    public string ItemId { get; set; } = null!;

    public string? ItemName { get; set; }

    public decimal? ItemQty { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual DocTran Doc { get; set; } = null!;

    public virtual ICollection<DocBookingLog> DocBookingLogs { get; set; } = new List<DocBookingLog>();

    public virtual MastItem Item { get; set; } = null!;
}
