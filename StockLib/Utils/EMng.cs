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
        TongCucThongKeThang = 41,
        TongCucThongKeQuy = 42,
        TongCucHaiQuan_XK = 43,
        TongCucHaiQuan_NK = 44,
        CurrentTime = 50
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
        None = 0,
        [Display(Name = "Chỉ số giá tiêu dùng(%)")]
        CPI_GiaTieuDung = 1,
        [Display(Name = "Giá Vàng(%)")]
        CPI_GiaVang = 2,
        [Display(Name = "USD(%)")]
        CPI_DoLa = 3,
        [Display(Name = "Lạm phát(%)")]
        CPI_LamPhat = 4,
        [Display(Name = "Bán lẻ(tỷ đồng)")]
        BanLe = 5,
        [Display(Name = "FDI(triệu USD)")]
        FDI = 6,
        [Display(Name = "Đầu tư công(tỷ)")]
        DauTuCong = 7,
        [Display(Name = "IIP - Điện")]
        IIP_Dien = 8,
        [Display(Name = "Hành khách - hàng không")]
        HanhKhach_HangKhong = 9,
        [Display(Name = "Vận tải đường Biển(nghìn tấn)")]
        VanTai_DuongBien = 10,
        [Display(Name = "Vận tải đường Bộ(nghìn tấn)")]
        VanTai_DuongBo = 11,
        [Display(Name = "Vận tải Hàng không(nghìn tấn)")]
        VanTai_HangKhong = 12,
        [Display(Name = "Vận tải trong nước(nghìn tấn)")]
        VanTai_TrongNuoc = 13,
        [Display(Name = "Vận tải nước ngoài(nghìn tấn)")]
        VanTai_NuocNgoai = 14,
        [Display(Name = "Giá VT - Đường biển")]
        GiaVT_Bien = 15,
        [Display(Name = "Giá VT - Hàng không")]
        GiaVT_HangKhong = 16,
        [Display(Name = "Giá VT - Kho bãi")]
        GiaVT_KhoBai = 17,
        [Display(Name = "Giá VT - Bưu chính, chuyển phát")]
        GiaVT_BuuChinh = 18,
        [Display(Name = "Giá NVL - giá điện")]
        GiaNVL_Dien = 19,
        [Display(Name = "IIP - Than")]
        IIP_Than = 20,
        [Display(Name = "IIP - Dầu thô")]
        IIP_DauTho = 21,
        [Display(Name = "IIP - Trang phục")]
        IIP_TrangPhuc = 22,
        [Display(Name = "IIP - Chế biến gỗ")]
        IIP_CheBienGo = 23,
        [Display(Name = "IIP - Giường, tủ, bàn ghế")]
        IIP_GiuongTuBanGhe = 24,
        [Display(Name = "IIP - Hóa chất")]
        IIP_HoaChat = 25,
        [Display(Name = "IIP - Thuốc")]
        IIP_Thuoc = 26,
        [Display(Name = "IIP - Nhựa, cao su")]
        IIP_CaoSuNhua = 27,
        [Display(Name = "IIP - Thiết bị điện")]
        IIP_ThietBiDien = 28,
        [Display(Name = "SPCN - Thủy sản")]
        SPCN_ThuySan = 29,
        [Display(Name = "SPCN - Đường")]
        SPCN_Duong = 30,
        [Display(Name = "SPCN - Điện")]
        SPCN_Bia = 31,
        [Display(Name = "SPCN - Ure")]
        SPCN_Ure = 32,
        [Display(Name = "SPCN - NPK")]
        SPCN_NPK = 33,
        [Display(Name = "SPCN - Xi măng")]
        SPCN_Ximang = 34,
        [Display(Name = "SPCN - Thép cán")]
        SPCN_ThepCan = 35,
        [Display(Name = "SPCN - Thép thanh")]
        SPCN_ThepThanh = 36
    }

    public enum EHaiQuan
    {
        [Display(Name = "Thủy sản")]
        ThuySan = 1,
        [Display(Name = "Sắt thép")]
        SatThep = 2,
        [Display(Name = "Sản phẩm sắt thép")]
        SPSatThep = 3,
        [Display(Name = "Dệt may")]
        DetMay = 4,
        [Display(Name = "Cao su")]
        CaoSu = 5,
        [Display(Name = "Xi măng")]
        Ximang = 6,
        [Display(Name = "Gỗ")]
        Go = 7,
        [Display(Name = "Phân bón")]
        PhanBon = 8,
        [Display(Name = "Gạo")]
        Gao = 9,
        [Display(Name = "Than")]
        Than = 10,
        [Display(Name = "Dầu thô")]
        DauTho = 11,
        [Display(Name = "Xăng dầu")]
        XangDau = 12,
        [Display(Name = "Hóa chất")]
        HoaChat = 13,
        [Display(Name = "Sản phẩm hóa chất")]
        SPHoaChat = 14,
        [Display(Name = "Chất dẻo")]
        ChatDeo = 15,
        [Display(Name = "Sản phẩm chất dẻo")]
        SPChatDeo = 16,
        [Display(Name = "Dây điện")]
        DayDien = 17,

        [Display(Name = "Sản phẩm sắt thép")]
        SPSatThep_NK = 18,
        [Display(Name = "Ô tô dưới 9 chỗ")]
        Oto9Cho_NK = 19,
        [Display(Name = "Ô tô vận tải")]
        OtoVanTai_NK =20,
    }

    public enum EValuation
    {
        [Display(Name = "Định giá P/E")]
        PE = 1,
    }

    public enum EPoint
    {
        Unknown = -1,
        [Display(Name = "Rất tiêu cực")]
        VeryNegative = 1,
        [Display(Name = "Tiêu cực")]
        Negative = 3,
        [Display(Name = "Trung tính")]
        Normal = 5,
        [Display(Name = "Tích cực")]
        Positive = 7,
        [Display(Name = "Rất tích cực")]
        VeryPositive = 9
    }

    public enum EForex
    {
        [Display(Name = "Dầu thô")]
        CL = 1,
        [Display(Name = "DXY - chỉ số USD")]
        DXU1 = 2
    }
}
