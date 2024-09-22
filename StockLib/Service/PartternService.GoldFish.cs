﻿using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task GoldFish(string code)
        {
            try
            {
                var lData = await _apiService.SSI_GetDataStock_Alltime(code);
                GoldFish(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.GoldFish|EXCEPTION| {ex.Message}");
            }
        }
        private void GoldFish(List<Quote> lData)
        {
            try
            {
                lData = lData.OrderBy(x => x.Date).ToList();
                var lHigh = new List<Quote>();
                var lLow = new List<Quote>();
                foreach (var item in lData) 
                {
                    lHigh.Add(new Quote
                    {
                        Date = item.Date,
                        Close = item.High
                    });
                    lLow.Add(new Quote
                    {
                        Date = item.Date,
                        Close = item.Low
                    });
                }

                var lSma_6 = lData.GetSma(6);
                var lSma_39_H = lHigh.GetSma(39);
                var lSma_39_L = lLow.GetSma(39);
                var count = lData.Count();

                for (int i = 10; i < count; i++)
                {
                    if (i < 40)
                    {
                        continue;
                    }
                    var item = lData[i];
                    var sma_6 = lSma_6.ElementAt(i);
                    if (sma_6.Sma is null)
                        continue;

                    var k = sma_6.Sma * 0.04 / 100;
                    var ab = lSma_39_H.ElementAt(i).Sma + k;
                    var ba = lSma_39_L.ElementAt(i).Sma - k;
                    
                    var XXX = item.Close - lData[i - 10].Close;
                    var buy = XXX > 0 && (double)item.Close > ab;
                    var sell = XXX < 0 && (double)item.Close < ba;
                    if (!_flagBuy && buy)
                    {
                        _flagBuy = true;
                        PrintBuy(item, i, true);
                    }
                    if (sell)
                    {
                        PrintBuy(item, i, false);
                        _flagBuy = false;
                    }
                }

                PrintBuyLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.GoldFish|EXCEPTION| {ex.Message}");
            }
        }

        int _countBuy = 0;
        bool _flagBuy = false;
        Quote _buy = null;
        int _indexBuy = -1;
        List<decimal> _lrateBuy = new List<decimal>();
        private void PrintBuy(Quote item, int index, bool isBuy)
        {
            if(isBuy)
            {
                _buy = item;
                _indexBuy = index;
            }
            else
            {
                if (!_flagBuy)
                    return;

                var totalDays = index - _indexBuy;
                var rate = Math.Round(100 * (-1 + item.Close / _buy.Close), 1);
                _lrateBuy.Add(rate);
                _countBuy++;

                //Console.WriteLine($"|MUA {_buy.Date.ToString("dd/MM/yyyy")}: {_buy.Close}|BAN {item.Date.ToString("dd/MM/yyyy")}: {item.Close}|Nam giu: {totalDays}|TP: {rate}%");
            }
        }

        private void PrintBuyLast()
        {
            Console.WriteLine();
            Console.WriteLine($"=> So Lan Mua-Ban: {_countBuy}| TakeProfit trung binh: {Math.Round(_lrateBuy.Average(), 1)}%| Tong TakeProfit: {Math.Round(_lrateBuy.Sum(), 1)}%");
        }
    }
}
