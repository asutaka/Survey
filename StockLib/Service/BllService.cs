using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL;
using StockLib.DAL.Entity;
using StockLib.Utils;

namespace StockLib.Service
{
    public interface IBllService
    {
        Task SyncBCTC_BatDongSan();
        Task SyncBCTC_NganHang();
        Task SyncBCTC_ChungKhoan();
        Task SyncBCTC_Thep();
        Task SyncBCTC_BanLe();
        Task SyncBCTC_Dien();

        Task<Stream> Chart_NganHang_DoanhThu_LoiNhuan(IEnumerable<string> lInput);
        Task<Stream> Chart_NganHang_TangTruongTinDung_RoomTinDung(IEnumerable<string> lInput);
        Task<Stream> Chart_NganHang_NoXau(IEnumerable<string> lInput);
        Task<Stream> Chart_NganHang_NimCasaChiPhiVon(IEnumerable<string> lInput);

        Task<List<Stream>> Chart_BatDongSan(IEnumerable<string> lInput);
        //Task<Stream> Chart_BDS_DoanhThu_LoiNhuan(IEnumerable<string> lInput);
        //Task<Stream> Chart_BDS_TonKho(IEnumerable<string> lInput);
        //Task<Stream> Chart_BDS_NguoiMua(IEnumerable<string> lInput);
        //Task<Stream> Chart_BDS_NoTrenVonChu(IEnumerable<string> lInput);

        //Task<Stream> Chart_VIN_DoanhThu_LoiNhuan();
        //Task<Stream> Chart_VIN_TonKho();
        //Task<Stream> Chart_VIN_NguoiMua();
        //Task<Stream> Chart_VIN_NoTrenVonChu();

        Task<Stream> Chart_CK_DoanhThu_LoiNhuan(IEnumerable<string> lInput);
        Task<Stream> Chart_CK_TangTruongTinDung_RoomTinDung(IEnumerable<string> lInput);
        Task<Stream> Chart_CK_MoiGioi(IEnumerable<string> lInput);
        Task<Stream> Chart_CK_TuDoanh(IEnumerable<string> lInput);

        Task<Stream> Chart_Thep_DoanhThu_LoiNhuan(IEnumerable<string> lInput);
        Task<Stream> Chart_Thep_TonKho(IEnumerable<string> lInput);
        Task<Stream> Chart_Thep_NoTrenVonChu(IEnumerable<string> lInput);

        Task<Stream> Chart_BanLe_DoanhThu_LoiNhuan(IEnumerable<string> lInput);
        Task<Stream> Chart_BanLe_TonKho(IEnumerable<string> lInput);
        Task<Stream> Chart_BanLe_NoTrenVonChu(IEnumerable<string> lInput);

        Task<Stream> Chart_Dien_DoanhThu_LoiNhuan(IEnumerable<string> lInput);
        Task<Stream> Chart_Dien_NoTrenVonChu(IEnumerable<string> lInput);
    }
    public partial class BllService : IBllService
    {
        private readonly ILogger<BllService> _logger;
        private readonly IStockRepo _stockRepo;
        private readonly IStockFinancialRepo _financialRepo;
        private readonly IFinancialBDSRepo _bdsRepo;
        private readonly IFinancialNHRepo _nhRepo;
        private readonly IFinancialCKRepo _ckRepo;
        private readonly IFinancialThepRepo _thepRepo;
        private readonly IFinancialBanLeRepo _banleRepo;
        private readonly IFinancialDienRepo _dienRepo;
        private readonly IConfigDataRepo _configRepo;
        private readonly IAPIService _apiService;
        public BllService(ILogger<BllService> logger,
                            IStockRepo stockRepo,
                            IStockFinancialRepo financialRepo,
                            IFinancialBDSRepo financialBDSRepo,
                            IFinancialNHRepo financialNHRepo,
                            IFinancialCKRepo financialCKRepo,
                            IFinancialThepRepo financialThepRepo,
                            IFinancialBanLeRepo financialBanLeRepo,
                            IFinancialDienRepo financialDienRepo,
                            IConfigDataRepo configRepo,
                            IAPIService apiService)
        {
            _logger = logger;
            _stockRepo = stockRepo;
            _financialRepo = financialRepo;
            _bdsRepo = financialBDSRepo;
            _nhRepo = financialNHRepo;
            _ckRepo = financialCKRepo;
            _thepRepo = financialThepRepo;
            _banleRepo = financialBanLeRepo;
            _dienRepo = financialDienRepo;
            _configRepo = configRepo;
            _apiService = apiService;
        }

        private (long, long, long) GetCurrentTime()
        {
            var dt = DateTime.Now;
            if (StaticVal._currentTime.Item1 <= 0
                || (long.Parse($"{dt.Year}{dt.Month}") != StaticVal._currentTime.Item4))
            {
                var filter = Builders<ConfigData>.Filter.Eq(x => x.ty, (int)EConfigDataType.CurrentTime);
                var eTime = _configRepo.GetEntityByFilter(filter);
                var eYear = eTime.t / 10;
                var eQuarter = eTime.t - eYear * 10;
                StaticVal._currentTime = (eTime.t, eYear, eQuarter, long.Parse($"{eYear}{dt.Month}"));
            }

            return (StaticVal._currentTime.Item1, StaticVal._currentTime.Item2, StaticVal._currentTime.Item3);
        }
    }
}