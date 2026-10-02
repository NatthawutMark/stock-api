using System.Runtime.InteropServices;

namespace stock_api.dbo;

public class ItemDbo
{
    public class ItemList
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
}