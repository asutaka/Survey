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
            var pe = await DinhGiaPE(input, lQuote);
            var usd = EPoint.Normal;
            if (StaticVal._lDNVayVonNuocNgoai.Contains(stock.s))
            {
                usd = await DinhGia_Forex(EForex.DXU1, 2, 5);
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
                //return await Chart_Go(input);
            }

            var isDauKhi = stock.h24.Any(y => y.code == "7573" || y.code == "0500");
            if (isDauKhi)
            {
                var daumo = await DinhGia_Forex(EForex.CL, 5, 15);
                
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
        private async Task<EPoint> DinhGia_Forex(EForex forex, double step1, double step2)
        {
            try
            {
                var lVal = await _apiService.VietStock_GetForex(forex.ToString());
                if(lVal is null || !lVal.t.Any())
                    return EPoint.VeryNegative;

                var c_last = lVal.c.Last();
                var c_near = lVal.c.SkipLast(1).Last();
                var t_prev = ((long)lVal.t.Last()).UnixTimeStampToDateTime().AddYears(-1).AddMonths(1);
                var dtPrev = new DateTime(t_prev.Year, t_prev.Month, 1, 0, 0, 0);
                var timestamp = new DateTimeOffset(dtPrev).ToUnixTimeSeconds();
                var t_index = lVal.t.Where(x => x < timestamp).Max(x => x);
                var index = lVal.t.IndexOf(t_index);
                var c_prev = lVal.c.ElementAt(index);

                var qoq = Math.Round(100 * (-1 + c_last / c_prev), 1);
                var qoqoy = Math.Round(100 * (-1 + c_last / c_near), 1);

                var total_qoq = 0;
                if(qoq > step2)
                {
                    total_qoq = (int)EPoint.VeryPositive;
                }
                else if(qoq <= step2 && qoq > step1)
                {
                    total_qoq = (int)EPoint.Positive;
                }
                else if(qoq <= step1 && qoq >= -step1)
                {
                    total_qoq = (int)EPoint.Normal;
                }
                else if(qoq < -step1 && qoq >= -step2)
                {
                    total_qoq = (int)EPoint.Negative;
                }
                else
                {
                    total_qoq = (int)EPoint.VeryNegative;
                }

                var total_qoqoy = 0;
                if (qoqoy > step2)
                {
                    total_qoqoy = (int)EPoint.VeryPositive;
                }
                else if (qoqoy <= step2 && qoqoy > step1)
                {
                    total_qoqoy = (int)EPoint.Positive;
                }
                else if (qoqoy <= step1 && qoqoy >= -step1)
                {
                    total_qoqoy = (int)EPoint.Normal;
                }
                else if (qoqoy < -step1 && qoqoy >= -step2)
                {
                    total_qoqoy = (int)EPoint.Negative;
                }
                else
                {
                    total_qoqoy = (int)EPoint.VeryNegative;
                }

                var total = total_qoq * 0.6 + total_qoqoy * 0.4;
                if(total < (int)EPoint.Negative)
                {
                    return EPoint.VeryNegative;
                }
                if(total < (int)EPoint.Normal)
                {
                    return EPoint.Negative;
                }
                if(total < (int)EPoint.Positive)
                {
                    return EPoint.Normal;
                }
                if(total < (int)EPoint.VeryPositive)
                {
                    return EPoint.Positive;
                }
                return EPoint.VeryPositive;
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.DinhGia_GiaDauTho|EXCEPTION| {ex.Message}");
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