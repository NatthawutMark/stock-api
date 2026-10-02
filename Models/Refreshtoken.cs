using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class Refreshtoken
{
    public string Id { get; set; } = null!;

    public string? UserId { get; set; }

    public string? Token { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? RevokedDate { get; set; }

    public string? ReplacedToken { get; set; }

    public virtual UserProfile? User { get; set; }
}
