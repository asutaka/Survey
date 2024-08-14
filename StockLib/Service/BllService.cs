using Microsoft.Extensions.Logging;
using StockLib.DAL;

namespace StockLib.Service
{
    public interface IBllService
    {
        Task SyncBCTC_BatDongSan();
        Task SyncBCTC_NganHang();
        Task<Stream> Chart_NganHang_DoanhThu_LoiNhuan(IEnumerable<string> lInput);
        Task<Stream> Chart_NganHang_TangTruongTinDung_RoomTinDung(IEnumerable<string> lInput);
        Task<Stream> Chart_NganHang_NoXau(IEnumerable<string> lInput);
        Task<Stream> Chart_NganHang_NimCasaChiPhiVon(IEnumerable<string> lInput);

        Task<Stream> Chart_BDS_DoanhThu_LoiNhuan(IEnumerable<string> lInput);
        Task<Stream> Chart_BDS_TonKho(IEnumerable<string> lInput);
        Task<Stream> Chart_BDS_NguoiMua(IEnumerable<string> lInput);
        Task<Stream> Chart_BDS_NoTrenVonChu(IEnumerable<string> lInput);

        Task<Stream> Chart_VIN_DoanhThu_LoiNhuan();
        Task<Stream> Chart_VIN_TonKho();
        Task<Stream> Chart_VIN_NguoiMua();
        Task<Stream> Chart_VIN_NoTrenVonChu();
    }
    public partial class BllService : IBllService
    {
        private readonly ILogger<BllService> _logger;
        private readonly IStockRepo _stockRepo;
        private readonly IStockFinancialRepo _financialRepo;
        private readonly IFinancialBDSRepo _bdsRepo;
        private readonly IFinancialNHRepo _nhRepo;
        private readonly IConfigMainRepo _configMainRepo;
        private readonly IAPIService _apiService;
        public BllService(ILogger<BllService> logger,
                            IStockRepo stockRepo,
                            IStockFinancialRepo financialRepo,
                            IFinancialBDSRepo financialBDSRepo,
                            IFinancialNHRepo financialNHRepo,
                            IConfigMainRepo configMainRepo,
                            IAPIService apiService)
        {
            _logger = logger;
            _stockRepo = stockRepo;
            _financialRepo = financialRepo;
            _bdsRepo = financialBDSRepo;
            _nhRepo = financialNHRepo;
            _configMainRepo = configMainRepo;
            _apiService = apiService;
        }
    }
}