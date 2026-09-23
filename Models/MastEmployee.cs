using System;
using System.Collections.Generic;

namespace back_stock.Models;

public partial class MastEmployee
{
    public string Id { get; set; } = null!;

    public string? EmpCode { get; set; }

    public string? FName { get; set; }

    public string? LName { get; set; }

    public string? Tel { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }
}
