using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Skender.Stock.Indicators;
using StockLib.DAL;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public interface IDinhGiaService
    {
        Task<string> Mes_DinhGia(string input);
    }
    public partial class DinhGiaService : IDinhGiaService
    {
        private readonly ILogger<BllService> _logger;

        private readonly IChiSoPERepo _peRepo;
        private readonly IShareRepo _shareRepo;
        private readonly IKeHoachRepo _kehoachRepo;
        private readonly IThongKeRepo _thongkeRepo;
        private readonly IThongKeQuyRepo _thongkequyRepo;
        private readonly IThongKeHaiQuanRepo _haiquanRepo;
        private readonly IStockTypeRepo _stockTypeRepo;


        private readonly IAPIService _apiService;
        public DinhGiaService(ILogger<BllService> logger,
                            IChiSoPERepo peRepo,
                            IShareRepo shareRepo,
                            IKeHoachRepo kehoachRepo,
                            IThongKeRepo thongkeRepo,
                            IThongKeQuyRepo thongkequyRepo,
                            IThongKeHaiQuanRepo haiquanRepo,
                            IStockTypeRepo stockTypeRepo,
                            IAPIService apiService)
        {
            _logger = logger;
            _peRepo = peRepo;
            _shareRepo = shareRepo;
            _kehoachRepo = kehoachRepo;
            _haiquanRepo = haiquanRepo;
            _thongkeRepo = thongkeRepo;
            _thongkequyRepo = thongkequyRepo;
            _stockTypeRepo = stockTypeRepo;
            _apiService = apiService;
        }
        private async Task<(EPoint, EStockType)> DinhGiaNganh(string code, int nganh)
        {
            var eNganh = (EStockType)nganh;
            if(eNganh == EStockType.BanLe)
            {
                return (await DG_BanLe(code), eNganh);
            }

            if (eNganh == EStockType.BDS)
            {
                return (await DG_BDS(code), eNganh);
            }

            if (eNganh == EStockType.CangBien)
            {
                return (await DG_CangBien(code), eNganh);
            }

            if (eNganh == EStockType.CaoSu)
            {
                return (await DG_CaoSu(code), eNganh);
            }

            if (eNganh == EStockType.ChungKhoan)
            {
                return (await DG_ChungKhoan(code), eNganh);
            }

            if (eNganh == EStockType.DauKhi)
            {
                return (await DG_DauKhi(code), eNganh);
            }

            if (eNganh == EStockType.DetMay)
            {
                return (await DG_DetMay(code), eNganh);
            }

            if (eNganh == EStockType.DienGio)
            {
                return (await DG_DienGio(code), eNganh);
            }

            if (eNganh == EStockType.DienKhi)
            {
                return (await DG_DienKhi(code), eNganh);
            }

            if (eNganh == EStockType.DienMatTroi)
            {
                return (await DG_DienMatTroi(code), eNganh);
            }

            if (eNganh == EStockType.DienThan)
            {
                return (await DG_DienThan(code), eNganh);
            }

            if (eNganh == EStockType.ThuyDien)
            {
                return (await DG_ThuyDien(code), eNganh);
            }

            if (eNganh == EStockType.Forex)
            {
                return (Swap(await DinhGia_Forex(EForex.DXU1, 2, 5)), eNganh);
            }

            if (eNganh == EStockType.Go)
            {
                return (await DG_Go(code), eNganh);
            }

            if (eNganh == EStockType.HangKhong)
            {
                return (await DG_HangKhong(code), eNganh);
            }

            if (eNganh == EStockType.KCN)
            {
                return (await DG_KCN(code), eNganh);
            }

            if (eNganh == EStockType.Logistic)
            {
                return (await DG_Logistic(code), eNganh);
            }

            if (eNganh == EStockType.NganHang)
            {
                return (await DG_NganHang(code), eNganh);
            }

            if (eNganh == EStockType.Nhua)
            {
                return (await DG_Nhua(code), eNganh);
            }

            if (eNganh == EStockType.Oto)
            {
                return (await DG_Oto(code), eNganh);
            }

            if (eNganh == EStockType.PhanBon)
            {
                return (await DG_PhanBon(code), eNganh);
            }

            if (eNganh == EStockType.Than)
            {
                return (await DG_Than(code), eNganh);
            }

            if (eNganh == EStockType.Thep)
            {
                return (await DG_Thep(code), eNganh);
            }

            if (eNganh == EStockType.ThuySan)
            {
                return (await DG_ThuySan(code), eNganh);
            }

            if (eNganh == EStockType.Vin)
            {
                return (await DG_Vin(code), eNganh);
            }

            if (eNganh == EStockType.XayDung)
            {
                return (await DG_XayDung(code), eNganh);
            }

            if (eNganh == EStockType.XiMang)
            {
                return (await DG_XiMang(code), eNganh);
            }

            if (eNganh == EStockType.VanTaiBien)
            {
                return (await DG_VanTaiBien(code), eNganh);
            }

            if (eNganh == EStockType.ChanNuoi)
            {
                return (await DG_ChanNuoi(code), eNganh);
            }

            if (eNganh == EStockType.NongNghiep)
            {
                return (await DG_NongNghiep(code), eNganh);
            }

            if (eNganh == EStockType.HoaChat)
            {
                return (await DG_HoaChat(code), eNganh);
            }

            if (eNganh == EStockType.CaPhe)
            {
                return (await DG_CaPhe(code), eNganh);
            }

            if (eNganh == EStockType.Gao)
            {
                return (await DG_Gao(code), eNganh);
            }

            if (eNganh == EStockType.Duoc)
            {
                return (await DG_Duoc(code), eNganh);
            }

            if (eNganh == EStockType.DichVuYTe)
            {
                return (await DG_DichVuYTe(code), eNganh);
            }

            if (eNganh == EStockType.BaoHiem)
            {
                return (await DG_BaoHiem(code), eNganh);
            }

            if (eNganh == EStockType.CNTT)
            {
                return (await DG_CNTT(code), eNganh);
            }

            if (eNganh == EStockType.DauTuCong)
            {
                return (await DG_DauTuCong(code), eNganh);
            }

            if (eNganh == EStockType.ThietBiDien)
            {
                return (await DG_ThietBiDien(code), eNganh);
            }

            if (eNganh == EStockType.Duong)
            {
                return (await DG_Duong(code), eNganh);
            }

            if (eNganh == EStockType.Bia)
            {
                return (await DG_Bia(code), eNganh);
            }

            if (eNganh == EStockType.SPNongNghiepKhac)
            {
                return (await DG_SPNongNghiepKhac(code), eNganh);
            }

            if (eNganh == EStockType.NuocNgot)
            {
                return (await DG_NuocNgot(code), eNganh);
            }

            if (eNganh == EStockType.Sua)
            {
                return (await DG_Sua(code), eNganh);
            }

            if (eNganh == EStockType.XuatKhau)
            {
                return (await DG_XuatKhau(code), eNganh);
            }

            if (eNganh == EStockType.NangLuong)
            {
                return (await DG_NangLuong(code), eNganh);
            }

            if (eNganh == EStockType.Khac)
            {
                return (await DG_Khac(code), eNganh);
            }

            return (EPoint.Unknown, eNganh);
        }

        public async Task<string> Mes_DinhGia(string input)
        {
            var stock = _stockTypeRepo.GetEntityByFilter(Builders<StockType>.Filter.Eq(x => x.s, input));
            if (stock == null)
                return null;

            var strRes = new StringBuilder();
            strRes.AppendLine($"Định giá cổ phiếu {stock.s}");

            var lQuote = await _apiService.SSI_GetDataStock(stock.s);
            var pe = await DinhGiaPE(input, lQuote);
            strRes.AppendLine($"+ P/E: {pe.GetDisplayName()}");

            var lDinhGia = new List<double>();
            if(stock.ty1 > -1)
            {
                var dg = await DinhGiaNganh(stock.s, stock.ty1);
                if(dg.Item1 != EPoint.Unknown)
                {
                    strRes.AppendLine($"+ {dg.Item2.GetDisplayName()}: {dg.Item1.GetDisplayName()}");
                    lDinhGia.Add((double)dg.Item1 * stock.ty1_r);
                }
            }
            if (stock.ty2 > -1)
            {
                var dg = await DinhGiaNganh(stock.s, stock.ty2);
                if (dg.Item1 != EPoint.Unknown)
                {
                    strRes.AppendLine($"+ {dg.Item2.GetDisplayName()}: {dg.Item1.GetDisplayName()}");
                    lDinhGia.Add((double)dg.Item1 * stock.ty2_r);
                }
            }
            if (stock.ty3 > -1)
            {
                var dg = await DinhGiaNganh(stock.s, stock.ty3);
                if (dg.Item1 != EPoint.Unknown)
                {
                    strRes.AppendLine($"+  {dg.Item2.GetDisplayName()}: {dg.Item1.GetDisplayName()}");
                    lDinhGia.Add((double)dg.Item1 * stock.ty3_r);
                }
            }

            if (!lDinhGia.Any())
                return null;

            var avgPoint = Math.Round(lDinhGia.Sum() / lDinhGia.Count(), 1);
            var total = Math.Round(((double)pe + avgPoint) / 2, 1);
            if(total > (double)EPoint.Positive)
            {
                strRes.AppendLine($"=> Kết Luận: {EPoint.VeryPositive.GetDisplayName()}");
            }
            else if (total > (double)EPoint.Normal)
            {
                strRes.AppendLine($"=> Kết Luận: {EPoint.Positive.GetDisplayName()}");
            }
            else if (total > (double)EPoint.Negative)
            {
                strRes.AppendLine($"=> Kết Luận: {EPoint.Normal.GetDisplayName()}");
            }
            else if (total > (double)EPoint.VeryNegative)
            {
                strRes.AppendLine($"=> Kết Luận: {EPoint.Negative.GetDisplayName()}");
            }
            else
            {
                strRes.AppendLine($"=> Kết Luận: {EPoint.VeryNegative.GetDisplayName()}");
            }
            return strRes.ToString();

            //var isVayVonNuocNgoai = false;
            //var usd = EPoint.Normal;
            //if (StaticVal._lDNVayVonNuocNgoai.Contains(stock.s))
            //{
            //    usd = await DinhGia_Forex(EForex.DXU1, 2, 5);
            //    isVayVonNuocNgoai = true;
            //}

            //var isGo = stock.h24.Any(y => y.code == "1733");
            //if (isGo)
            //{
            //    var xk = await DinhGiaXNK(EHaiQuan.Go, 5, 15);
            //    strRes.AppendLine($"  + P/E: {pe.GetDisplayName()}");
            //    strRes.AppendLine($"  + Giá trị xuất khẩu: {xk.GetDisplayName()}");

            //    var xk_gia = await DinhGiaXNK_Gia(EHaiQuan.Go, 5, 15);
            //    if (xk_gia != EPoint.Unknown)
            //    {
            //        strRes.AppendLine($"  + Giá xuất khẩu: {xk.GetDisplayName()}");
            //    }

            //    //return await Chart_Go(input);
            //}

            //var isDauKhi = stock.h24.Any(y => y.code == "7573" || y.code == "0500");
            //if (isDauKhi)
            //{
            //    var lInput = new List<(EPoint, int)>();

            //    var daumo = await DinhGia_Forex(EForex.CL, 5, 15);
            //    strRes.AppendLine($"  + P/E: {pe.GetDisplayName()}");
            //    strRes.AppendLine($"  + Giá Dầu Thô: {daumo.GetDisplayName()}");

            //    if (isVayVonNuocNgoai)
            //    {
            //        lInput.Add((pe, 40));
            //        lInput.Add((daumo, 30));
            //        lInput.Add((Swap(usd), 30));
            //        strRes.AppendLine(Swap(usd).GetDisplayName());
            //    }
            //    else
            //    {
            //        lInput.Add((pe, 50));
            //        lInput.Add((daumo, 50));
            //    }
            //    var tong = TongDinhGia(lInput);
            //    strRes.AppendLine($"=> Kết Luận: {tong.GetDisplayName()}");
            //    return strRes.ToString();
            //}
        }
        private EPoint TongDinhGia(List<(EPoint, int)> lInput)
        {
            var result = lInput.Sum(x => (double)x.Item1 * x.Item2 / 100);
            if (result < (int)EPoint.Negative)
                return EPoint.VeryNegative;
            if (result < (int)EPoint.Normal)
                return EPoint.Negative;
            if (result < (int)EPoint.Positive)
                return EPoint.Normal;
            return EPoint.VeryPositive;
        }

        private EPoint Swap(EPoint point)
        {
            if (point == EPoint.VeryPositive)
                return EPoint.VeryNegative;
            if (point == EPoint.VeryNegative)
                return EPoint.VeryPositive;
            if (point == EPoint.Positive)
                return EPoint.Negative;
            if (point == EPoint.Negative)
                return EPoint.Positive;
            return point;
        }

        //private async Task<EPoint> DinhGiaThongKe(EKeyTongCucThongKe thongke)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.DinhGiaThongKe|EXCEPTION| {ex.Message}");
        //    }
        //}
        //private async Task<EPoint> DinhGiaThongKeQuy(EKeyTongCucThongKe thongke)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.DinhGiaThongKeQuy|EXCEPTION| {ex.Message}");
        //    }
        //}
        private async Task<EPoint> DinhGiaXNK(EHaiQuan haiquan, double step1, double step2)
        {
            try
            {
                var lXK = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)haiquan)).OrderByDescending(x => x.d);
                if (lXK is null || !lXK.Any())
                    return EPoint.VeryNegative;

                var cur = lXK.First();
                var near = lXK.Skip(1).FirstOrDefault();
                var prev = lXK.FirstOrDefault(x => x.d == cur.d - 1000);
                double va = 0;
                if (prev != null && prev.va > 0)
                {
                    var qoq_va = Math.Round(100 * (-1 + cur.va / prev.va), 1);
                    var total_va = 0;
                    if (qoq_va > step2)
                    {
                        total_va = (int)EPoint.VeryPositive;
                    }
                    else if (qoq_va <= step2 && qoq_va > step1)
                    {
                        total_va = (int)EPoint.Positive;
                    }
                    else if (qoq_va <= step1 && qoq_va >= -step1)
                    {
                        total_va = (int)EPoint.Normal;
                    }
                    else if (qoq_va < -step1 && qoq_va >= -step2)
                    {
                        total_va = (int)EPoint.Negative;
                    }
                    else
                    {
                        total_va = (int)EPoint.VeryNegative;
                    }
                    va = total_va;
                }
                if (near != null && near.va > 0)
                {
                    var qoqoy_va = Math.Round(100 * (-1 + cur.va / near.va), 1);
                    var total_va = 0;
                    if (qoqoy_va > step2)
                    {
                        total_va = (int)EPoint.VeryPositive;
                    }
                    else if (qoqoy_va <= step2 && qoqoy_va > step1)
                    {
                        total_va = (int)EPoint.Positive;
                    }
                    else if (qoqoy_va <= step1 && qoqoy_va >= -step1)
                    {
                        total_va = (int)EPoint.Normal;
                    }
                    else if (qoqoy_va < -step1 && qoqoy_va >= -step2)
                    {
                        total_va = (int)EPoint.Negative;
                    }
                    else
                    {
                        total_va = (int)EPoint.VeryNegative;
                    }
                    va = va * 0.6 + total_va * 0.4;
                }

                if (va < (int)EPoint.Negative)
                {
                    return EPoint.VeryNegative;
                }
                if (va < (int)EPoint.Normal)
                {
                    return EPoint.Negative;
                }
                if (va < (int)EPoint.Positive)
                {
                    return EPoint.Normal;
                }
                if (va < (int)EPoint.VeryPositive)
                {
                    return EPoint.Positive;
                }
                return EPoint.VeryPositive;
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.DinhGiaXNK|EXCEPTION| {ex.Message}");
            }
            return EPoint.VeryNegative;
        }
        private async Task<EPoint> DinhGiaXNK_Gia(EHaiQuan haiquan, double step1, double step2)
        {
            try
            {
                var lXK = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)haiquan)).OrderByDescending(x => x.d);
                if (lXK is null || !lXK.Any())
                    return EPoint.VeryNegative;

                var cur = lXK.First();
                var near = lXK.Skip(1).FirstOrDefault();
                var prev = lXK.FirstOrDefault(x => x.d == cur.d - 1000);
                double va = 0, price = 0;
                if (prev != null && prev.price > 0)
                {
                    var qoq_price = Math.Round(100 * (-1 + cur.price / prev.price), 1);
                    var total_price = 0;
                    if (qoq_price > step2)
                    {
                        total_price = (int)EPoint.VeryPositive;
                    }
                    else if (qoq_price <= step2 && qoq_price > step1)
                    {
                        total_price = (int)EPoint.Positive;
                    }
                    else if (qoq_price <= step1 && qoq_price >= -step1)
                    {
                        total_price = (int)EPoint.Normal;
                    }
                    else if (qoq_price < -step1 && qoq_price >= -step2)
                    {
                        total_price = (int)EPoint.Negative;
                    }
                    else
                    {
                        total_price = (int)EPoint.VeryNegative;
                    }
                    price = total_price;
                }
                if (near != null && near.price > 0)
                {
                    var qoqoy_price = Math.Round(100 * (-1 + cur.price / near.price), 1);
                    var total_price = 0;
                    if (qoqoy_price > step2)
                    {
                        total_price = (int)EPoint.VeryPositive;
                    }
                    else if (qoqoy_price <= step2 && qoqoy_price > step1)
                    {
                        total_price = (int)EPoint.Positive;
                    }
                    else if (qoqoy_price <= step1 && qoqoy_price >= -step1)
                    {
                        total_price = (int)EPoint.Normal;
                    }
                    else if (qoqoy_price < -step1 && qoqoy_price >= -step2)
                    {
                        total_price = (int)EPoint.Negative;
                    }
                    else
                    {
                        total_price = (int)EPoint.VeryNegative;
                    }
                    price = price * 0.6 + total_price * 0.4;
                }
                if (price <= 0)
                    return EPoint.Unknown;
                if (price < (int)EPoint.Negative)
                {
                    return EPoint.VeryNegative;
                }
                if (price < (int)EPoint.Normal)
                {
                    return EPoint.Negative;
                }
                if (price < (int)EPoint.Positive)
                {
                    return EPoint.Normal;
                }
                if (price < (int)EPoint.VeryPositive)
                {
                    return EPoint.Positive;
                }
                return EPoint.VeryPositive;
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.DinhGiaXNK_Gia|EXCEPTION| {ex.Message}");
            }
            return EPoint.Unknown;
        }
        private async Task<EPoint> DinhGiaPE(string code, List<Quote> lQuote)
        {
            try
            {
                var dt = DateTime.Now;
                if (lQuote is null || !lQuote.Any())
                {
                    return EPoint.VeryNegative;
                }
                //pe
                var lKehoach = _kehoachRepo.GetByFilter(Builders<KeHoach>.Filter.Eq(x => x.s, code));
                if (lKehoach is null || !lKehoach.Any())
                {
                    return EPoint.Negative;
                }

                var curPlan = lKehoach.FirstOrDefault(x => x.d == dt.Year);
                if (curPlan is null)
                {
                    return EPoint.Negative;
                }

                double pf_truth = 0;
                var avgRate = lKehoach.Where(x => x.pf_real > 0).Average(x => x.pf_real_r);
                var cum = lKehoach.FirstOrDefault(x => x.pf_cum > 0);
                if (cum != null)
                {
                    var quarter = dt.GetQuarter() - 1;
                    var flag = cum.pf_cum_r >= quarter * 25;
                    if (flag)
                    {
                        pf_truth = curPlan.pf_plan;
                    }
                }

                if (pf_truth == 0)
                {
                    pf_truth = Math.Round(curPlan.pf_plan * avgRate / 100, 1);
                }

                if (pf_truth < 0)
                {
                    return EPoint.VeryNegative;
                }

                if (pf_truth == 0)
                {
                    return EPoint.Negative;
                }

                //True Path 
                var share = _shareRepo.GetEntityByFilter(Builders<Share>.Filter.Eq(x => x.s, code));
                if (share is null || share.share <= 0)
                {
                    return EPoint.VeryNegative;
                }

                var eps_truth = Math.Round(pf_truth * 1000000000 / share.share, 1);
                if (eps_truth == 0)
                {
                    return EPoint.VeryNegative;
                }
                var quote = lQuote.MaxBy(x => x.Date);//Giá mới nhất
                var pe_truth = Math.Round((double)quote.Close * 1000 / eps_truth, 1);
                if (pe_truth <= 0)
                {
                    return EPoint.VeryNegative;
                }

                var lpe = _peRepo.GetByFilter(Builders<ChiSoPE>.Filter.Eq(x => x.s, code));
                var lastPE = lpe.MaxBy(x => x.d);
                if (lastPE.eps <= 0)
                {
                    return EPoint.VeryNegative;
                }

                var pe_cur = Math.Round((double)quote.Close * 1000 / lastPE.eps, 1);
                var pe_avg = lpe.Where(x => x.d.ToString().EndsWith(dt.GetQuarter().ToString())).Average(x => x.pe);
                if (pe_cur >= pe_avg
                    || pe_cur >= pe_truth)
                {
                    return EPoint.Normal;
                }

                if (pe_cur * 1.05 < pe_truth)
                {
                    return EPoint.VeryPositive;
                }

                return EPoint.Positive;
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.DinhGiaPE|EXCEPTION| {ex.Message}");
            }
            return EPoint.VeryNegative;
        }
    }
}
