using System;

namespace stock_api.Request;

public class SystemMenuRequest
{
    public class reqFields
    {
        public string? parentId { get; set; }
        public string nameTh { get; set; }
        public string? nameEN { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }
}
