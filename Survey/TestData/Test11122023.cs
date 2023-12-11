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
    public class Test11122023
    {
        public static void Test1()
        {
            //var lData = Data.GetData("axsusdt", EInterval.I15M);
            var lData = Data.GetData("axsusdt", EInterval.I1H);
            var lDataQuote = lData.Select(x => x.To<Quote>());
            var count = lData.Count();
            var lIchimoku = lDataQuote.GetIchimoku();
            var lMa20 = CalculateMng.MA(lData.Select(x => x.Volume).ToArray(), TicTacTec.TA.Library.Core.MAType.Sma, 20, count);
            
            var lBuy = new List<FinancialDataPoint>();
            var lSell = new List<FinancialDataPoint>();
            var state = true;//true: chờ, false: buy
            var stateBuy = false;
            var stateSell = false;
            for (int i = 0; i < count; i++)
            {
                var item = lData.ElementAt(i);
                var ichimoku = lIchimoku.ElementAt(i);
                var ma20 = lMa20.ElementAt(i);
                
                if(stateBuy)
                {
                    lBuy.Add(item);
                    stateBuy = false;
                    state = false;
                    continue;
                }
                if (stateSell)
                {
                    lSell.Add(item);
                    stateSell = false;
                    state = true;
                    continue;
                }

                if(state)
                {
                    var check = Check2Buy(item, ichimoku, 5);
                    if (check)
                    {
                        stateBuy = true;
                    }
                }
                else 
                {
                    var check = Check2Sell(item, ichimoku, ma20,4);
                    if (check)
                    {
                        stateSell = true;
                    }
                }
            }
            //print
            var countResult = lSell.Count();
            var lRate = new List<double>();
            for (int i = 0; i < countResult; i = i + 2)
            {
                var itemBuy = lBuy.ElementAt(i);
                var itemSell = lSell.ElementAt(i);
                var PriceBuy = itemBuy.Open;
                var PriceSell = (itemSell.Open + itemSell.Close) / 2;
                var rate = Math.Round((PriceSell - PriceBuy) * 100 / PriceBuy, 2);
                lRate.Add(rate);

                var print = $"BUY: {itemBuy.DateTimeStamp.ToString("dd/MM/yyyy HH:mm")};PRICE_B: {PriceBuy} | SELL: {itemSell.DateTimeStamp.ToString("dd/MM/yyyy HH:mm")}; PRICE_S: {PriceSell} | RATE: {rate}%";
                Console.WriteLine(print);
            }
            Console.WriteLine($"COUNT: {countResult}");
            Console.WriteLine($"SUM: {lRate.Average()}");
            var tmp9 = 1;
        }
        /*
         * check nến vừa cắt qua mây ichimoku thì mua ở giá mở cửa của nến tiếp theo
            case 1: nến H vượt mây
            case 2: nến C vượt mây 
            case 3: nến L vượt mây
            case 4: nến H vượt mây và nến xanh
            case 5: nến C vượt mây và nến xanh
            case 6: nến L vượt mây và nến xanh
         */
        public static bool Check2Buy(FinancialDataPoint item, IchimokuResult itemIchi, int mode = 1)
        {
            var ichiTop = itemIchi.SenkouSpanA > itemIchi.SenkouSpanB ? itemIchi.SenkouSpanA : itemIchi.SenkouSpanB;
            if (ichiTop is null)
                return false;
            //case 1
            if(item.High > (double)ichiTop && mode == 1)
            {
                return true;
            }

            //case 2
            if (item.Close > (double)ichiTop && mode == 2)
            {
                return true;
            }

            //case 3
            if (item.Low > (double)ichiTop && mode == 3)
            {
                return true;
            }

            //case 4
            if (item.High > (double)ichiTop
                && item.Close >= item.Open && mode == 4)
            {
                return true;
            }

            //case 5
            if (item.Close > (double)ichiTop
                && item.Close >= item.Open && mode == 5)
            {
                return true;
            }

            //case 6
            if (item.Low > (double)ichiTop
                && item.Close >= item.Open && mode == 6)
            {
                return true;
            }

            return false;
        }
        /*
         * bán khi giá đóng cửa nhỏ hơn ma20 thì bán ở giá trung bình Open-close của nến tiếp theo
            case 1: giá H nhỏ hơn ma20
            case 2: giá C nhỏ hơn ma20 
            case 3: giá L nhỏ hơn ma20
            case 4: giá H nhỏ hơn cận trên mây
            case 5: giá C nhỏ hơn cận trên mây 
            case 6: giá L nhỏ hơn cận trên mây
         */
        public static bool Check2Sell(FinancialDataPoint item, IchimokuResult itemIchi, double itemMA, int mode = 1)
        {

            //Case 1
            if (item.High < itemMA && mode == 1)
            {
                return true;
            }
            //Case 2
            if (item.Close < itemMA && mode == 2)
            {
                return true;
            }
            //Case 3
            if (item.Low < itemMA && mode == 3)
            {
                return true;
            }


            var ichiTop = itemIchi.SenkouSpanA > itemIchi.SenkouSpanB ? itemIchi.SenkouSpanA : itemIchi.SenkouSpanB;
            if (ichiTop is null)
                return false;
            //Case 4
            if (item.High < (double)ichiTop && mode == 4)
            {
                return true;
            }
            //Case 5
            if (item.Close < (double)ichiTop && mode == 5)
            {
                return true;
            }
            //Case 6
            if (item.Low < (double)ichiTop && mode == 6)
            {
                return true;
            }
            return false;
        }
    }
}
