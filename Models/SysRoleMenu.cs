using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class SysRoleMenu
{
    public string Id { get; set; } = null!;

    public string? UserId { get; set; }

    public string? RoleId { get; set; }

    public bool? CanView { get; set; }

    public bool? CanCreate { get; set; }

    public bool? CanApprove { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual SysRole? Role { get; set; }

    public virtual UserProfile? User { get; set; }
}
