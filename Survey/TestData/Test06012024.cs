﻿using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Survey.Models;
using Skender.Stock.Indicators;

namespace Survey.TestData
{
    public class Test06012024
    {
        public static void MainFunc()
        {
            Console.WriteLine("Coin, Rate Max, Num Max, Rate Min, Num Min");
            //var lCoin = Data.GetCoins(10);
            var lCoin = new List<TicketModel> { new TicketModel { symbol = "twtusdt" } };
            foreach (var item in lCoin)
            {
                Handle(item.symbol.ToLower());
                Thread.Sleep(500);
            }
            //Print();
        }

        private static bool CheckRSI(RsiResult param)
        {
            return (param.Rsi < 32);
        }

        private static bool CheckPhanKy(IEnumerable<QuoteEx> lData, int index, IEnumerable<RsiResult> lRsi)
        {
            if (index < 10)
                return false;

            var element = lData.ElementAt(index);
            var element_1 = lData.ElementAt(index - 1);
            if (element.Close <= element.Open
                || element_1.Close >= element_1.Open)
                return false;

            var botVal = element_1.Close;
            var rsi_1 = lRsi.ElementAt(index - 1);
            if (rsi_1.Rsi > 40)
                return false;

            var flagTop = false;
            RsiResult rsiCheck = null;
            decimal valCheck = 0;

            var indexCount = 10;
            for (int i = index - 2; i > 0; i--)
            {
                indexCount--;
                if (indexCount <= 0)
                    return false;

                var itemRSI = lRsi.ElementAt(i);
                if(flagTop)
                {
                    if(itemRSI.Rsi < rsiCheck.Rsi)
                    {
                        rsiCheck = itemRSI;
                        valCheck = lData.ElementAt(i).Close;
                        continue;
                    }    

                    if(itemRSI.Rsi > rsiCheck.Rsi)
                    {
                        //check phân kỳ dương
                        if (rsiCheck.Rsi < rsi_1.Rsi
                            && valCheck > botVal)
                        {
                            var print = lData.ElementAt(i);
                            Console.WriteLine($"PRINT: {print.Date.ToString("dd/MM/yyyy HH")}");
                            return true;
                        }
                    }

                    continue;
                }    

                if(itemRSI.Rsi > rsi_1.Rsi)
                {
                    flagTop = true;
                    rsiCheck = itemRSI;
                    valCheck = lData.ElementAt(i).Close;
                    continue;
                }    
            }

            return false;
        }

        public static void Handle(string coin)
        {
            var lDataQuote = Data.GetData(coin, EInterval.I4H).Select(x => x.To<QuoteEx>());
            var count = lDataQuote.Count();
            var lRsi = lDataQuote.GetRsi();
            var lBB = lDataQuote.GetBollingerBands();

            //Giữ tối đa 30 nến = 5 ngày
            var lSave = new List<QuoteEx>();
            bool hasBuy = false;
            var index = 0;
            decimal minVal = 0, maxVal = 0;
            int indexMin = 0, indexMax = 0;
            var flagFollow = false;
            QuoteEx elementPrev = null;
            for (int i = 0; i < count; i++)
            {
                var item = lDataQuote.ElementAt(i);
                var rsi = lRsi.ElementAt(i);
                if (rsi.Rsi is null || rsi.Rsi <= 0)
                    continue;

                var phanKy = CheckPhanKy(lDataQuote, i, lRsi);
                if(phanKy)
                {
                    Console.WriteLine($"{item.Date.ToString("dd/MM/yyyy HH")}|RSI: {rsi.Rsi}");
                }

                //if (i > 0)
                //{
                //    elementPrev = lDataQuote.ElementAt(i - 1);
                //}



                //var tmp = CheckRSI(rsi);
                //if (tmp)
                //{
                //    Console.WriteLine($"{item.Date.ToString("dd/MM/yyyy HH")}|RSI: {rsi.Rsi}");
                //}


                //if (hasBuy)
                //{
                //    index++;
                //    if (item.Close > maxVal)
                //    {
                //        maxVal = item.Close;
                //        indexMax = index;
                //    }

                //    if (item.Close < minVal)
                //    {
                //        minVal = item.Close;
                //        indexMin = index;
                //    }


                //    if (index >= 90)
                //    {
                //        lSave.Last().Max = maxVal;
                //        lSave.Last().Min = minVal;
                //        lSave.Last().SoNenViTriMax = indexMax;
                //        lSave.Last().SoNenViTriMin = indexMin;
                //        lSave.Last().PhanTramMax = 100 * (-1 + Math.Round(maxVal / lSave.Last().Close, 2));
                //        lSave.Last().PhanTramMin = 100 * (-1 + Math.Round(minVal / lSave.Last().Close, 2));

                //        hasBuy = false;
                //        Print(lSave.Last());
                //    }

                //    continue;
                //}

                //if (!flagFollow)
                //{
                //    var chkRSI = CheckRSI(rsi);
                //    if (chkRSI)
                //    {
                //        flagFollow = true;
                //    }
                //    continue;
                //}

                //if (item.Close > item.Open
                //    && item.Volume > 100000)//60k?
                //{
                //    hasBuy = true;
                //    flagFollow = false;

                //    item.Coin = coin;

                //    minVal = item.Close;
                //    maxVal = item.Close;
                //    index = 0;
                //    indexMin = index;
                //    indexMax = index;

                //    var itemPrev1 = lDataQuote.ElementAt(i - 1);
                //    var itemPrev2 = lDataQuote.ElementAt(i - 2);
                //    var itemPrev3 = lDataQuote.ElementAt(i - 3);
                //    var High1 = itemPrev1.Close > itemPrev1.Open ? itemPrev1.Close : itemPrev1.Open;
                //    var High2 = itemPrev2.Close > itemPrev2.Open ? itemPrev2.Close : itemPrev2.Open;
                //    var High3 = itemPrev3.Close > itemPrev3.Open ? itemPrev3.Close : itemPrev3.Open;

                //    var Low1 = itemPrev1.Close > itemPrev1.Open ? itemPrev1.Open : itemPrev1.Close;
                //    var Low2 = itemPrev2.Close > itemPrev2.Open ? itemPrev2.Open : itemPrev2.Close;
                //    var Low3 = itemPrev3.Close > itemPrev3.Open ? itemPrev3.Open : itemPrev3.Close;

                //    var High23 = High2 > High3 ? High2 : High3;
                //    var Low23 = Low2 > Low3 ? Low3 : Low2;

                //    var Top = High1 > High23 ? High1 : High23;
                //    var Bot = Low1 > Low23 ? Low23 : Low1;

                //    item.DownRate = Math.Round((Top - Bot) * 100 / Bot, 2);
                //    item.SignalRate = Math.Round((item.Close - item.Open) * 100 / item.Open, 2);
                //    //item.Date = item.Date.AddHours(-7);
                //    lSave.Add(item);
                //}
            }

            //if (hasBuy)
            //{
            //    lSave.Last().Max = maxVal;
            //    lSave.Last().SoNenViTriMax = indexMax;
            //    lSave.Last().PhanTramMax = 100 * (-1 + Math.Round(maxVal / lSave.Last().Close, 2));
            //    lSave.Last().Min = minVal;
            //    lSave.Last().SoNenViTriMin = indexMin;
            //    lSave.Last().PhanTramMin = 100 * (-1 + Math.Round(minVal / lSave.Last().Close, 2));
            //    Print(lSave.Last());
            //}
        }

        private static void Print(QuoteEx item)
        {
            var formatCoin = item.Coin.Replace("USDT", "_USDT");
            var settings = 0.LoadJsonFile<AppsettingModel>("appsettings");
            var link = $"{settings.ViewWeb.Single}/{formatCoin}";
            var print = $"{item.Coin},{item.Date.ToString("dd/MM/yyyy HH")},{item.DownRate}%,{item.SignalRate}%,{item.PhanTramMax}%,{item.SoNenViTriMax},{item.PhanTramMin}%,{item.SoNenViTriMin},{item.Volume},{link}";
            Console.WriteLine(print);
        }
    }
}
