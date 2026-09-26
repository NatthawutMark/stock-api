using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class SysMenu
{
    public string Id { get; set; } = null!;

    public string? ParentId { get; set; }

    public string? NameTh { get; set; }

    public string? NameEn { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual ICollection<SysMenu> InverseParent { get; set; } = new List<SysMenu>();

    public virtual SysMenu? Parent { get; set; }

    public virtual SysMenuUser? SysMenuUser { get; set; }
}
