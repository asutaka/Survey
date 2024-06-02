using Skender.Stock.Indicators;
using SurveyStock.DAL;
using SurveyStock.Util;
using System;
using System.Linq;

namespace SurveyStock.BLL
{
    public static partial class CalMng
    {
        static int SoLanVaoLenh = 0;
        static decimal TongTakeProfit = 0;
        static double TongSoNgayNamGiu = 0;
        private static void PrintTakeProfit(Quote buy, Quote sell)
        {
            var rate = Math.Round((sell.Close - buy.Open) * 100 / buy.Open, 1);
            var num = (sell.Date - buy.Date).TotalDays;
            SoLanVaoLenh++;
            TongTakeProfit += rate;
            TongSoNgayNamGiu += num;

            Console.WriteLine($"Mua: {buy.Date.ToString("dd/MM/yyyy")}|Ban: {sell.Date.ToString("dd/MM/yyyy")}|So Nen: {num}| Ti le: {rate}%");
        }
        
        private static void PrintTotal()
        {
            Console.WriteLine($"So lan vao lenh: {SoLanVaoLenh}\n" +
                              $"Tong loi nhuan: {TongTakeProfit}%\n" +
                              $"Trung binh 1 lan vào lenh: {Math.Round(TongTakeProfit / SoLanVaoLenh, 2)}%\n" +
                              $"So nen nam giu TB: {Math.Round(TongSoNgayNamGiu / SoLanVaoLenh, 1)}");
        }

        public static void BuySellByMa(string code, int period = 20)
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

        public static void BuySellByEMa(string code, int period = 20)
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
                            PrintTakeProfit(itemBuy, item);
                        }
                    }
                }
                PrintTotal();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
