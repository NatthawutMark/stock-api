using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class Refreshtoken
{
    public string Id { get; set; }

    public string UserId { get; set; } = null!; 

    public string Token { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public bool IsRevoked { get; set; } = false;

    public DateTime CreateDate { get; set; } = DateTime.Now;

    public DateTime? RevokedDate { get; set; }

    public string? ReplacedToken { get; set; }

    public virtual UserProfile? User { get; set; }
}
