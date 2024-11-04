using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL;
using StockLib.DAL.Entity;
using StockLib.Utils;

namespace StockLib.Service
{
    public interface IBllService
    {
        Task SyncTest();
        Task SyncBCTC_BatDongSan(bool onlyLast = false);
        Task SyncBCTC_NganHang(bool onlyLast = false);
        Task SyncBCTC_ChungKhoan(bool onlyLast = false);
        Task SyncBCTC_Thep(bool onlyLast = false);
        Task SyncBCTC_BanLe(bool onlyLast = false);
        Task SyncBCTC_Dien(bool onlyLast = false);
        Task SyncBCTC_CangBien(bool onlyLast = false);
        Task SyncBCTC_CaoSu(bool onlyLast = false);
        Task SyncBCTC_DetMay(bool onlyLast = false);
        Task SyncBCTC_Go(bool onlyLast = false);
        Task SyncBCTC_HangKhong(bool onlyLast = false);
        Task SyncBCTC_Logistic(bool onlyLast = false);
        Task SyncBCTC_Nhua(bool onlyLast = false);
        Task SyncBCTC_Oto(bool onlyLast = false);
        Task SyncBCTC_PhanBon(bool onlyLast = false);
        Task SyncBCTC_Than(bool onlyLast = false);
        Task SyncBCTC_ThuySan(bool onlyLast = false);
        Task SyncBCTC_Ximang(bool onlyLast = false);
        Task SyncBCTC_DauKhi(bool onlyLast = false);

        Task SyncPE();
        Task SyncKeHoach();
        Task SyncShare();


        Task<List<Stream>> Chart_BatDongSan(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_NganHang(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_ChungKhoan(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_Thep(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_BanLe(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_Dien(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_HangKhong(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_Logistic(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_CangBien(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_CaoSu(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_DetMay(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_Ximang(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_Than(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_PhanBon(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_Nhua(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_ThuySan(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_Oto(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_Go(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_KCN(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_XayDung(IEnumerable<string> lInput);
        Task<List<Stream>> Chart_DauKhi(IEnumerable<string> lInput);
        Task<string> Mes_DinhGia(string input);
        Task<List<Stream>> Chart_MaCK(string input);
    }
    public partial class BllService : IBllService
    {
        private readonly ILogger<BllService> _logger;
        private readonly IStockRepo _stockRepo;
        private readonly IFinancialRepo _financialRepo;
        private readonly IKeHoachRepo _kehoachRepo;
        private readonly IChiSoPERepo _peRepo;
        private readonly IConfigDataRepo _configRepo;
        private readonly IThongKeRepo _thongkeRepo;
        private readonly IThongKeQuyRepo _thongkequyRepo;
        private readonly IThongKeHaiQuanRepo _haiquanRepo;
        private readonly IAPIService _apiService;
        private readonly IDinhGiaService _dinhgiaService;

        public BllService(ILogger<BllService> logger,
                            IStockRepo stockRepo,
                            IFinancialRepo financialRepo,
                            IKeHoachRepo kehoachRepo,
                            IChiSoPERepo peRepo,
                            IConfigDataRepo configRepo,
                            IThongKeRepo thongkeRepo,
                            IThongKeQuyRepo thongkequyRepo,
                            IThongKeHaiQuanRepo haiquanRepo,
                            IAPIService apiService,
                            IDinhGiaService dinhgiaService)
        {
            _logger = logger;
            _stockRepo = stockRepo;
            _financialRepo = financialRepo;
            _peRepo = peRepo;
            _configRepo = configRepo;
            _thongkequyRepo = thongkequyRepo;
            _thongkeRepo = thongkeRepo;
            _haiquanRepo = haiquanRepo;
            _apiService = apiService;
            _dinhgiaService = dinhgiaService;
            _kehoachRepo = kehoachRepo;
            StockInstance();
        }

        public async Task<string> Mes_DinhGia(string input)
        {
            return await _dinhgiaService.Mes_DinhGia(input);
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

        private List<Stock> StockInstance()
        {
            if (StaticVal._lStock != null && StaticVal._lStock.Any())
                return StaticVal._lStock;
            StaticVal._lStock = _stockRepo.GetAll();

            return StaticVal._lStock;
        }
    }
}