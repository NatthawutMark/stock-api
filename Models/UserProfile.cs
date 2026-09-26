using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class UserProfile
{
    public string Id { get; set; } = null!;

    public string? FName { get; set; }

    public string? LName { get; set; }

    public string? Tel { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual SysMenuUser? SysMenuUser { get; set; }

    public virtual ICollection<SysRoleUser> SysRoleUsers { get; set; } = new List<SysRoleUser>();

    public virtual ICollection<UserAuthen> UserAuthens { get; set; } = new List<UserAuthen>();
}
