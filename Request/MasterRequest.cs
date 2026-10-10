using System;
namespace stock_api.Request;

public class MasterRequest
{
    private string? _name;
    public class MastItemRequest
    {
        private string? _itemCode;
        public string? id { get; set; }
        public string? itemCode { get => _itemCode; set => _itemCode = value?.Trim(); }
        public string? itemName { get; set; }
        public string? brandId { get; set; }
        public int? minAlert { get; set; }
        public int? maxAlert { get; set; }
        public string? uomId { get; set; }
        public string? locationId { get; set; }
        public decimal? buyPrice { get; set; }
        public decimal? sellPrice { get; set; }
        public bool? isLotno { get; set; }
        public bool? isSerialNo { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? createDate { get; set; }
        public string? UpdateBy { get; set; }
        public string? UpdateDate { get; set; }
        public string? originalItemCode { get; set; }
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
        private string? _code;
        public string? id { get; set; }
        public string? Code { get => _code; set => _code = value?.Trim(); }
        public string? WarehouseName { get; set; }
        public string? description { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
        public string? originalCode { get; set; }
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
        public string? originalCustCode { get; set; }
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
    public class MastEmployeeRequest
    {
        private string? _empCode;
        private string? _fName;
        private string? _lName;
        private string? _tel;
        public string? id { get; set; }
        public string? empCode { get => _empCode; set => _empCode = value?.Trim(); }
        public string? fName { get => _fName; set => _fName = value?.Trim(); }
        public string? lName { get => _lName; set => _lName = value?.Trim(); }
        public string? tel { get => _tel; set => _tel = value?.Trim(); }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }

    public class MastStatusRequest
    {
        private string? _transTypeId;
        private string? _code;
        private string? _nameTh;
        private string? _nameEn;
        public string? id { get; set; }
        public string? transTypeId { get => _transTypeId; set => _transTypeId = value?.Trim(); }
        public string? code { get => _code; set => _code = value?.Trim(); }
        public string? nameTh { get => _nameTh; set => _nameTh = value?.Trim(); }
        public string? nameEn { get => _nameEn; set => _nameEn = value?.Trim(); }
        public int? orderNo { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }

    public class MastDocTypeRequest
    {
        private string? _menuId;
        private string? _name;
        public string? id { get; set; }
        public string? menuId { get; set; }
        public string? menuName { get => _name; set => _name = value?.Trim(); }
        public string? docTypeName { get => _name; set => _name = value?.Trim(); }
        public string? name { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
    public class MastItemGroupRequest
    {
        private string? _itemId;
        private string? _groupId;
        public string? id { get; set; }
        public string? itemId { get => _itemId; set => _itemId = value?.Trim(); }
        public string? groupId { get => _groupId; set => _groupId = value?.Trim(); }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }

    public class MastItemWarehouseRequest
    {
        private string? _itemId;
        private string? _warehouseId;
        public string? id { get; set; }
        public string? itemId { get => _itemId; set => _itemId = value?.Trim(); }
        public string? warehouseId { get => _warehouseId; set => _warehouseId = value?.Trim(); }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
        public string? createBy { get; set; }
        public string? UpdateBy { get; set; }
    }

    public class MastItemGroupSyncRequest
    {
        public string? itemId { get; set; }
        public List<string>? groupIds { get; set; }
    }

    public class MastItemWarehouseSyncRequest
    {
        public string? itemId { get; set; }
        public List<string>? warehouseIds { get; set; }
    }
}
