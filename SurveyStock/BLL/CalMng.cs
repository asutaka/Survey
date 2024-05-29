using Skender.Stock.Indicators;
using SurveyStock.DAL;
using SurveyStock.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyStock.BLL
{
    public static class CalMng
    {
        /// <summary>
        /// Tinh so luong co phieu tren ma20 cung ngay
        /// </summary>
        public static void Ma20RateAboveBelow_1d()
        {
            if(!PrepareData._dic1d.Any())
            {
                PrepareData.LoadData1d();
                PrepareIndicator.Ma20_1d();
            }
            //Lay ma co ngay len san som nhat
            var dic = PrepareData._dicEx1d.OrderByDescending(x => x.Value.Count());
            var itemOldest = dic.First();
            var lDate = itemOldest.Value.Select(x => x.Date);
            var countDate = lDate.Count();
            //Danh sách thống kê
            var lStastitic = new List<(DateTime, double)>();
            for (int i = 0; i < countDate; i++)
            {
                var itemDate = lDate.ElementAt(i);

                var lEntityOfDate = dic.Where(x => x.Value.Any(y => y.Date == itemDate))
                .SelectMany(p => p.Value)
                .Where(c => c.Date == itemDate)
                .Select(x => x);

                float countValid = lEntityOfDate.Count();
                var countRateValid = lEntityOfDate.Count(x => x.Ma20 > 0 && x.Close >= (decimal)x.Ma20);
                var itemStastitic = (itemDate, Math.Round(countRateValid / countValid, 2));
                lStastitic.Add(itemStastitic);
            }

            foreach (var item in lStastitic)
            {
                Console.WriteLine($"{item.Item1.ToString("dd/MM/yyyy")}: {item.Item2}");
            }
        }
        /// <summary>
        /// Tinh so luong co phieu tren ma20 cung tuan
        /// </summary>
        public static void Ma20RateAboveBelow_1w()
        {
            if (!PrepareData._dicEx1w.Any())
            {
                PrepareData.LoadData1W();
                PrepareIndicator.Ma20_1w();
            }
            //Lay ma co ngay len san som nhat
            var dic = PrepareData._dicEx1w.OrderByDescending(x => x.Value.Count());
            var itemOldest = dic.First();
            var lDate = itemOldest.Value.Select(x => x.Date);
            var countDate = lDate.Count();
            //Danh sách thống kê
            var lStastitic = new List<(DateTime, double)>();
            for (int i = 0; i < countDate; i++)
            {
                var itemDate = lDate.ElementAt(i);

                var lEntityOfDate = dic.Where(x => x.Value.Any(y => y.Date == itemDate))
                .SelectMany(p => p.Value)
                .Where(c => c.Date == itemDate)
                .Select(x => x);

                float countValid = lEntityOfDate.Count();
                var countRateValid = lEntityOfDate.Count(x => x.Ma20 > 0 && x.Close >= (decimal)x.Ma20);
                var itemStastitic = (itemDate, Math.Round(countRateValid / countValid, 2));
                lStastitic.Add(itemStastitic);
            }

            foreach (var item in lStastitic)
            {
                Console.WriteLine($"{item.Item1.ToString("dd/MM/yyyy")}: {item.Item2}");
            }
        }

        static int SoLanVaoLenh = 0;
        static decimal TongTakeProfit = 0;
        private static void PrintTakeProfit(Quote buy, Quote sell)
        {
            var rate = Math.Round((sell.Close - buy.Open) * 100 / buy.Open, 1);
            var num = (sell.Date - buy.Date).TotalDays;
            SoLanVaoLenh++;
            TongTakeProfit += rate;

            Console.WriteLine($"Mua: {buy.Date.ToString("dd/MM/yyyy")}|Ban: {sell.Date.ToString("dd/MM/yyyy")}|So Nen: {num}| Ti le: {rate}%");
        }
        private static void PrintTotal()
        {
            Console.WriteLine($"So lan vao lenh: {SoLanVaoLenh}| Tong: {TongTakeProfit}%");
        }


        public static void BuySellByMa20(string code)
        {
            try
            {
                var lData = sqliteDayDB.GetData(code).Select(x => new Quote
                {
                    Date = x.t.UnixTimeStampToDateTime(),
                    Open = x.o,
                    Close = x.c,
                    High = x.h,
                    Low = x.l,
                    Volume = x.v
                }).ToList();
                var lMa20 = lData.GetSma(20);
                var count = lData.Count();
                var mode = EModeBuySell.NoAction;
                Quote itemBuy = null;
                for (int i = 0; i < count; i++)
                {
                    var item = lData.ElementAt(i);
                    var itemMa20 = lMa20.ElementAt(i).Sma;
                    if (itemMa20 is null)
                        continue;
                    if (mode == EModeBuySell.NoAction)
                    {
                        if(item.Close < (decimal)itemMa20) 
                            mode = EModeBuySell.Listen;
                        continue;
                    }

                    if (mode == EModeBuySell.Listen)
                    {
                        if (item.Close > (decimal)itemMa20)
                        {
                            mode = EModeBuySell.Buy;
                            itemBuy = item;
                        }
                    }

                    if(mode == EModeBuySell.Buy)
                    {
                        if(item.Close < (decimal)itemMa20)
                        {
                            mode = EModeBuySell.Listen;
                            PrintTakeProfit(itemBuy, item);
                        }
                    }

                }
                PrintTotal();
            }
            catch(Exception ex) 
            { 
                Console.WriteLine (ex.Message );
            }
        }
    }
}
