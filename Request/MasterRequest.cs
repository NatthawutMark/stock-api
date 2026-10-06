using System;
namespace stock_api.Request;

public class MasterRequest
{
    private string? _name;
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
    public class MastLocationRequest
    {
        public string? id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? createDate { get; set; }
        public string? UpdateBy { get; set; }
        public string? UpdateDate { get; set; }
    }
    public class MastWarehouseRequest
    {
        public string? id { get; set; }
        public string Code { get; set; }
        public string WarehouseName { get; set; }
        public string? description { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }
    public class MastGroupRequest
    {
        private string? _nameTh;
        private string? _nameEn;
        public string? id { get; set; }
        public string? nameTh { get => _nameTh; set => _nameTh = value?.Trim(); }
        public string? nameEn { get => _nameEn; set => _nameEn = value?.Trim(); }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }
    public class MastUomRequest
    {
        private string? _name;
        public string? id { get; set; }
        public string? name { get => _name; set => _name = value?.Trim(); }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }
    public class MastCustomerRequest
    {
        private string? _custCode;
        public string? id { get; set; }
        public string? custCode { get => _custCode; set => _custCode = value?.Trim(); }
        public string? custName { get; set; }
        public string? contactName { get; set; }
        public string? tel { get; set; }
        public string? address { get; set; }
        public string? remark { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }
    public class MastVendorRequest
    {
        private string? _vendorCode;
        public string? id { get; set; }
        public string? vendCode { get => _vendorCode; set => _vendorCode = value?.Trim(); }
        public string? vendName { get; set; }
        public string? contactName { get; set; }
        public string? tel { get; set; }
        public string? address { get; set; }
        public string? remark { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }
    public class MastTransTypeRequest
    {
        private string? _nameTh;
        private string? _nameEn;
        public string? id { get; set; }
        public string? nameTh { get => _nameTh; set => _nameTh = value?.Trim(); }
        public string? nameEn { get => _nameEn; set => _nameEn = value?.Trim(); }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }
    public class MastReasonRequest
    {
        private string? _name;
        public string? id { get; set; }
        public string? name { get => _name; set => _name = value?.Trim(); }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }
}
