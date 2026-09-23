using System;
using System.Collections.Generic;

namespace back_stock.Models;

public partial class MastGroup
{
    public string Id { get; set; } = null!;

    public string NameTh { get; set; } = null!;

    public string? NameEn { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual ICollection<MastItemGroup> MastItemGroups { get; set; } = new List<MastItemGroup>();
}
