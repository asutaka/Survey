﻿using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyDanZagerCustom(string code)
        {
            try
            {
                _code = code;

                //var lData = await _apiService.SSI_GetDataStock_Alltime(code);
                var lData = await _apiService.SSI_GetDataStock(code);
                DanZagerCustom(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyDanZagerCustom|EXCEPTION| {ex.Message}");
            }
        }
        /*
         Dan Zager custom:
            - Nến xanh ít nhất 2%
            - Vol tại điểm lớn hơn 1% 10 nến liền trước(ít nhất 9 nến)
            - BB trên - Close >= Open - BB dưới

            1. Cut khi giá giảm 5% tính từ điểm pivot
            2. tăng >= 10% từ điểm pivot -> Bán khi xuất hiện nến đỏ >= 3% hoặc giá cắt xuống MA20
         */
        private bool _flagRate10 = false;
        private void DanZagerCustom(List<Quote> lData)
        {
            try
            {
                lData = lData.OrderBy(x => x.Date).ToList();
                var count = lData.Count();
                var lbb = lData.GetBollingerBands();

                for (int i = 50; i < count; i++)
                {
                    var item = lData[i];
                    var bb = lbb.ElementAt(i);
                    if (_flagBuy)
                    {
                        if(_flagRate10)
                        {
                            var rateItem = Math.Round(100 * (-1 + item.Close / item.Open));
                            if (rateItem <= -3
                                || item.Close < (decimal)bb.Sma)
                                //if (item.Close < (decimal)bb.Sma)
                            {
                                PrintBuy(item, i, false);
                                _flagRate10 = false;
                                _flagBuy = false;
                                continue;
                            }
                        }

                        var rate = Math.Round(100 * (-1 + item.Close / _buy.Close), 1);
                        if (rate >= 10)
                        {
                            _flagRate10 = true;
                        }
                        else if (rate <= -7)
                        {
                            PrintBuy(item, i, false);
                            _flagRate10 = false;
                            _flagBuy = false;
                            continue;
                        }

                        continue;
                    }

                    if (item.Close < item.Open * (decimal)1.02)
                        continue;
                    var vol_check = lData.Skip(i - 10).Take(10).Count(x => item.Volume >= x.Volume * (decimal)1.02) >= 9;
                    if (!vol_check)
                        continue;
                    
                    var bb_check = ((decimal)(bb.UpperBand ?? 0) - item.Close) >= (item.Open - (decimal)(bb.LowerBand ?? 0));
                    if (!bb_check)
                        continue;

                    _flagBuy = true;
                    PrintBuy(item, i, true);
                }

                PrintBuyLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.DanZagerCustom|EXCEPTION| {ex.Message}");
            }
        }
    }
}