using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class SysRole
{
    public string Id { get; set; } = null!;

    public string? NameTh { get; set; }

    public string? NameEn { get; set; }

    public bool? IsAdmin { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual SysRoleUser? SysRoleUser { get; set; }
}
