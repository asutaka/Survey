using Skender.Stock.Indicators;
using Survey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Survey.Utils
{
    public static class Analyze
    {
        public static List<TraceViewModel> AnalyzeViaIchimoku()
        {
            var dicOutput = new Dictionary<string, IEnumerable<FinancialDataPoint>>();
            //150 coin 
            var lCoin = Data.GetCoins(150);
            //data of 150
            StaticVal._strProgressMain = EProgress.GetData.GetDisplayName();
            var dicData = new Dictionary<string, IEnumerable<FinancialDataPoint>>();
            foreach (var item in lCoin)
            {
                var lData = Data.GetData(item.symbol, EInterval.I1H);
                var count = lData.Count();
                if (count >= 200)
                {
                    dicData.Add(item.symbol, lData);
                }
                Thread.Sleep(500);
            }

            //ichimoku
            StaticVal._strProgressMain = EProgress.Analyze.GetDisplayName();
            foreach (var item in dicData)
            {
                var lData = item.Value;
                var lDataQuote = lData.Select(x => x.To<Quote>());
                var count = lDataQuote.Count();
                var lIchimoku = lDataQuote.GetIchimoku();
                var last = lDataQuote.Last();

                if(last.Close > lIchimoku.Last().SenkouSpanA
                    && last.Close > lIchimoku.Last().SenkouSpanB)
                {
                    if (PassCondition(lDataQuote.ElementAt(count - 1), lIchimoku.ElementAt(count - 1))
                    || PassCondition(lDataQuote.ElementAt(count - 2), lIchimoku.ElementAt(count - 2))
                    || PassCondition(lDataQuote.ElementAt(count - 3), lIchimoku.ElementAt(count - 3)))
                    {
                        dicOutput.Add(item.Key, item.Value);
                    }
                }
            }

            var lResult = new List<TraceViewModel>();
            foreach (var item in dicOutput)
            {
                var lData = item.Value;
                var lDataQuote = lData.Select(x => x.To<Quote>());
                var lRsi = lDataQuote.GetRsi();
                var lMACD = lDataQuote.GetMacd();
                var macd = lMACD.Last();
                lResult.Add(new TraceViewModel
                {
                    Coin = item.Key,
                    RSI = lRsi.Last().Rsi ?? 0,
                    MACD = GetMACDStr(macd)
                }); 
            }
            var index = 1;
            foreach (var item in lResult)
            {
                item.STT = index++;
            }
            StaticVal._strProgressMain = EProgress.Idle.GetDisplayName();
            return lResult;
        }

        public static List<TraceViewModel> AnalyzeViaRSI()
        {
            var dicOutput = new Dictionary<string, IEnumerable<FinancialDataPoint>>();
            //150 coin 
            var lCoin = Data.GetCoins(150);
            //data of 150
            StaticVal._strProgressMain = EProgress.GetData.GetDisplayName();
            var dicData = new Dictionary<string, IEnumerable<FinancialDataPoint>>();
            foreach (var item in lCoin)
            {
                var lData = Data.GetData(item.symbol, EInterval.I4H);
                var count = lData.Count();
                if (count >= 50)
                {
                    dicData.Add(item.symbol, lData);
                }
                Thread.Sleep(500);
            }

            //RSI
            StaticVal._strProgressMain = EProgress.Analyze.GetDisplayName();
            foreach (var item in dicData)
            {
                var lData = item.Value;
                var lDataQuote = lData.Select(x => x.To<Quote>());
                var count = lDataQuote.Count();
                var lRSI = lDataQuote.GetRsi();
                var last = lDataQuote.Last();
                var rsi = lRSI.ElementAt(count - 2);
                var downRate = 100 * (-1 + Math.Round(last.Open / last.Close, 2));

                if (downRate > 50
                    || (last.Close > last.Open
                    && (rsi.Rsi??0) < 32))
                {
                    dicOutput.Add(item.Key, item.Value);
                }
            }

            var lResult = new List<TraceViewModel>();
            foreach (var item in dicOutput)
            {
                var lData = item.Value;
                var lDataQuote = lData.Select(x => x.To<Quote>());
                var lRsi = lDataQuote.GetRsi();
                var lMACD = lDataQuote.GetMacd();
                var macd = lMACD.Last();
                lResult.Add(new TraceViewModel
                {
                    Coin = item.Key,
                    RSI = lRsi.Last().Rsi ?? 0,
                    MACD = GetMACDStr(macd)
                });
            }
            var index = 1;
            foreach (var item in lResult)
            {
                item.STT = index++;
            }
            StaticVal._strProgressMain = EProgress.Idle.GetDisplayName();
            return lResult;
        }

        #region AnalyzeViaIchimoku
        private static string GetMACDStr(MacdResult macd)
        {
            var result = string.Empty;
            result += macd.Histogram > 0 ? "MACD cắt lên" : "MACD cắt xuống";
            return result;
        }
        private static bool PassCondition(Quote item, IchimokuResult ichimoku)
        {
            var spanTop = ichimoku.SenkouSpanA > ichimoku.SenkouSpanB ? ichimoku.SenkouSpanA : ichimoku.SenkouSpanB;

            if (item.Close > spanTop
                && spanTop > item.Open)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
