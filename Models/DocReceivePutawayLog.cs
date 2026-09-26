using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class DocReceivePutawayLog
{
    public string Id { get; set; } = null!;

    public string ItemDetailId { get; set; } = null!;

    public decimal? PutawayQty { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual DocReceiveDetail ItemDetail { get; set; } = null!;
}
