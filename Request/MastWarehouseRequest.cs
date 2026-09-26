using System;

namespace stock_api.Request;

public class MastWarehouseRequest
{
    public class reqFields
    {
        public string Code { get; set; }
        public string WarehouseName { get; set; }
        public string? description { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }
}
