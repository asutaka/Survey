using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task BatDay(string code)
        {
            try
            {
                _code = code;
                var lByBit = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Linear, code, Bybit.Net.Enums.KlineInterval.FourHours, null, null, 1000);
                var lData = lByBit.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                lData.Reverse();

                BatDay(lData, 7);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.BatDay|EXCEPTION| {ex.Message}");
            }
        }

        private void BatDay(List<Quote> lData, decimal a)
        {
            try
            {
                var count = lData.Count();
                var lbb = lData.GetBollingerBands();
                var lVol = new List<Quote>();
                foreach (var item in lData) 
                {
                    lVol.Add(new Quote { Date = item.Date, Close = item.Volume, Open = item.Open, High = item.High, Low = item.Low, Volume = item.Volume });
                }
                var lMaVol = lVol.GetSma(20);
                var isBuy = false;
                Quote itemFlag = null;

                Quote itemHold = null;
                decimal entryHold = 0;
                var isHold = false;
                var indexHold = -1;
                decimal sl = 0;

                for (int i = 50; i < count; i++)
                {
                    var item = lData[i];   
                    var bb = lbb.ElementAt(i);
                    var itemVol = lMaVol.ElementAt(i);

                    //if (isHold)
                    //{
                    //    var rate = Math.Round(100 * (-1 + item.Close / entryHold), 1);
                    //    if (i - indexHold >= 20) 
                    //    {
                    //        var tp = Math.Round(100 * (-1 + item.Close / itemHold.Close), 1);
                    //        Console.WriteLine($"BUY: {itemHold.Date.ToString($"dd/MM/yyyy HH")}|SELL: {item.Date.ToString($"dd/MM/yyyy HH")}| TP: {tp}");
                    //        itemHold = null;
                    //        entryHold = 0;
                    //        isHold = false;
                    //        indexHold = -1;
                    //    }
                    //    if(item.Low <= sl)
                    //    {
                    //        var tp = Math.Round(100 * (-1 + sl / itemHold.Close), 1);
                    //        Console.WriteLine($"BUY: {itemHold.Date.ToString($"dd/MM/yyyy HH")}|SELL: {item.Date.ToString($"dd/MM/yyyy HH")}| TP: {tp}");
                    //        itemHold = null;
                    //        entryHold = 0;
                    //        isHold = false;
                    //        indexHold = -1;
                    //    }
                    //    if(item.Close < item.Open && item.Volume > (decimal)(itemVol.Sma * 1.5))
                    //    {
                    //        var tp = Math.Round(100 * (-1 + item.Close / itemHold.Close), 1);
                    //        Console.WriteLine($"BUY: {itemHold.Date.ToString($"dd/MM/yyyy HH")}|SELL: {item.Date.ToString($"dd/MM/yyyy HH")}| TP: {tp}");
                    //        itemHold = null;
                    //        entryHold = 0;
                    //        isHold = false;
                    //        indexHold = -1;
                    //    }

                    //    if(rate >= 10)
                    //    {
                    //        var real = 1 - Math.Ceiling(rate / 2) / 100;
                    //        var slcur = item.High * real;
                    //        if(slcur > sl)
                    //        {
                    //            sl = slcur;
                    //        }
                    //    }

                    //    continue;
                    //}

                    //if(isBuy)
                    //{
                    //    isBuy = false;
                    //    var entry = (decimal)0.6 * itemFlag.Close + (decimal)0.4 * itemFlag.Open;
                    //    if(item.Low <= entry)
                    //    {
                    //        itemHold = item;
                    //        entryHold = entry;
                    //        isHold = true;
                    //        indexHold = i;
                    //        sl = entry * (decimal)0.7;
                    //        //Console.WriteLine($"{item.Date.ToString($"dd/MM/yyyy HH")}|Val: {entry}");
                    //    }
                    //    itemFlag = null;
                    //    continue;
                    //}

                    if (item.Close < item.Open * (decimal)1.01
                        //|| item.Volume <= (decimal)(itemVol.Sma * 1.5)
                        || item.Low >= (decimal)bb.LowerBand
                        || item.Close >= (decimal)bb.Sma
                        //|| (item.High - item.Low) * 2 > (decimal)(bb.UpperBand - bb.LowerBand)
                        ) 
                        continue;
                    isBuy = true;
                    itemFlag = item;

                    Console.WriteLine($"{item.Date.ToString($"dd/MM/yyyy HH")}|Vol: {item.Volume}|MA: {itemVol.Sma}");

                    //var buy = (3 * item.High + item.Low) / 4;
                    //if(item.Open - item.Low >= 4*(item.High - item.Close))
                    //{
                    //    buy = (3 * item.High + item.Open) / 4;
                    //}
                    //Console.WriteLine(item.Date.ToString($"dd/MM/yyyy HH|{buy}"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyCoinDanZagerCustom|EXCEPTION| {ex.Message}");
            }
        }

        //private void BatDay(List<Quote> lData, decimal a)
        //{
        //    try
        //    {
        //        var count = lData.Count();
        //        var lbb = lData.GetBollingerBands();
        //        var lVol = new List<Quote>();
        //        foreach (var item in lData)
        //        {
        //            lVol.Add(new Quote { Date = item.Date, Close = item.Volume, Open = item.Open, High = item.High, Low = item.Low, Volume = item.Volume });
        //        }
        //        var lMaVol = lVol.GetSma(20);

        //        for (int i = 50; i < count; i++)
        //        {
        //            var item = lData[i];
        //            var bb = lbb.ElementAt(i);
        //            var itemVol = lMaVol.ElementAt(i);

        //            if (item.Close < item.Open * (decimal)1.01
        //                || item.Volume <= (decimal)(itemVol.Sma * 1.5)
        //                || item.Low >= (decimal)bb.LowerBand
        //                || item.Close >= (decimal)bb.Sma)
        //                continue;

        //            Console.WriteLine($"{item.Date.ToString($"dd/MM/yyyy HH")}|Vol: {item.Volume}|MA: {itemVol.Sma}");

        //            //var buy = (3 * item.High + item.Low) / 4;
        //            //if(item.Open - item.Low >= 4*(item.High - item.Close))
        //            //{
        //            //    buy = (3 * item.High + item.Open) / 4;
        //            //}
        //            //Console.WriteLine(item.Date.ToString($"dd/MM/yyyy HH|{buy}"));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"PartternService.SurveyCoinDanZagerCustom|EXCEPTION| {ex.Message}");
        //    }
        //}
    }
}
