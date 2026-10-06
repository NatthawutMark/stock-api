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
        public string? brandName { get; set; }
        public int? minAlert { get; set; }
        public int? maxAlert { get; set; }
        public string? uomName { get; set; }
        public string? locationName { get; set; }
        public decimal? buyPrice { get; set; }
        public decimal? sellPrice { get; set; }
        public bool? isLotno { get; set; }
        public bool? isSerialNo { get; set; }
        public bool? isActive { get; set; }
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

}