using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class DocTransLog
{
    public string Id { get; set; } = null!;

    public string DocId { get; set; } = null!;

    public string? FromStatusId { get; set; }

    public string? ToStatusId { get; set; }

    public string? FromUserId { get; set; }

    public string? ToUserId { get; set; }

    public string? Remark { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual DocTran Doc { get; set; } = null!;
}
