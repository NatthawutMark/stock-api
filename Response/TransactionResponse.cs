using System;
using System.Collections.Generic;

namespace stock_api.Response;

public static class TransactionResponse
{
    public class DocTransResponse
    {
        public string id { get; set; } = null!;
        public string menuId { get; set; } = null!;
        public string? menuName { get; set; }
        public string statusCode { get; set; } = null!;
        public string? statusNameTh { get; set; }
        public string? statusNameEn { get; set; }
        public string docTypeId { get; set; } = null!;
        public string? docTypeName { get; set; }
        public string? docNo { get; set; }
        public DateTime? docDate { get; set; }
        public string? refDocNo { get; set; }
        public string? empId { get; set; }
        public string? empName { get; set; }
        public string? vendId { get; set; }
        public string? vendName { get; set; }
        public string? custId { get; set; }
        public string? custName { get; set; }
        public string? warehouseId { get; set; }
        public string? warehouseName { get; set; }
        public string? bookingId { get; set; }
        public string? docIssueReturnId { get; set; }
        public string? transferFromWarehouseId { get; set; }
        public string? transferFromWarehouseName { get; set; }
        public string? transferToWarehouseId { get; set; }
        public string? transferToWarehouseName { get; set; }
        public string? remark { get; set; }
        public bool isQualityControl { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
        public DateTime createDate { get; set; }
        public string? createBy { get; set; }
        public DateTime updateDate { get; set; }
        public string? updateBy { get; set; }

        public List<DocTransLogResponse>? logs { get; set; }
    }

    public class DocTransLogResponse
    {
        public string id { get; set; } = null!;
        public string docId { get; set; } = null!;
        public string? fromStatusId { get; set; }
        public string? fromStatusName { get; set; }
        public string? toStatusId { get; set; }
        public string? toStatusName { get; set; }
        public string? fromUserId { get; set; }
        public string? toUserId { get; set; }
        public string? remark { get; set; }
        public DateTime createDate { get; set; }
        public string? createBy { get; set; }
    }
}

