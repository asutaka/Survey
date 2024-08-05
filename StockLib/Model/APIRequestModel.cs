namespace StockLib.Model
{
    public class RequestVerificationRequestBody
    {
        public string StockCode { get; set; }
        public string Unit { get; set; }
        public string __RequestVerificationToken { get; set; }
    }
    public class ReportDataDetailValueRequestBody
    {
        public string Index { get; set; }
        public string IsShowData { get; set; }
        public string ReportDataId { get; set; }
        public string RowNumber { get; set; }
        public string SortTimeType { get; set; }
        public string TotalCount { get; set; }
        public string YearPeriod { get; set; }
    }
}
