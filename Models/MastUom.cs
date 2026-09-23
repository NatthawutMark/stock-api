using System;
using System.Collections.Generic;

namespace back_stock.Models;

public partial class MastUom
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual ICollection<MastItem> MastItems { get; set; } = new List<MastItem>();
}
