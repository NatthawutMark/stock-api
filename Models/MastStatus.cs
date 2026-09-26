using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class MastStatus
{
    public string Id { get; set; } = null!;

    public string TransTypeId { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? NameTh { get; set; }

    public string? NameEn { get; set; }

    public int? OrderNo { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual ICollection<DocTran> DocTrans { get; set; } = new List<DocTran>();

    public virtual MastTransType TransType { get; set; } = null!;
}
