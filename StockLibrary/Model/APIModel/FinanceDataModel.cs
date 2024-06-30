using System.Collections.Generic;

namespace StockLibrary.Model.APIModel
{
    public class FinanceModel
    {
        public List<FinanceDataModel> data { get; set; }
        public string code { get; set; }
    }
    public class FinanceDataModel
    {
        public decimal currentRatio { get; set; }
        public decimal debtAsset { get; set; }
        public decimal debtEquity { get; set; }
        public decimal dilutedEPS { get; set; }
        public decimal dilutedPe { get; set; }
        public decimal eps { get; set; }
        public decimal grossProfitMargin { get; set; }
        public decimal lengthReport { get; set; }
        public decimal netProfitMargin { get; set; }
        public decimal pb { get; set; }
        public decimal pe { get; set; }
        public decimal profit { get; set; }
        public decimal quickRatio { get; set; }
        public decimal revenue { get; set; }
        public decimal roa { get; set; }
        public decimal roe { get; set; }
        public decimal roic { get; set; }
        public decimal yearReport { get; set; }
    }
}
