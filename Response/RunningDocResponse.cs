using System;
namespace stock_api.Response;

public class RunningDocResponse
{
    public class ConfigDocNumber
    {
        public string menuId { get; set; }
        public string? docTypeId { get; set; }
        public string prefix { get; set; }
        public int digit { get; set; }
        public string resetPeriod { get; set; }
        public string Pattern { get; set; }
    }
    public class DocNumberRunning
    {
        public string Id { get; set; }
        public string ConfigId { get; set; }
        public int RunningYear { get; set; }
        public int? RunningMonth { get; set; }
        public int CurrentNumber { get; set; }
        public string LastDocNo { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}