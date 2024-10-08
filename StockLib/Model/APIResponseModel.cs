using Newtonsoft.Json;

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

    public class BCTCAPIResponse
    {
        public string FileExt { get; set; }
        public int TotalRow { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
        public string LastUpdate { get; set; }
    }


    public class Money24h_PTKT_LV1Response
    {
        public int status { get; set; }
        public Money24h_PTKT_LV2Response data { get; set; }
    }

    public class Money24h_PTKT_LV2Response
    {
        public List<Money24h_PTKTResponse> data { get; set; }
        public int total_symbol { get; set; }
    }
    public class Money24h_PTKTResponse
    {
        public string symbol { get; set; }
        public decimal match_price { get; set; }//Giá hiện tại
        public decimal basic_price { get; set; }//Giá tham chiếu
        public decimal accumylated_vol { get; set; }//Vol
        public decimal change_vol_percent_5 { get; set; }//Thay đổi so với 5 phiên trước
    }

    public class Money24h_NhomNganhResponse
    {
        public Money24h_NhomNganh_DataResponse data { get; set; }
        public int status { get; set; }
    }

    public class Money24h_NhomNganh_DataResponse
    {
        public List<Money24h_NhomNganh_GroupResponse> groups { get; set; }
        public long last_update { get; set; }
    }

    public class Money24h_NhomNganh_GroupResponse
    {
        public string icb_code { get; set; }
        public string icb_name { get; set; }
        public int icb_level { get; set; }
        public List<Money24h_NhomNganh_GroupResponse> child { get; set; }
        public int total_stock { get; set; }
        public int total_stock_increase { get; set; }
        public int total_stock_nochange { get; set; }
        public int toal_stock_decrease { get; set; }
        public decimal avg_change_percent { get; set; }
        public decimal total_val { get; set; }
        public decimal toal_val_increase { get; set; }
        public decimal total_val_nochange { get; set; }
        public decimal total_val_decrease { get; set; }
    }

    public class Money24h_ForeignResponse
    {
        public int no { get; set; }
        public long d { get; set; }
        public string s { get; set; }
        public decimal sell_qtty { get; set; }
        public decimal sell_val { get; set; }
        public decimal buy_qtty { get; set; }
        public decimal buy_val { get; set; }
        public decimal net_val { get; set; }
        public long t { get; set; }
    }

    public class GOV_HaiQuanResponse
    {
        public List<HaiQuanDetail> arr { get; set; }
    }

    public class HaiQuanDetail
    {
        public string NGAY_SO_BO { get; set; }
        public string TIEU_DE { get; set; }
        public string NGAY_CONG_BO { get; set; }
        public string FILE_SO_BO { get; set; }
    }

    public class SSI_PEResponse
    {
        public List<SSI_PEDetail> data { get; set; }
    }

    public class SSI_PEDetail
    {
        public int yearReport { get; set; }
        public int lengthReport { get; set; }
        public double dilutedEPS { get; set; }
        public double dilutedPe { get; set; }
    }

    public class SSI_ShareResponse
    {
        public SSI_Share data { get; set; }
    }

    public class SSI_Share
    {
        public double sharesOutstanding { get; set; }
    }

    public class Money24h_KeHoach
    {
        public int status { get; set; }
        public List<Money24h_KeHoach_Data> data { get; set; }
    }

    public class Money24h_KeHoach_Data
    {
        public int year { get; set; }
        public double isa3 { get; set; }
        public double isa16 { get; set; }
        public double isa22 { get; set; }
        public List<Money24h_KeHoach_Quarter> quarter { get; set; }
    }

    public class Money24h_KeHoach_Quarter
    {
        public int quarter { get; set; }
        public double isa3_report { get; set; }
        public double isa3_percent { get; set; }
        public double isa16_report { get; set; }
        public double isa16_percent { get; set; }
        public double isa22_report { get; set; }
        public double isa22_percent { get; set; }
    }

    public class VietStock_Forex
    {
        public List<double> c { get; set; }
        public List<double> t { get; set; }
    }

    public class DSC_Main
    {
        public DSC_PageProp pageProps { get; set; }
    }

    public class DSC_PageProp
    {
        public DSC_DataCategory dataCategory { get; set; }
    }

    public class DSC_DataCategory
    {
        public DSC_DataList dataList { get; set; }
    }

    public class DSC_DataList
    {
        public List<DSC_Data> data { get; set; }
    }

    public class DSC_Data
    {
        public int id { get; set; }
        public DSC_Atribute attributes { get; set; }
    }

    public class DSC_Atribute
    {
        public string title { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime publishedAt { get; set; }
        public DateTime public_at { get; set; }
        public string slug { get; set; }
        public DSC_Category category_id { get; set; }
    }

    public class DSC_Category
    {
        public DSC_Data data { get; set; }
    }

    public class VNDirect_Main 
    {
        public List<VNDirect_Data> data { get; set; }
    }

    public class VNDirect_Data
    {
        public string newsId { get; set; }
        public string tagsCode { get; set; }
        public string newsTitle { get; set; }
        public string newsDate { get; set; }
        public string newsTime { get; set; }
    }

    public class MigrateAsset_Main
    {
        public List<MigrateAsset_Data> data { get; set; }
    }

    public class MigrateAsset_Data
    {
        public int id { get; set; }
        public string title { get; set; }
        public string file_path { get; set; }
        public DateTime published_at { get; set; }
        public string stock_related { get; set; }
    }

    public class AGR_Data
    {
        public int ReportID { get; set; }
        public string Symbol { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
    }

    public class VCBS_Main
    {
        public List<VCBS_Data> data { get; set; }
    }

    public class VCBS_Data
    {
        public int id { get; set; }
        public string stockSymbol { get; set; }
        public string name { get; set; }
        public VCBS_Category category { get; set; }
        public DateTime publishedAt { get; set; }
    }

    public class VCBS_Category
    {
        public string code { get; set; }
    }

    public class BCPT_Crawl_Data
    {
        public string id { get; set; }
        public string title { get; set; }
        public DateTime date { get; set; }
    }

    public class MacroMicro_WCI_Main
    {
        public MacroMicro_WCI_Data data { get; set; }
    }

    public class MacroMicro_WCI_Data
    {
        [JsonProperty(PropertyName = "c:44756")]
        public MacroMicro_Close c { get; set; }
    }

    public class MacroMicro_Close
    {
        public List<List<object>> series { get; set; }
    }

    public class Investing_Main
    {
        public List<List<float>> data { get; set; }
    }
}
