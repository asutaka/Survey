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
        [Display(Name = "Lợi nhuận gộp")]
        LNGop = 2217,
        [Display(Name = "Lợi nhuận ròng")]
        LNRong = 2208,

        [Display(Name = "Vay và nợ thuê tài chính ngắn hạn")]
        VayNganHan = 3077,
        [Display(Name = "Vay và nợ thuê tài chính dài hạn")]
        VayDaiHan = 3078,

        [Display(Name = "Vốn chủ sở hữu")]
        VonChuSoHuu = 2998,
        [Display(Name = "Tồn kho")]
        TonKho = 3006,
        [Display(Name = "Tồn kho")]
        TonKhoThep = 3027,
        [Display(Name = "Người mua trả tiền trước")]
        NguoiMuaTraTienTruoc = 3049,

       
        [Display(Name = "Tiền gửi tại NHNN")]
        TienGuiNHNN = 4311,
        [Display(Name = "Tiền, vàng gửi tại các TCTD khác")]
        TienGuiTCTD = 4344,
        [Display(Name = "Cho vay các TCTD khác")]
        ChoVayTCTD = 4326,
        [Display(Name = "Chứng khoán kinh doanh")]
        ChungKhoanKD = 4346,
        [Display(Name = "Cho vay khách hàng")]
        ChoVayKH = 4348,
        [Display(Name = "Trích lập dự phòng")]
        TrichLap = 4349,
        [Display(Name = "Chứng khoán đầu tư sẵn sàng để bán")]
        ChungKhoanDauTu = 4350,
        [Display(Name = "Chứng khoán giữ đến đáo hạn")]
        ChungKhoanDaoHan = 4351,

        [Display(Name = "LNST Ngân Hàng")]
        LNSTNH = 4378,
        [Display(Name = "Chi phí lãi và các chi phí tương tự")]
        ChiPhiLai = 4396,
        [Display(Name = "Thu nhập từ dịch vụ")]
        ThuNhapTuDichVu = 4397,
        [Display(Name = "Thu nhập từ lãi")]
        ThuNhapLai = 4399,


        [Display(Name = "Thu nhập lãi thuần")]
        ThuNhapLaiThuan = 4385,

        [Display(Name = "Doanh thu thuần")]
        DoanhThuThuan = 4590,
        [Display(Name = "Lợi nhuận kế toán sau thuế")]
        LNKTST = 4585,
        [Display(Name = "Lãi từ tự doanh FVTPL")]
        LaiFVTPL = 5434,
        [Display(Name = "Lãi từ tự doanh HTM")]
        LaiHTM = 5435,
        [Display(Name = "Lãi từ tự doanh AFS")]
        LaiAFS =5437,
        [Display(Name = "Lãi từ hoạt động cho vay")]
        LaiChoVay =5436,
        [Display(Name = "Doanh thu môi giới")]
        DoanhThuMoiGioi = 4599,

        [Display(Name = "Lỗ từ tự doanh FVTPL")]
        LoFVTPL = 5439,
        [Display(Name = "Lỗ từ tự doanh HTM")]
        LoHTM = 5440,
        [Display(Name = "Lỗ từ tự doanh AFS")]
        LoAFS = 5442,
        [Display(Name = "Chi phí môi giới")]
        ChiPhiMoiGioi = 5445,

        [Display(Name = "Tài sản FVTPL")]
        TaiSanFVTPL = 5372,
        [Display(Name = "Tài sản HTM")]
        TaiSanHTM = 5398,
        [Display(Name = "Tài sản AFS")]
        TaiSanAFS = 5374,
        [Display(Name = "Tài sản cho vay")]
        TaiSanChoVay = 5373,
        [Display(Name = "Vốn chủ sở hữu")]
        VonChuSoHuuCK = 4478
    }

    public enum EFinanceIndex
    {
        [Display(Name = "Tỷ lệ chi phí hoạt động/Tổng thu nhập HĐKD trước dự phòng (CIR)")]
        CIR = 109
    }

    public enum EMoney24hTimeType
    {
        [Display(Name = "today")]
        today = 1,
        [Display(Name = "week")]
        week = 2,
        [Display(Name = "month")]
        month = 3
    }

    public enum EMoney24hExchangeMode
    {
        [Display(Name = "10")]
        HSX = 1,
        [Display(Name = "02")]
        HNX = 2,
        [Display(Name = "03")]
        UPCOM = 3
    }

    public enum EConfigDataType
    {
        TuDoanhHNX = 1,
        TuDoanhUpcom = 2,
        TuDoanhHose = 3,
        GDNN_today = 10,
        GDNN_week = 11,
        GDNN_month = 12,
        ThongKeNhomNganh_today = 20,
        ThongKeNhomNganh_week = 21,
        ThongKeNhomNganh_month = 22,
        ChiBaoKyThuat = 30,
        TongCucThongKe = 40
    }

    public enum EHnxExchange
    {
        NY = 1,
        UP = 2
    }
    /// <summary>
    /// CPI: QoQoY
    /// </summary>
    public enum EKeyTongCucThongKe
    {
        [Display(Name = "Du lịch")]
        DuLich = 1,
        [Display(Name = "Vận tải đường Sắt(nghìn tấn)")]
        VanTai_DuongSat = 2,
        [Display(Name = "Vận tải đường Biển(nghìn tấn)")]
        VanTai_DuongBien = 3,
        [Display(Name = "Vận tải đường Thủy nội địa(nghìn tấn)")]
        VanTai_DuongThuy = 4,
        [Display(Name = "Vận tải đường Bộ(nghìn tấn)")]
        VanTai_DuongBo = 5,
        [Display(Name = "Vận tải Hàng không(nghìn tấn)")]
        VanTai_HangKhong = 6,
        [Display(Name = "Vận tải trong nước(nghìn tấn)")]
        VanTai_TrongNuoc = 7,
        [Display(Name = "Vận tải nước ngoài(nghìn tấn)")]
        VanTai_NuocNgoai = 8,
        [Display(Name = "Chỉ số giá tiêu dùng(%)")]
        CPI_GiaTieuDung = 9,
        [Display(Name = "Giá Vàng(%)")]
        CPI_GiaVang = 10,
        [Display(Name = "USD(%)")]
        CPI_DoLa = 11,
        [Display(Name = "Lạm phát(%)")]
        CPI_LamPhat = 12,
        [Display(Name = "Xuất khẩu từ DN(triệu USD)")]
        XK_TrongNuoc = 13,
        [Display(Name = "Xuất khẩu từ DN FDI(triệu USD)")]
        XK_FDI = 14,
        [Display(Name = "Xuất khẩu thủy sản(triệu USD)")]
        XK_ThuySan = 15,
        [Display(Name = "Xuất khẩu gạo(triệu USD)")]
        XK_Gao = 16,
        [Display(Name = "Xuất khẩu xi măng(triệu USD)")]
        XK_Ximang = 17,
        [Display(Name = "Xuất khẩu hóa chất(triệu USD)")]
        XK_HoaChat = 18,
        [Display(Name = "Xuất khẩu SP hóa chất(triệu USD)")]
        XK_SPHoaChat = 19,
        [Display(Name = "Xuất khẩu SP chất dẻo(triệu USD)")]
        XK_SPChatDeo = 20,
        [Display(Name = "Xuất khẩu cao su(triệu USD)")]
        XK_CaoSu = 21,
        [Display(Name = "Xuất khẩu gỗ(triệu USD)")]
        XK_Go = 22,
        [Display(Name = "Xuất khẩu dệt may(triệu USD)")]
        XK_DetMay = 23,
        [Display(Name = "Xuất khẩu sắt thép(triệu USD)")]
        XK_SatThep = 24,
        [Display(Name = "Nhập khẩu từ DN(triệu USD)")]
        NK_TrongNuoc = 25,
        [Display(Name = "Nhập khẩu từ DN FDI(triệu USD)")]
        NK_FDI = 26,
        [Display(Name = "Nhập khẩu thức ăn gia súc(triệu USD)")]
        NK_ThucAnGiaSuc = 27,
        [Display(Name = "Nhập khẩu phân bón(triệu USD)")]
        NK_PhanBon = 28,
        [Display(Name = "Nhập khẩu vải(triệu USD)")]
        NK_Vai = 29,
        [Display(Name = "Nhập khẩu sắt thép(triệu USD)")]
        NK_SatThep = 30,
        [Display(Name = "Nhập khẩu SP sắt thép(triệu USD)")]
        NK_SPSatThep = 31,
        [Display(Name = "Nhập khẩu ô tô(triệu USD)")]
        NK_Oto = 32,
        [Display(Name = "Bán lẻ(tỷ đồng)")]
        BanLe = 33,
        [Display(Name = "FDI(triệu USD)")]
        FDI = 34,
        [Display(Name = "Đầu tư công(tỷ)")]
        DauTuCong = 35,
        [Display(Name = "Sản phẩm công nghiệp")]
        SP_CongNghiep = 36,
        [Display(Name = "IIP")]
        IIP = 37,
        [Display(Name = "GDP")]
        GDP = 38,
        [Display(Name = "Chăn Nuôi Lợn")]
        ChanNuoiLon = 39,
        [Display(Name = "Giá sản xuất(%)")]
        GiaSX = 40,
        [Display(Name = "Giá vận tải(%)")]
        GiaVT = 41,
        [Display(Name = "Giá xuất khẩu(%)")]
        GiaXK = 42,
        [Display(Name = "Giá nhập khẩu(%)")]
        GiaNK = 43,
    }
}
