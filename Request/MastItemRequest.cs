using System;
namespace stock_api.Request;

public class MastItemRequest
{
    public class reqFields
    {
        public string? itemCode { get; set; }
        public string? itemName { get; set; }
        public string? location { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }

}
