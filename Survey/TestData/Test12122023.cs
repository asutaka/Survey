using Survey.Models;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skender.Stock.Indicators;

namespace Survey.TestData
{
    public class Test12122023
    {
        public static void Test1()
        {
            //var lData = Data.GetData("axsusdt", EInterval.I15M);
            var lData = Data.GetData("1inchusdt", EInterval.I1H);
            var lDataQuote = lData.Select(x => x.To<Quote>());
            var count = lData.Count();
            var lIchimoku = lDataQuote.GetIchimoku();
            var lMa20 = CalculateMng.MA(lData.Select(x => x.Volume).ToArray(), TicTacTec.TA.Library.Core.MAType.Sma, 20, count);
            var lRSI = lDataQuote.GetRsi();
            
            var lBuy = new List<FinancialDataPoint>();
            var lSell = new List<FinancialDataPoint>();
            var state = true;//true: chờ, false: buy
            var stateBuy = false;
            var stateSell = false;

            FinancialDataPoint itemBase = null;
            for (int i = 0; i < count; i++)
            {
                FinancialDataPoint itemPrevious = null;
                if(i > 0)
                {
                    itemPrevious = lData.ElementAt(i - 1);
                }    
                
                var item = lData.ElementAt(i);
                var ichimoku = lIchimoku.ElementAt(i);
                var ma20 = lMa20.ElementAt(i);
                var rsi = lRSI.ElementAt(i)?.Rsi ?? 0;
                
                if(stateBuy)
                {
                    itemBase = item;
                    lBuy.Add(item);
                    stateBuy = false;
                    state = false;
                }

                if (stateSell)
                {
                    item.Description = des;
                    des = string.Empty;
                    lSell.Add(item);
                    stateSell = false;
                    state = true;
                    continue;
                }

                if (state)
                {
                    var check = Check2Buy(item, ichimoku, rsi);
                    if (check)
                    {
                        stateBuy = true;
                    }
                }
                else 
                {
                    var check = Check2Sell(item, ichimoku, rsi);
                    if (check)
                    {
                        stateSell = true;
                    }
                }
            }
            //print
            var countResult = lSell.Count();
            var lRate = new List<double>();
            for (int i = 0; i < countResult; i++)
            {
                var itemBuy = lBuy.ElementAt(i);
                var itemSell = lSell.ElementAt(i);
                var PriceBuy = itemBuy.Open;
                var PriceSell = itemSell.Open;
                var rate = Math.Round((PriceSell - PriceBuy) * 100 / PriceBuy, 2);
                lRate.Add(rate);

                var print = $"BUY: {itemBuy.DateTimeStamp.ToString("dd/MM/yyyy HH:mm")};PRICE_B: {PriceBuy} | SELL: {itemSell.DateTimeStamp.ToString("dd/MM/yyyy HH:mm")}; DES: {itemSell.Description}; PRICE_S: {PriceSell} | RATE: {rate}%";
                Console.WriteLine(print);
            }
            Console.WriteLine($"COUNT: {countResult}");
            Console.WriteLine($"SUM: {lRate.Average()}");
        }
        /*
         * check nến vừa cắt qua mây ichimoku thì mua ở giá mở cửa của nến tiếp theo
            case 1: nến H vượt mây
            case 2: nến C vượt mây 
            case 3: nến L vượt mây
            case 4: nến H vượt mây và nến xanh
            case 5: nến C vượt mây và nến xanh(tỉ lệ đang tốt nhất)
            case 6: nến L vượt mây và nến xanh
         */
       
        static FinancialDataPoint entitySignalBase = null;
        public static bool Check2Buy(FinancialDataPoint item, IchimokuResult itemIchi, double rsi)
        {
            var ichiTop = itemIchi.SenkouSpanA > itemIchi.SenkouSpanB ? itemIchi.SenkouSpanA : itemIchi.SenkouSpanB;
            var ichiBot = itemIchi.SenkouSpanA < itemIchi.SenkouSpanB ? itemIchi.SenkouSpanA : itemIchi.SenkouSpanB;
            if (ichiTop is null)
                return false;

            //case 5
            if (item.Close > (double)ichiTop && item.Low < (double)ichiTop && item.Close > item.Open && item.Low > (double)ichiBot)
            {
                entitySignalBase = item;
                return true;
            }

            return false;
        }
        /*
         * bán khi giá đóng cửa nhỏ hơn ma20 thì bán ở giá trung bình Open-close của nến tiếp theo
            case 1: giá H nhỏ hơn ma20
            case 2: giá C nhỏ hơn ma20 
            case 3: giá L nhỏ hơn ma20
            case 4: giá H nhỏ hơn cận trên mây(tỉ lệ đang tốt nhất)
            case 5: giá C nhỏ hơn cận trên mây 
            case 6: giá L nhỏ hơn cận trên mây
         */
        static FinancialDataPoint entitySignal1 = null;
        static FinancialDataPoint entitySignal2 = null;
        static bool step1 = false;
        static bool step2 = false;
        static bool step3 = false;
        static string des = string.Empty;

        public static bool Check2Sell(FinancialDataPoint item, IchimokuResult itemIchi, double rsi)
        {
            var ichiTop = itemIchi.SenkouSpanA > itemIchi.SenkouSpanB ? itemIchi.SenkouSpanA : itemIchi.SenkouSpanB;
            if (ichiTop is null)
            {
                des = "case1";
                Reset();
                return true;
            }    

            if (item.Volume > entitySignalBase.Volume * 7
                && rsi > 70)
            {
                des = "case2";
                Reset();
                return true;
            }

            if(step3)
            {
                //TH lý tưởng
                if(item.High < (double)ichiTop)
                {
                    Reset();
                    des = "case3";
                    return true;
                }
            }    

            if (step2)
            {
                if (item.Low > entitySignal2.Low && item.Close > item.Open)
                {
                    step3 = true;
                    return false;
                }
                Reset();
                des = "case4";
                return true;
            }

            if(step1)
            {
                if (item.Low > entitySignal1.Low && item.Close > item.Open)
                {
                    entitySignal2 = item;
                    step2 = true;
                    return false;
                }
                Reset();
                des = "case5";
                return true;
            }

            if (item.Low > (double)ichiTop
                && item.Low > entitySignalBase.Low
                && item.Close > item.Open)
            {
                step1 = true;
                entitySignal1 = item;
                return false;
            }


            Reset();
            des = "case6";
            return true;
        }

        public static void Reset()
        {
            step1 = false;
            step2 = false;
            step3 = false;
            entitySignalBase = null;
            entitySignal1 = null;
            entitySignal2 = null;
        }
    }
}
