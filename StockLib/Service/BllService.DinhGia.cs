using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Skender.Stock.Indicators;
using StockLib.DAL.Entity;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<string> Mes_DinhGia(string input)
        {
            var stock = StaticVal._lStock.FirstOrDefault(x => x.s == input);
            if (stock == null)
                return null;

            var lQuote = await _apiService.SSI_GetDataStock(stock.s);
            var pe = DinhGiaPE(input, lQuote);

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
                //return await Chart_Go(input);
            }

            var isDauKhi = stock.h24.Any(y => y.code == "7573" || y.code == "0500");
            if (isDauKhi)
            {
                //return await Chart_DauKhi(input);
            }
            return null;
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
            catch(Exception ex)
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