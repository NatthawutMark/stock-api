using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class SysMenuUser
{
    public string Id { get; set; } = null!;

    public string? UserId { get; set; }

    public string? MenuId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual SysMenu? Menu { get; set; }

    public virtual UserProfile? User { get; set; }
}
