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
        Task SyncBCTC_CangBien();
        Task SyncBCTC_CaoSu();
        Task SyncBCTC_DetMay();
        Task SyncBCTC_Go();
        Task SyncBCTC_HangKhong();
        Task SyncBCTC_Logistic();
        Task SyncBCTC_Nhua();
        Task SyncBCTC_Oto();
        Task SyncBCTC_PhanBon();
        Task SyncBCTC_Than();
        Task SyncBCTC_ThuySan();
        Task SyncBCTC_Ximang();
        Task SyncBCTC_DauKhi();

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
        Task DetectStockType();
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
        private readonly IFinancialOtoRepo _otoRepo;
        private readonly IFinancialCangBienRepo _cangbienRepo;
        private readonly IFinancialPhanBonRepo _phanbonRepo;
        private readonly IFinancialThanRepo _thanRepo;
        private readonly IFinancialThuySanRepo _thuysanRepo;
        private readonly IFinancialNhuaRepo _nhuaRepo;
        private readonly IFinancialXimangRepo _ximangRepo;
        private readonly IFinancialCaoSuRepo _caosuRepo;
        private readonly IFinancialDauTuCongRepo _dtcRepo;
        private readonly IFinancialDetMayRepo _detmayRepo;
        private readonly IFinancialGoRepo _goRepo;
        private readonly IFinancialDauKhiRepo _daukhiRepo;
        private readonly IFinancialHangKhongRepo _hangkhongRepo;
        private readonly IFinancialLogisticRepo _logisticRepo;
        private readonly IShareRepo _shareRepo;
        private readonly IKeHoachRepo _kehoachRepo;
        private readonly IChiSoPERepo _peRepo;
        private readonly IConfigDataRepo _configRepo;
        private readonly IThongKeRepo _thongkeRepo;
        private readonly IThongKeQuyRepo _thongkequyRepo;
        private readonly IThongKeHaiQuanRepo _haiquanRepo;
        private readonly IStockTypeRepo _stockTypeRepo;
        private readonly IAPIService _apiService;
        private readonly IDinhGiaService _dinhgiaService;

        public BllService(ILogger<BllService> logger,
                            IStockRepo stockRepo,
                            IStockFinancialRepo financialRepo,
                            IStockTypeRepo stockTypeRepo,
                            IFinancialBDSRepo financialBDSRepo,
                            IFinancialNHRepo financialNHRepo,
                            IFinancialCKRepo financialCKRepo,
                            IFinancialThepRepo financialThepRepo,
                            IFinancialBanLeRepo financialBanLeRepo,
                            IFinancialDienRepo financialDienRepo,
                            IFinancialOtoRepo otoRepo,
                            IFinancialCangBienRepo cangbienRepo,
                            IFinancialPhanBonRepo phanbonRepo,
                            IFinancialThanRepo thanRepo,
                            IFinancialThuySanRepo thuysanRepo,
                            IFinancialCaoSuRepo caosuRepo,
                            IFinancialDauTuCongRepo dtcRepo,
                            IFinancialDetMayRepo detmayRepo,
                            IFinancialGoRepo goRepo,
                            IFinancialDauKhiRepo daukhiRepo,
                            IFinancialHangKhongRepo hangkhongRepo,
                            IFinancialNhuaRepo nhuaRepo,
                            IFinancialXimangRepo ximangRepo,
                            IFinancialLogisticRepo logisticRepo,
                            IShareRepo shareRepo,
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
            _stockTypeRepo = stockTypeRepo;
            _financialRepo = financialRepo;
            _bdsRepo = financialBDSRepo;
            _nhRepo = financialNHRepo;
            _ckRepo = financialCKRepo;
            _thepRepo = financialThepRepo;
            _banleRepo = financialBanLeRepo;
            _dienRepo = financialDienRepo;
            _otoRepo = otoRepo;
            _cangbienRepo = cangbienRepo;
            _phanbonRepo = phanbonRepo;
            _thanRepo = thanRepo;
            _thuysanRepo = thuysanRepo;
            _caosuRepo = caosuRepo;
            _dtcRepo = dtcRepo;
            _detmayRepo = detmayRepo;
            _peRepo = peRepo;
            _goRepo = goRepo;
            _hangkhongRepo = hangkhongRepo;
            _nhuaRepo = nhuaRepo;
            _ximangRepo = ximangRepo;
            _logisticRepo = logisticRepo;
            _daukhiRepo = daukhiRepo;
            _configRepo = configRepo;
            _thongkequyRepo = thongkequyRepo;
            _thongkeRepo = thongkeRepo;
            _haiquanRepo = haiquanRepo;
            _apiService = apiService;
            _dinhgiaService = dinhgiaService;
            _shareRepo = shareRepo;
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

            //DetectStockType().GetAwaiter().GetResult();

            return StaticVal._lStock;
        }

        public async Task DetectStockType()
        {
            foreach (var stock in StaticVal._lStock)
            {
                try
                {
                    var model = new StockType
                    {
                        s = stock.s
                    };
                    var lType = new List<EStockType>();
                    var isXayDung = StaticVal._lXayDung.Any(x => x == stock.s);
                    if (isXayDung)
                    {
                        lType.Add(EStockType.XayDung);
                    }
                    var isKCN = StaticVal._lKCN.Any(x => x == stock.s);
                    if (isKCN)
                    {
                        lType.Add(EStockType.KCN);
                    }
                    var isVin = StaticVal._lVin.Any(x => x == stock.s);
                    if (isVin)
                    {
                        lType.Add(EStockType.Vin);
                    }
                    var isBDS = !isXayDung && !isKCN && !isVin && stock.h24.Any(y => y.code == "2357" || y.code == "8600");
                    if (isBDS)
                    {
                        lType.Add(EStockType.BDS);
                    }
                    var isNganHang = stock.h24.Any(y => y.code == "8300");
                    if (isNganHang)
                    {
                        lType.Add(EStockType.NganHang);
                    }
                    var isChungKhoan = stock.h24.Any(y => y.code == "8777");
                    if (isChungKhoan)
                    {
                        lType.Add(EStockType.ChungKhoan);
                    }
                    var isThep = stock.h24.Any(y => y.code == "1757");
                    if (isThep)
                    {
                        lType.Add(EStockType.Thep);
                    }
                    var isBanLe = stock.h24.Any(y => y.code == "5379"
                                                || y.code == "3530"
                                                || y.code == "3577");
                    if (isBanLe)
                    {
                        lType.Add(EStockType.BanLe);
                    }

                    var isDien = stock.h24.Any(y => y.code == "7535");
                    if (isDien)
                    {
                        lType.Add(EStockType.ThuyDien);
                    }
                    var isCangBien = stock.h24.Any(y => y.code == "2777");
                    if (isCangBien)
                    {
                        lType.Add(EStockType.CangBien);
                    }

                    var isLogistic = stock.h24.Any(y => y.code == "2773" || y.code == "2779");
                    if (isLogistic)
                    {
                        lType.Add(EStockType.Logistic);
                    }

                    var isHangKhong = stock.h24.Any(y => y.code == "5751");
                    if (isHangKhong)
                    {
                        lType.Add(EStockType.HangKhong);
                    }
                    var isCaoSu = StaticVal._lCaoSu.Any(x => x == stock.s);
                    if (isCaoSu)
                    {
                        lType.Add(EStockType.CaoSu);
                    }

                    var isNhua = StaticVal._lNhua.Any(x => x == stock.s);
                    if (isNhua)
                    {
                        lType.Add(EStockType.Nhua);
                    }

                    var isOto = stock.h24.Any(y => y.code == "3353");
                    if (isOto)
                    {
                        lType.Add(EStockType.Oto);
                    }
                    var isPhanBon = StaticVal._lPhanBon.Any(x => x == stock.s);
                    if (isPhanBon)
                    {
                        lType.Add(EStockType.PhanBon);
                    }

                    var isThan = stock.h24.Any(y => y.code == "1771");
                    if (isThan)
                    {
                        lType.Add(EStockType.Than);
                    }

                    var isThuySan = StaticVal._lThuySan.Any(x => x == stock.s);
                    if (isThuySan)
                    {
                        lType.Add(EStockType.ThuySan);
                    }

                    var isXimang = StaticVal._lXimang.Any(x => x == stock.s);
                    if (isXimang)
                    {
                        lType.Add(EStockType.XiMang);
                    }

                    var isDetmay = stock.h24.Any(y => y.code == "3763");
                    if (isDetmay)
                    {
                        lType.Add(EStockType.DetMay);
                    }
                    var isGo = stock.h24.Any(y => y.code == "1733");
                    if (isGo)
                    {
                        lType.Add(EStockType.Go);
                        //var xk = await DinhGiaXNK(EHaiQuan.Go, 5, 15);
                        //strRes.AppendLine($"  + P/E: {pe.GetDisplayName()}");
                        //strRes.AppendLine($"  + Giá trị xuất khẩu: {xk.GetDisplayName()}");

                        //var xk_gia = await DinhGiaXNK_Gia(EHaiQuan.Go, 5, 15);
                        //if (xk_gia != EPoint.Unknown)
                        //{
                        //    strRes.AppendLine($"  + Giá xuất khẩu: {xk.GetDisplayName()}");
                        //}

                        //return await Chart_Go(input);
                    }

                    var isDauKhi = stock.h24.Any(y => y.code == "7573" || y.code == "0500");
                    if (isDauKhi)
                    {
                        lType.Add(EStockType.DauKhi);
                        //var lInput = new List<(EPoint, int)>();

                        //var daumo = await Forex(EForex.CL, 5, 15);
                        //strRes.AppendLine($"  + P/E: {pe.GetDisplayName()}");
                        //strRes.AppendLine($"  + Giá Dầu Thô: {daumo.GetDisplayName()}");

                        //if (isVayVonNuocNgoai)
                        //{
                        //    lInput.Add((pe, 40));
                        //    lInput.Add((daumo, 30));
                        //    lInput.Add((Swap(usd), 30));
                        //    strRes.AppendLine(Swap(usd).GetDisplayName());
                        //}
                        //else
                        //{
                        //    lInput.Add((pe, 50));
                        //    lInput.Add((daumo, 50));
                        //}
                        //var tong = TongDinhGia(lInput);
                        //strRes.AppendLine($"=> Kết Luận: {tong.GetDisplayName()}");
                        //return strRes.ToString();
                    }
                    if (StaticVal._lDNVayVonNuocNgoai.Contains(stock.s))
                    {
                        lType.Add(EStockType.Forex);
                    }

                    if (!lType.Any())
                        continue;

                    var count = lType.Count();
                    if (count > 0)
                    {
                        model.ty1 = (int)lType[0];
                    }
                    if (count > 1)
                    {
                        model.ty2 = (int)lType[1];
                    }
                    if (count > 2)
                    {
                        model.ty3 = (int)lType[2];
                    }

                    _stockTypeRepo.InsertOne(model);
                }
                catch (Exception ex)
                {
                    var tmp = ex.Message;
                }
            }
        }
    }
}