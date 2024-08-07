using System.ComponentModel.DataAnnotations;

namespace StockLib.Utils
{
    public enum EReportNormId
    {
        [Display(Name = "Doanh thu")]
        DoanhThu = 2206,
        [Display(Name = "Giá vốn")]
        GiaVon = 2207,
        [Display(Name = "Lợi nhuận sau thuế")]
        LNST = 2212,
        [Display(Name = "Nợ phải trả")]
        NoPhaiTra = 2997,
        [Display(Name = "Vốn chủ sở hữu")]
        VonChuSoHuu = 2998,
        [Display(Name = "Tồn kho")]
        TonKho = 3006,
        [Display(Name = "Người mua trả tiền trước")]
        NguoiMuaTraTienTruoc = 3049,

        [Display(Name = "Thu nhập từ lãi")]
        ThuNhapLai = 4399,
        [Display(Name = "Thu nhập từ dịch vụ")]
        ThuNhapTuDichVu = 4397,
        [Display(Name = "Chi phí hoạt động")]
        ChiPhiHD = 4391,
        [Display(Name = "Trích lập dự phòng")]
        TrichLap = 4392,
        [Display(Name = "LNST Ngân Hàng")]
        LNSTNH = 4378
    }
}
