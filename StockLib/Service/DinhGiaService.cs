using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Skender.Stock.Indicators;
using StockLib.DAL;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<string> Mes_DinhGia(string input)
        {
            var stock = StaticVal._lStock.FirstOrDefault(x => x.s == input);
            if (stock == null)
                return null;

            var strRes = new StringBuilder();
            strRes.AppendLine($"Định giá cổ phiếu {stock.s}");

            var lQuote = await _apiService.SSI_GetDataStock(stock.s);
            var pe = await DinhGiaPE(input, lQuote);
            var isVayVonNuocNgoai = false;
            var usd = EPoint.Normal;
            if (StaticVal._lDNVayVonNuocNgoai.Contains(stock.s))
            {
                usd = await Forex(EForex.DXU1, 2, 5);
                isVayVonNuocNgoai = true;
            }

            var isXayDung = StaticVal._lXayDung.Any(x => x == stock.s);
            if (isXayDung)
            {
                //return await Chart_XayDung(input);
            }
            var isKCN = StaticVal._lKCN.Any(x => x == stock.s);
            if (isKCN)
            {
                //return await Chart_KCN(input);
            }
            var isVin = StaticVal._lVin.Any(x => x == stock.s);
            if (isVin)
            {
                //return await Chart_BatDongSan(input);
            }
            var isBDS = !isXayDung && !isKCN && !isVin && stock.h24.Any(y => y.code == "2357" || y.code == "8600");
            if (isBDS)
            {
                //return await Chart_BatDongSan(input);
            }
            var isNganHang = stock.h24.Any(y => y.code == "8300");
            if (isNganHang)
            {
                //return await Chart_NganHang(input);
            }
            var isChungKhoan = stock.h24.Any(y => y.code == "8777");
            if (isChungKhoan)
            {
                //return await Chart_ChungKhoan(input);
            }
            var isThep = stock.h24.Any(y => y.code == "1757");
            if (isThep)
            {
                //return await Chart_Thep(input);
            }
            var isBanLe = stock.h24.Any(y => y.code == "5379"
                                        || y.code == "3530"
                                        || y.code == "3577");
            if (isBanLe)
            {
                //return await Chart_BanLe(input);
            }

            var isDien = stock.h24.Any(y => y.code == "7535");
            if (isDien)
            {
                //return await Chart_Dien(input);
            }

            var isCangBien = stock.h24.Any(y => y.code == "2777");
            if (isCangBien)
            {
                //return await Chart_CangBien(input);
            }

            var isLogistic = stock.h24.Any(y => y.code == "2773" || y.code == "2779");
            if (isLogistic)
            {
                //return await Chart_Logistic(input);
            }

            var isHangKhong = stock.h24.Any(y => y.code == "5751");
            if (isHangKhong)
            {
                //return await Chart_HangKhong(input);
            }

            var isCaoSu = StaticVal._lCaoSu.Any(x => x == stock.s);
            if (isCaoSu)
            {
                //return await Chart_CaoSu(input);
            }

            var isNhua = StaticVal._lNhua.Any(x => x == stock.s);
            if (isNhua)
            {
                //return await Chart_Nhua(input);
            }

            var isOto = stock.h24.Any(y => y.code == "3353");
            if (isOto)
            {
                //return await Chart_Oto(input);
            }

            var isPhanBon = StaticVal._lPhanBon.Any(x => x == stock.s);
            if (isPhanBon)
            {
                // return await Chart_PhanBon(input);
            }

            var isThan = stock.h24.Any(y => y.code == "1771");
            if (isThan)
            {
                //return await Chart_Than(input);
            }

            var isThuySan = StaticVal._lThuySan.Any(x => x == stock.s);
            if (isThuySan)
            {
                //return await Chart_ThuySan(input);
            }

            var isXimang = StaticVal._lXimang.Any(x => x == stock.s);
            if (isXimang)
            {
                //return await Chart_Ximang(input);
            }

            var isDetmay = stock.h24.Any(y => y.code == "3763");
            if (isDetmay)
            {
                //return await Chart_DetMay(input);
            }

            var isGo = stock.h24.Any(y => y.code == "1733");
            if (isGo)
            {
                var xk = await DinhGiaXNK(EHaiQuan.Go, 5, 15);
                strRes.AppendLine($"  + P/E: {pe.GetDisplayName()}");
                strRes.AppendLine($"  + Giá trị xuất khẩu: {xk.GetDisplayName()}");

                var xk_gia = await DinhGiaXNK_Gia(EHaiQuan.Go, 5, 15);
                if (xk_gia != EPoint.Unknown)
                {
                    strRes.AppendLine($"  + Giá xuất khẩu: {xk.GetDisplayName()}");
                }

                //return await Chart_Go(input);
            }

            var isDauKhi = stock.h24.Any(y => y.code == "7573" || y.code == "0500");
            if (isDauKhi)
            {
                var lInput = new List<(EPoint, int)>();

                var daumo = await Forex(EForex.CL, 5, 15);
                strRes.AppendLine($"  + P/E: {pe.GetDisplayName()}");
                strRes.AppendLine($"  + Giá Dầu Thô: {daumo.GetDisplayName()}");

                if (isVayVonNuocNgoai)
                {
                    lInput.Add((pe, 40));
                    lInput.Add((daumo, 30));
                    lInput.Add((Swap(usd), 30));
                    strRes.AppendLine(Swap(usd).GetDisplayName());
                }
                else
                {
                    lInput.Add((pe, 50));
                    lInput.Add((daumo, 50));
                }
                var tong = TongDinhGia(lInput);
                strRes.AppendLine($"=> Kết Luận: {tong.GetDisplayName()}");
                return strRes.ToString();
            }
            return null;
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
//private async Task MaChungKhoanDescription(long userId, string input)
//{
//    if (input.Equals("PVD"))
//    {
//        await BotInstance().SendTextMessageAsync(userId, $"{input}: Cho thuê giàn khoan");
//    }
//    else if (input.Equals("PVS"))
//    {
//        await BotInstance().SendTextMessageAsync(userId, $"{input}: Thăm dò và khai thác dầu khí");
//    }
//    else if (input.Equals("PVT") || input.Equals("PVP"))
//    {
//        await BotInstance().SendTextMessageAsync(userId, $"{input}: Vận tải dầu khí");
//    }
//    else if (input.Equals("GAS"))
//    {
//        await BotInstance().SendTextMessageAsync(userId, $"{input}: Chế biến và phân phối khí(mua trong nước + nhập khẩu)");
//    }
//    else if (input.Equals("BSR"))
//    {
//        await BotInstance().SendTextMessageAsync(userId, $"{input}: Chế biến dầu mỏ(100% nhập khẩu)");
//    }
//    else if (input.Equals("POW"))
//    {
//        await BotInstance().SendTextMessageAsync(userId, $"{input}: Điện khí(khí là nguyên liệu đầu vào)");
//    }
//    else if (input.Equals("DPM") || input.Equals("DCM"))
//    {
//        await BotInstance().SendTextMessageAsync(userId, $"{input}: Dùng khí để tổng hợp NH3");
//    }
//    else if (input.Equals("PLX"))
//    {
//        await BotInstance().SendTextMessageAsync(userId, $"{input}: Phân phối dầu khí(chiếm 50% thị phần)");
//    }
//    else if (input.Equals("OIL"))
//    {
//        await BotInstance().SendTextMessageAsync(userId, $"{input}: Phân phối dầu khí(chiếm 20% thị phần)");
//    }
//}