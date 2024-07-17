using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SLib.Model;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        //TA: Giá hiện tại, chỉ báo bắt đáy, ma20, ichi, rsi zero, vol, ema21, ema50, e21cross50
        public async Task<(int, string)> TA(string code)
        {
            var strOut = new StringBuilder();
            try
            {
                var chibao = await ChiBaoKyThuatOnlyStock(code, 0);
                if (chibao is null)
                    return (0, null);

                strOut.AppendLine($"*Giá hiện tại: {chibao.curPrice}({Math.Round(100 * (-1 + chibao.curPrice / chibao.basicPrice), 1)}%)");
                var strChiBao = new StringBuilder();
                if (chibao.isCrossMa20Up)
                {
                    strChiBao.AppendLine("+ Giá cắt lên MA20");
                }
                if (chibao.isIchi)
                {
                    strChiBao.AppendLine("+ Giá nằm trên Ichimoku");
                }
                if (chibao.isRsiZero)
                {
                    strChiBao.AppendLine("+ RSI Zero");
                }
                if (chibao.isCrossEma21Up)
                {
                    strChiBao.AppendLine("+ Giá cắt lên E21");
                }
                if (chibao.isCrossEma50Up)
                {
                    strChiBao.AppendLine("+ Giá cắt lên E50");
                }
                if (chibao.isEma21_50)
                {
                    strChiBao.AppendLine("+ E21 cắt E50");
                }
                if (strChiBao.Length > 0)
                {
                    strOut.AppendLine($"*TA:");
                    strOut.Append(strChiBao.ToString());
                }

                return (1, strOut.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return (0, null);
        }

        //Thống kê giao dịch: + NN mua bán + Tự doanh + Mua bán chủ động
        public async Task<(int, string)> ThongKeGD(string code)
        {
            try
            {
                var strOut = new StringBuilder();
                strOut.AppendLine($"*Thống kê giao dịch:");
                var dt = DateTime.Now;
                var dtFirstWeek = dt.AddDays(-(int)dt.DayOfWeek);
                var lData = await _apiService.GetForeign(code, 1, 1000, dtFirstWeek.ToString("dd/MM/yyyy"), dt.AddDays(1).ToString("dd/MM/yyyy"));
                if (lData?.Any() ?? false) 
                {
                    //NN
                    var curRoom = lData.First().foreignCurrentRoom;
                    var buySell = lData.Sum(x => x.netBuySellVal);
                    var flagNN = buySell >= 0 ? "Mua ròng" : "Bán ròng";
                    strOut.AppendLine($"+ Room ngoại: {curRoom.ToString("#,##0")} cổ phiếu");

                    var strBuySell = string.Empty;
                    if (buySell > 1000000000)
                    {
                        buySell = Math.Round(buySell / 1000000000, 1);
                        strBuySell = $"{flagNN} {buySell.ToString("#,##0.#")} tỷ";
                    }
                    else if (buySell > 1000000)
                    {
                        buySell = Math.Round(buySell / 1000000, 1);
                        strBuySell = $"{flagNN} {buySell.ToString("#,##0.#")} triệu";
                    }
                    else
                    {
                        strBuySell = $"Mua ròng 0 đồng";
                    }
                    strOut.AppendLine($"+ GDNN: {strBuySell}");
                    
                    //Tự doanh
                    FilterDefinition<TuDoanh> filter = Builders<TuDoanh>.Filter.Eq(x => x.s, code);
                    var lTuDoanh = _tudoanhRepo.GetByFilter(filter);
                    lTuDoanh = lTuDoanh.Where(x => x.d > new DateTimeOffset(dtFirstWeek.AddDays(-1)).ToUnixTimeSeconds()).ToList();
                    var tudoanh = lTuDoanh.Sum(x => x.net);
                    var flagTuDoanh = tudoanh >= 0 ? "Mua ròng" : "Bán ròng";
                    var valStr = string.Empty;
                    tudoanh = Math.Abs(tudoanh / 1000);
                    if (tudoanh < 1000)
                    {
                        valStr = $"{Math.Round(tudoanh, 0)} triệu";
                    }
                    else
                    {
                        tudoanh = Math.Round(tudoanh / 1000, 1);
                        valStr = $"{tudoanh} tỷ";
                    }
                    strOut.AppendLine($"+ Tự doanh: {flagTuDoanh} {valStr}");

                    //Mua bán chủ động
                    var muachudong = lData.Sum(x => x.totalBuyTradeVol);
                    var banchudong = lData.Sum(x => x.totalSellTradeVol);
                    var tongmuabanchudong = muachudong + banchudong;
                    if(banchudong <= 0)
                    {
                        banchudong = 1;
                        muachudong = 0;
                    }
                    var rateMuachudong = Math.Round(muachudong * 100 / tongmuabanchudong, 1);
                    var rateBanchudong = 100 - rateMuachudong;
                    strOut.AppendLine($"+ Mua/ Bán chủ động: {rateMuachudong}%/ {rateBanchudong}%");
                    return (1, strOut.ToString());
                }    

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (0, null);
        }

        //FA: + Lợi nhuận + Kế hoạch năm +BCTC quý x
        public async Task<(int, string)> FA(string code)
        {
            try
            {
                var strOut = new StringBuilder();
                strOut.AppendLine($"*FA:");
                //Lợi nhuận
                var loinhuan = await ThongKeLoiNhuan(code);
                if(loinhuan.Item1 > 0)
                {
                    strOut.AppendLine(loinhuan.Item2);
                }
                //Kế hoạch năm
                var kehoachNam = await KeHoachNam(code);
                if(kehoachNam.Item1 > 0)
                {
                    strOut.AppendLine(kehoachNam.Item2);
                }
                var linkKeHoach = $"https://24hmoney.vn/stock/{code}/ke-hoach-kinh-doanh";
                strOut.AppendLine($"Link kế hoạch: {linkKeHoach}");
                //BCTC quý
                var linkBCTC = $"https://finance.vietstock.vn/{code}/tai-tai-lieu.htm?doctype=1";
                strOut.AppendLine($"Link BCTC: {linkBCTC}");
                return (1, strOut.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (0, null);
        }

        //Thống kê khác: + Lợi nhuận DN tb năm + Đà tăng giá cp tb năm + buy MAup/sell MAdown
        public async Task<(int, string)> ThongKeKhac(string code)
        {
            try
            {
                //+Q1 / 2024: 319.1 tỷ / -20.01 %
                //+Q4 / 2023: 403.3 tỷ / 7.34 %
                //+Q3 / 2023: 109.5 tỷ / -69.29 %
                //+Q2 / 2023: 95.3 tỷ / -83.08 %
                //+Q1 / 2023: 398.9 tỷ / 22.67 %
                //+Q4 / 2022: 375.7 tỷ / -48.6 %
                //+Q3 / 2022: 356.6 tỷ / 19.56 %
                //+Q2 / 2022: 562.8 tỷ / 710.77 %
                //+Q1 / 2022: 325.2 tỷ / -34.25 %
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (0, null);
        }

        //Chuyên sâu: + Cơ cấu lợi nhuận + Phân tích lợi nhuận + Động lực tăng trưởng 
        public async Task<(int, string)> PTChuyenSau(string code)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (0, null);
        }
    }
}
