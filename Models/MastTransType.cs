using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class MastTransType
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual ICollection<DocTran> DocTrans { get; set; } = new List<DocTran>();

    public virtual ICollection<MastDocType> MastDocTypes { get; set; } = new List<MastDocType>();

    public virtual ICollection<MastStatus> MastStatuses { get; set; } = new List<MastStatus>();
}
