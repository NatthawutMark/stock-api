using System;

namespace stock_api.Request;

public class SystemMenuRequest
{
    public string Id { get; set; } = null!;
    public string? ParentId { get; set; }
    public string NameTh { get; set; } = null!;
    public string? NameEn { get; set; }
    public string? Url { get; set; }
    public int? OrderNo { get; set; }
    public bool IsActive { get; set; } = true;
    public string? CreateBy { get; set; }
    public string? UpdateBy { get; set; }
    public DateTime? UpdateDate { get; set; }

}
