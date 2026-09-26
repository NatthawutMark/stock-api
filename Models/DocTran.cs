using System;
using System.Collections.Generic;

namespace stock_api.Models;

public partial class DocTran
{
    public string Id { get; set; } = null!;

    public string TransTypeId { get; set; } = null!;

    public string StatusId { get; set; } = null!;

    public string DocTypeId { get; set; } = null!;

    public string? DocNo { get; set; }

    public DateTime? DocDate { get; set; }

    public string? RefDocNo { get; set; }

    public string? EmpId { get; set; }

    public string? VendId { get; set; }

    public string? CustId { get; set; }

    public string? WarehouseId { get; set; }

    public string? BookingId { get; set; }

    public string? DocIssueReturnId { get; set; }

    public string? TransferFromWarehouseId { get; set; }

    public string? TransferToWarehouseId { get; set; }

    public string? StatusFromId { get; set; }

    public string? StatusToId { get; set; }

    public string? Remark { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }

    public virtual MastCustomer? Cust { get; set; }

    public virtual ICollection<DocBookingDetail> DocBookingDetails { get; set; } = new List<DocBookingDetail>();

    public virtual ICollection<DocBookingLog> DocBookingLogs { get; set; } = new List<DocBookingLog>();

    public virtual ICollection<DocDisposalDetail> DocDisposalDetails { get; set; } = new List<DocDisposalDetail>();

    public virtual ICollection<DocIssueDetail> DocIssueDetails { get; set; } = new List<DocIssueDetail>();

    public virtual ICollection<DocReceiveDetail> DocReceiveDetails { get; set; } = new List<DocReceiveDetail>();

    public virtual ICollection<DocReturnDetail> DocReturnDetails { get; set; } = new List<DocReturnDetail>();

    public virtual ICollection<DocTransLog> DocTransLogs { get; set; } = new List<DocTransLog>();

    public virtual ICollection<DocTransferFromDetail> DocTransferFromDetails { get; set; } = new List<DocTransferFromDetail>();

    public virtual ICollection<DocTransferToDetail> DocTransferToDetails { get; set; } = new List<DocTransferToDetail>();

    public virtual MastDocType DocType { get; set; } = null!;

    public virtual MastEmployee? Emp { get; set; }

    public virtual ICollection<InventoryDamage> InventoryDamages { get; set; } = new List<InventoryDamage>();

    public virtual MastStatus Status { get; set; } = null!;

    public virtual MastTransType TransType { get; set; } = null!;

    public virtual MastVendor? Vend { get; set; }

    public virtual MastWarehouse? Warehouse { get; set; }
}
