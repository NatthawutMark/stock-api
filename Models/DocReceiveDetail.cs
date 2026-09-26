using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class DocReceiveDetail
{
    public string Id { get; set; } = null!;

    public string DocId { get; set; } = null!;

    public string? ItemId { get; set; }

    public string? ItemName { get; set; }

    public string? LotNo { get; set; }

    public string? SerialNo { get; set; }

    public string? LocationId { get; set; }

    public string? UomId { get; set; }

    public DateOnly? MfgDate { get; set; }

    public DateOnly? ExpireDate { get; set; }

    public decimal? ItemQty { get; set; }

    public decimal? PutawayQty { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual DocTran Doc { get; set; } = null!;

    public virtual ICollection<DocReceivePutawayLog> DocReceivePutawayLogs { get; set; } = new List<DocReceivePutawayLog>();

    public virtual MastItem? Item { get; set; }

    public virtual MastLocation? Location { get; set; }

    public virtual MastUom? Uom { get; set; }
}
