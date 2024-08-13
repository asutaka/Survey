using Microsoft.Extensions.Logging;
using StockLib.DAL;

namespace StockLib.Service
{
    public interface IBllService
    {
        Task SyncBCTC_BatDongSan();
        Task SyncBCTC_NganHang();
        Task<Stream> Chart_DoanhThu_LoiNhuan(IEnumerable<string> lNganHang);
        Task<Stream> Chart_TangTruongTinDung_RoomTinDung(IEnumerable<string> lNganHang);
        Task<Stream> Chart_NoXau(IEnumerable<string> lNganHang);
        Task<Stream> Chart_NoXau2(IEnumerable<string> lNganHang);
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