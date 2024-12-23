﻿using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyCoinDanZagerCustom(string code)
        {
            try
            {
                _code = code;
                var lByBit = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Spot, code, Bybit.Net.Enums.KlineInterval.FourHours, null, null, 1000);
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

                SurveyCoinDanZagerCustom(lData, 7);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyCoinDanZagerCustom|EXCEPTION| {ex.Message}");
            }
        }

        private void SurveyCoinDanZagerCustom(List<Quote> lData, decimal a)
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

                for (int i = 50; i < count; i++)
                {
                    var item = lData[i];
                    if(item.Date.Month == 9 && item.Date.Day == 6 && item.Date.Hour == 20)
                    {
                        var tmp = 1;
                    }    
                    var bb = lbb.ElementAt(i);
                    var itemVol = lMaVol.ElementAt(i);

                    if (item.Close < item.Open * (decimal)1.01
                        || item.Volume <= (decimal)(itemVol.Sma * 1.05))
                        continue;

                    var bb_check = item.Low < (decimal)bb.LowerBand && item.Close < (decimal)bb.Sma;
                    if (!bb_check)
                        continue;
                    var buy = (3 * item.High + item.Low) / 4;
                    if(item.Open - item.Low >= 4*(item.High - item.Close))
                    {
                        buy = (3 * item.High + item.Open) / 4;
                    }
                    Console.WriteLine(item.Date.ToString($"dd/MM/yyyy HH|{buy}"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyCoinDanZagerCustom|EXCEPTION| {ex.Message}");
            }
        }
        /*
         Dan Zager custom:
            - Nến xanh ít nhất 2%
            - Vol tại điểm lớn hơn 1% 10 nến liền trước(ít nhất 9 nến)
            - Nến cắt lên MA20 hoặc cắt lên BB dưới

            1. Cut khi giá giảm 7% tính từ điểm pivot
            2. tăng >= 10% từ điểm pivot -> Bán khi xuất hiện nến đỏ >= 3% hoặc giá cắt xuống MA20
         */
        //private void SurveyCoinDanZagerCustom(List<Quote> lData, decimal a)
        //{
        //    try
        //    {
        //        lData = lData.OrderBy(x => x.Date).ToList();
        //        var count = lData.Count();
        //        var lbb = lData.GetBollingerBands();

        //        for (int i = 50; i < count; i++)
        //        {
        //            var item = lData[i];
        //            var bb = lbb.ElementAt(i);
        //            if (_flagBuy)
        //            {
        //                if (_flagRate10)
        //                {
        //                    var rateItem = Math.Round(100 * (-1 + item.Close / item.Open), 2);
        //                    if (rateItem <= -3
        //                        || item.Close < (decimal)bb.Sma)
        //                    {
        //                        PrintBuy(item, i, false);
        //                        _flagRate10 = false;
        //                        _flagBuy = false;
        //                        continue;
        //                    }
        //                }

        //                var rate = Math.Round(100 * (-1 + item.Close / _buy.Close), 1);
        //                if (rate >= 10)
        //                {
        //                    _flagRate10 = true;
        //                }
        //                else if (rate <= -7)
        //                {
        //                    PrintBuy(item, i, false);
        //                    _flagRate10 = false;
        //                    _flagBuy = false;
        //                    continue;
        //                }

        //                continue;
        //            }

        //            if (item.Close < item.Open * (decimal)1.01
        //                || item.Low >= (decimal)bb.Sma)
        //                continue;
        //            var vol_check = lData.Skip(i - 10).Take(10).Count(x => item.Volume >= x.Volume * (decimal)1.07) >= 9;
        //            if (!vol_check)
        //                continue;

        //            var bb_check = (item.Low < (decimal)bb.Sma && item.Close > (decimal)bb.Sma && item.High < (decimal)bb.UpperBand && item.Close < item.Open * ((decimal)1 + Math.Round(a / 200, 2)))
        //                        || (item.Low < (decimal)bb.LowerBand && item.Close > (decimal)bb.LowerBand && item.High < (decimal)bb.Sma);
        //            if (!bb_check)
        //                continue;

        //            var last_check = lData[i - 1].Close < (decimal)lbb.ElementAt(i - 1).Sma;
        //            if (!last_check)
        //                continue;

        //            _flagBuy = true;
        //            PrintBuy(item, i, true);
        //        }

        //        PrintBuyLast();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"PartternService.SurveyCoinDanZagerCustom|EXCEPTION| {ex.Message}");
        //    }
        //}
    }
}
