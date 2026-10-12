using System;

namespace stock_api.Request;

public static class TransactionRequest
{
    public class DocNumber
    {
        public string? menuId { get; set; }
        public string? docTypeId { get; set; }
    }

    public class DocTransListRequest
    {
        public string? menuId { get; set; }
        public string? docTypeId { get; set; }
        public string? statusCode { get; set; }
        public string? docNo { get; set; }
        public string? refDocNo { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? empId { get; set; }
        public string? vendId { get; set; }
        public string? custId { get; set; }
        public string? warehouseId { get; set; }
        public bool? isActive { get; set; } = true;
        public bool? isDelete { get; set; } = false;
    }

    public class DocTransCreateRequest
    {
        public string? id { get; set; }
        public string? menuId { get; set; } = null!;
        public string? docTypeId { get; set; } = null!;
        public string? statusCode { get; set; } = "DRAFT";
        public string? docNo { get; set; }
        public DateTime? docDate { get; set; } = DateTime.Now;
        public string? refDocNo { get; set; }
        public string? empId { get; set; }
        public string? vendId { get; set; }
        public string? custId { get; set; }
        public string? warehouseId { get; set; }
        public string? bookingId { get; set; }
        public string? docIssueReturnId { get; set; }
        public string? transferFromWarehouseId { get; set; }
        public string? transferToWarehouseId { get; set; }
        public string? remark { get; set; }
        public bool? isQualityControl { get; set; } = false;
        public string? createBy { get; set; }
        public string? updateBy { get; set; }
        public List<DocReceiveDetail> details { get; set; }
    }

    public class DocTransUpdateRequest
    {
        public string id { get; set; } = null!;
        public string? docTypeId { get; set; }
        public DateTime? docDate { get; set; }
        public string? refDocNo { get; set; }
        public string? empId { get; set; }
        public string? vendId { get; set; }
        public string? custId { get; set; }
        public string? warehouseId { get; set; }
        public string? bookingId { get; set; }
        public string? docIssueReturnId { get; set; }
        public string? transferFromWarehouseId { get; set; }
        public string? transferToWarehouseId { get; set; }
        public string? remark { get; set; }
        public bool? isQualityControl { get; set; }
        public bool? isActive { get; set; }
        public string? updateBy { get; set; }
    }

    public class DocTransUpdateStatusRequest
    {
        public string docId { get; set; } = null!;
        public string statusCode { get; set; } = null!;
        public string? remark { get; set; }
        public string? updateBy { get; set; }
    }

    public class DocTransDeleteRequest
    {
        public string id { get; set; } = null!;
    }

    public class DocReceiveDetail
    {
        public string? id { get; set; }
        public string? docId { get; set; }
        public string? itemId { get; set; }
        public string? itemCode { get; set; }
        public string? itemName { get; set; }
        public string? lotNo { get; set; }
        public string? serialNo { get; set; }
        public string? locationId { get; set; }
        public string? locationName { get; set; }
        public string? uomId { get; set; }
        public string? uomName { get; set; }
        public DateTime? mfgDate { get; set; }
        public DateTime? expireDate { get; set; }
        public decimal? itemQty { get; set; }
        public decimal? putawayQty { get; set; }
        public decimal? itemPrice { get; set; }
        public decimal? totalPrice { get; set; }
        public string? trackingCode { get; set; }
        public bool? isActive { get; set; } = true;
        public bool? isDelete { get; set; } = false;
        public string? createBy { get; set; }
        public DateTime? createDate { get; set; }
        public string? updateBy { get; set; }
        public DateTime? updateDate { get; set; }
    }
}

