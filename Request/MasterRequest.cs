using System;
namespace stock_api.Request;

public class MasterRequest
{
    public class MastItemRequest
    {
        public string? id { get; set; }
        public string? itemCode { get; set; }
        public string? itemName { get; set; }
        public string? location { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
        public string? createBy { get; set; }
        public string? createDate { get; set; }
        public string? UpdateBy { get; set; }
        public string? UpdateDate { get; set; }
    }

    public class MastBrandRequest
    {
        public string? id { get; set; }
        public string? nameTh { get; set; }
        public string? nameEn { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
        public string? createBy { get; set; }
        public string? createDate { get; set; }
        public string? UpdateBy { get; set; }
        public string? UpdateDate { get; set; }
    }

}
