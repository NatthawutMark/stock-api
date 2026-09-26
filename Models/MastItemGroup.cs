using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class MastItemGroup
{
    public string Id { get; set; } = null!;

    public string? ItemId { get; set; }

    public string? GroupId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual MastGroup? Group { get; set; }

    public virtual MastItem? Item { get; set; }
}
