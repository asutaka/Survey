using System.Collections.Generic;

namespace SurveyStock.Model.APIModel
{
    public class ShareHolderModel
    {
        public List<ShareHolderDataModel> data { get; set; }
    }
    public class ShareHolderDataModel
    {
        public string symbol { get; set; }
        public string name { get; set; }
        public decimal quantity { get; set; }
        public decimal percentage { get; set; }
        public string publicDate { get; set; }
        public string ownershipTypeCode { get; set; }
        public string type { get; set; }
    }
}
