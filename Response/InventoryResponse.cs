using System.Runtime.InteropServices;

namespace stock_api.Response;

public class InventoryResponse
{
    public class InventoryList
    {
        public string? id { get; set; }
        public string? itemId { get; set; }
        public string? itemCode { get; set; }
        public string? itemName { get; set; }
        public string? lotNo { get; set; }
        public string? serialNo { get; set; }
        public decimal? itemQty { get; set; }
        public decimal? itemPrice { get; set; }
        public string? locationId { get; set; }
        public string? locationName { get; set; }
        public string? uomId { get; set; }
        public string? uomName { get; set; }
        public DateTime? mfgDate { get; set; }
        public DateTime? expireDate { get; set; }
        public string? warehouseId { get; set; }
        public string? warehouseName { get; set; }
    }

    public class IssueInventoryList
    {
        public string? id { get; set; }
        public string? itemId { get; set; }
        public string? itemCode { get; set; }
        public string? itemName { get; set; }
        public string? lotNo { get; set; }
        public string? serialNo { get; set; }
        public decimal? availableQty { get; set; }
        public decimal? itemPrice { get; set; }
        public string? locationName { get; set; }
        public string? uomName { get; set; }
        public DateTime? mfgDate { get; set; }
        public DateTime? expireDate { get; set; }
        public string? warehouseName { get; set; }
    }
    public class InventoryDmgList
    {
        public string? id { get; set; }
        public string? itemId { get; set; }
        public string? invId { get; set; }
        public string? docId { get; set; }
        public string? docNo { get; set; }
        public string? itemCode { get; set; }
        public string? itemName { get; set; }
        public string? lotNo { get; set; }
        public string? serialNo { get; set; }
        public string? uomId { get; set; }
        public string? uomName { get; set; }
        public decimal? itemQty { get; set; }
        public decimal? itemPrice { get; set; }
        public DateTime? mfgDate { get; set; }
        public DateTime? expireDate { get; set; }
        public DateTime? createAt { get; set; }
        public DateTime? updateAt { get; set; }
        public string? reasonId { get; set; }
        public string? reasonName { get; set; }
        public string? warehouseId { get; set; }
        public string? warehouseName { get; set; }
    }
}
