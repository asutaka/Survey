namespace StockLib.Model
{
    public class BCTCFileUploadResponse
    {
        public string file { get; set; }
        public long size { get; set; }
        public string ctime { get; set; }
        public string host { get; set; }
        public string name { get; set; }
    }

    public class BCTCOCRResponse
    {
        public string cookieName { get; set; }
        public string cookieValue { get; set; }
        public string jobId { get; set; }
    }

    public class BCTCOCRInput
    {
        public List<BCTCFileUploadResponse> files { get; set; }
        public string langCode { get; set; } = "vie";
        public string outputType { get; set; } = "pdf";
        public string title { get; set; }
        public string author { get; set; }
        public string subject { get; set; }
        public string keywords { get; set; }
        public bool removeBackground { get; set; }
        public bool rotatePages { get; set; }
        public bool deskew { get; set; } = true;
        public bool clean { get; set; }
        public bool forceOcr { get; set; }
        public bool joinFiles { get; set; }
    }

    public class BCTCStatusResponse
    {
        public string cookieName { get; set; }
        public string cookieValue { get; set; }
        public string jobId { get; set; }
        public string status { get; set; }
    }

    public class BaseResponse<T>
    {
        public List<T> data { get; set; }
    }

    public class ReportDataIDResponse : BaseResponse<ReportDataIDDetailResponse> { }

    public class ReportDataIDDetailResponse
    {
        public int RowNumber { get; set; }
        public int ReportDataID { get; set; }
        public int YearPeriod { get; set; }
        public int ReportTermID { get; set; }
        public int Isunited { get; set; }
        public int BasePeriodBegin { get; set; }
    }


    public class ReportTempIDResponse : BaseResponse<ReportTempIDDetailResponse> { }
    public class ReportTempIDDetailResponse
    {
        public int RowNumber { get; set; }
        public string IdTemp { get; set; }
        public int YearPeriod { get; set; }
        public int ReportTermID { get; set; }
    }


    public class ReportDataDetailValue_BCTTResponse : BaseResponse<ReportDataDetailValue_BCTTDetailResponse> { }

    public class ReportDataDetailValue_BCTTDetailResponse
    {
        public int ReportnormId { get; set; }
        public double? Value1 { get; set; }
        public double? Value2 { get; set; }
        public double? Value3 { get; set; }
        public double? Value4 { get; set; }
        public double? Value5 { get; set; }
        public double? Value6 { get; set; }
        public double? Value7 { get; set; }
        public double? Value8 { get; set; }
        public double? Value9 { get; set; }
    }

    public class TempDetailValue_CSTCResponse : BaseResponse<TempDetailValue_CSTCDetailResponse> { }

    public class TempDetailValue_CSTCDetailResponse
    {
        public int FinanceIndexID { get; set; }
        public double? Value1 { get; set; }
        public double? Value2 { get; set; }
        public double? Value3 { get; set; }
        public double? Value4 { get; set; }
        public double? Value5 { get; set; }
        public double? Value6 { get; set; }
        public double? Value7 { get; set; }
        public double? Value8 { get; set; }
        public double? Value9 { get; set; }
    }
}
