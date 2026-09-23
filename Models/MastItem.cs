using System;
using System.Collections.Generic;

namespace back_stock.Models;

public partial class MastItem
{
    public string Id { get; set; } = null!;

    public string ItemCode { get; set; } = null!;

    public string? ItemName { get; set; }

    public string? BrandId { get; set; }

    public int? MinAlter { get; set; }

    public int? MaxAlter { get; set; }

    public string? UomId { get; set; }

    public string LocationId { get; set; } = null!;

    public decimal? BuyPrice { get; set; }

    public decimal? SellPrice { get; set; }

    public bool? IsLotno { get; set; }

    public bool? IsSerialno { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual MastBrand? Brand { get; set; }

    public virtual MastLocation Location { get; set; } = null!;

    public virtual ICollection<MastItemGroup> MastItemGroups { get; set; } = new List<MastItemGroup>();

    public virtual ICollection<MastItemWarehouse> MastItemWarehouses { get; set; } = new List<MastItemWarehouse>();

    public virtual MastUom? Uom { get; set; }
}
