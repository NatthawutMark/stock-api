using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class UserAuthen
{
    public string Id { get; set; } = null!;

    public string? UserId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Plaintext { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual UserProfile? User { get; set; }
}
