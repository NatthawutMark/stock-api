using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class MastReason
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual ICollection<DocReturnDetail> DocReturnDetails { get; set; } = new List<DocReturnDetail>();

    public virtual ICollection<InventoryDamage> InventoryDamages { get; set; } = new List<InventoryDamage>();
}
