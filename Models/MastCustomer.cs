using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class MastCustomer
{
    public string Id { get; set; } = null!;

    public string? CustCode { get; set; }

    public string? CustName { get; set; }

    public string? ContactName { get; set; }

    public string? Tel { get; set; }

    public string? Address { get; set; }

    public string? Remark { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual ICollection<DocTran> DocTrans { get; set; } = new List<DocTran>();
}
