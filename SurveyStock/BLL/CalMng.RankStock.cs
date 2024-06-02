using Skender.Stock.Indicators;
using SurveyStock.DAL;
using SurveyStock.Model;
using SurveyStock.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyStock.BLL
{
    public static partial class CalMng
    {
        private static Dictionary<string, decimal> _dicRate = new Dictionary<string, decimal>();
        public static void RankStock(EmaType type, int period)
        {
            _dicRate.Clear();
            int _numThread = 20;
            var lData = sqliteComDB.GetData_Company();
            Parallel.ForEach(lData, new ParallelOptions { MaxDegreeOfParallelism = _numThread },
                item =>
                {
                    CalculateResult(item.stock_code, type, period);
                });
            var dicSort = _dicRate.OrderByDescending(x => x.Value);
            foreach (var item in dicSort)
            {
                Console.WriteLine($"{item.Key}: {Math.Round(item.Value, 1)}");
            }
        }
       
        private static void CalculateResult(string code, EmaType type, int period = 20)
        {
            if (type == EmaType.MA)
            {
                BuySellMa(code, period);
            }
            else 
            {
                BuySellEMa(code, period);
            }
        }
        private static void BuySellMa(string code, int period = 20)
        {
            var dic = new Dictionary<DateTime, decimal>();
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

                var lMa20 = lData.GetSma(period);
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
                        if (item.Close < (decimal)itemMa20)
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

                    if (mode == EModeBuySell.Buy)
                    {
                        if (item.Close < (decimal)itemMa20)
                        {
                            mode = EModeBuySell.Listen;
                            TakeProfit(itemBuy, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BuySellMa|EXCEPTION| {ex.Message}");
            }
            //
            var group = dic.GroupBy(x => x.Key.Year).Select(y => new { Year = y.Key, Rate = y.Sum(z => z.Value) });
            var avg = group.Any() ? Math.Round(group.Average(x => x.Rate),2) : 0;
            _dicRate.Add(code, avg);

            void TakeProfit(Quote buy, Quote sell)
            {
                try
                {
                    var rate = buy.Open <= 0 ? 0 : Math.Round((sell.Close - buy.Open) * 100 / buy.Open, 1);
                    dic.Add(buy.Date, rate);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"BuySellMa.TakeProfit|EXCEPTION| {ex.Message}");
                }
            }
        }

        private static void BuySellEMa(string code, int period = 20)
        {
            var dic = new Dictionary<DateTime, decimal>();
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

                var lMa20 = lData.GetEma(period);
                var count = lData.Count();
                var mode = EModeBuySell.NoAction;
                Quote itemBuy = null;
                for (int i = 0; i < count; i++)
                {
                    var item = lData.ElementAt(i);
                    var itemMa20 = lMa20.ElementAt(i).Ema;
                    if (itemMa20 is null)
                        continue;
                    if (mode == EModeBuySell.NoAction)
                    {
                        if (item.Close < (decimal)itemMa20)
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

                    if (mode == EModeBuySell.Buy)
                    {
                        if (item.Close < (decimal)itemMa20)
                        {
                            mode = EModeBuySell.Listen;
                            TakeProfit(itemBuy, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BuySellEMa|EXCEPTION| {ex.Message}");
            }
            //
            var group = dic.GroupBy(x => x.Key.Year).Select(y => new { Year = y.Key, Rate = y.Average(z => z.Value) });
            var avg = group.Any() ? group.Average(x => x.Rate) : 0;
            _dicRate.Add(code, avg);

            void TakeProfit(Quote buy, Quote sell)
            {
                try
                {
                    var rate = Math.Round((sell.Close - buy.Open) * 100 / buy.Open, 1);
                    dic.Add(buy.Date, rate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"BuySellEMa.TakeProfit|EXCEPTION| {ex.Message}");
                }
            }
        }
    }
}
