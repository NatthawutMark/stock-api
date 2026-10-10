using System.Runtime.InteropServices;

namespace stock_api.response;

public class MasterResponse
{
    #region MastItem
    public class MastItemResponse
    {
        public string? id { get; set; }
        public string? itemCode { get; set; }
        public string? itemName { get; set; }
        public string? brandId { get; set; }
        public string? brandName { get; set; }
        public int? minAlert { get; set; }
        public int? maxAlert { get; set; }
        public string? uomId { get; set; }
        public string? uomName { get; set; }
        public string? locationId { get; set; }
        public string? locationName { get; set; }
        public decimal? buyPrice { get; set; }
        public decimal? sellPrice { get; set; }
        public bool? isLotno { get; set; }
        public bool? isSerialNo { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
    }
    #endregion

    #region MastBrand
    public class MastBrandResponse
    {
        public string? id { get; set; }
        public string? nameTh { get; set; }
        public string? nameEn { get; set; }
        public bool? isActive { get; set; }
    }
    #endregion

    #region MastLocation
    public class MastLocationResponse
    {
        public string? id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public bool? isActive { get; set; }
    }
    #endregion

    #region MastGroup
    public class MastGroupResponse
    {
        public string? id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public bool? isActive { get; set; }
    }
    #endregion

    #region MastWarehouse
    public class MastWarehouseResponse
    {
        public string? id { get; set; }
        public string? code { get; set; }
        public string? warehouseName { get; set; }
        public string? description { get; set; }

        public bool? isActive { get; set; }
    }
    #endregion

    #region MastUom
    public class MastUomResponse
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public bool? isActive { get; set; }
    }
    #endregion

    #region MastCustomer
    public class MastCustomerResponse
    {
        public string? id { get; set; }
        public string? custCode { get; set; }
        public string? custName { get; set; }
        public string? contactName { get; set; }
        public string? tel { get; set; }
        public string? address { get; set; }
        public string? remark { get; set; }
        public bool? isActive { get; set; }
    }
    #endregion

    #region MastVendor
    public class MastVendorResponse
    {
        public string? id { get; set; }
        public string? vendCode { get; set; }
        public string? vendName { get; set; }
        public string? contactName { get; set; }
        public string? tel { get; set; }
        public string? address { get; set; }
        public string? remark { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
    }
    #endregion

    #region MastTransType
    public class MastTransTypeResponse
    {
        public string? id { get; set; }
        public string? menuId { get; set; }
        public string? menuName { get; set; }
        public string? nameTh { get; set; }
        public string? nameEn { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
    }
    #endregion

    #region MastReason
    public class MastReasonResponse
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
    }
    #endregion

    #region MastEmployee
    public class MastEmployeeResponse
    {
        public string? id { get; set; }
        public string? empCode { get; set; }
        public string? fName { get; set; }
        public string? lName { get; set; }
        public string? tel { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
    }
    #endregion

    #region MastStatus
    public class MastStatusResponse
    {
        public string? id { get; set; }
        public string? transTypeId { get; set; }
        public string? code { get; set; }
        public string? nameTh { get; set; }
        public string? nameEn { get; set; }
        public int? orderNo { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
    }
    #endregion

    #region MastDocType
    public class MastDocTypeResponse
    {
        public string? id { get; set; }
        public string? menuId { get; set; }
        public string? menuName { get; set; }
        public string? docTypeName { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
    }
    #endregion
    #region MastItemGroup
    public class MastItemGroupResponse
    {
        public string? id { get; set; }
        public string? itemId { get; set; }
        public string? groupId { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
    }
    #endregion

    #region MastItemWarehouse
    public class MastItemWarehouseResponse
    {
        public string? id { get; set; }
        public string? itemId { get; set; }
        public string? warehouseId { get; set; }
        public bool? isActive { get; set; }
        public bool? isDelete { get; set; }
    }
    #endregion
}
