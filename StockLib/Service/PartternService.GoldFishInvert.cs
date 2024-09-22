using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task GoldFishInvert(string code)
        {
            try
            {
                var lData = await _apiService.SSI_GetDataStock_Alltime(code);
                GoldFishInvert(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.GoldFish|EXCEPTION| {ex.Message}");
            }
        }
        private void GoldFishInvert(List<Quote> lData)
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
                    if (!_flagSell && sell)
                    {
                        _flagSell = true;
                        PrintSell(item, i, true);
                    }
                    if (buy)
                    {
                        PrintSell(item, i, false);
                        _flagSell = false;
                    }
                }

                PrintSellLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.GoldFish|EXCEPTION| {ex.Message}");
            }
        }

        int _countSell = 0;
        bool _flagSell = false;
        Quote _sell = null;
        int _indexSell = -1;
        List<decimal> _lrateSell = new List<decimal>();
        private void PrintSell(Quote item, int index, bool isSell)
        {
            if(isSell)
            {
                _sell = item;
                _indexSell = index;
            }
            else
            {
                if (!_flagSell)
                    return;

                var totalDays = index - _indexSell;
                var rate = Math.Round(100 * (-1 + item.Close / _sell.Close), 1);
                _lrateSell.Add(rate);
                _countSell++;

                //Console.WriteLine($"|MUA {_sell.Date.ToString("dd/MM/yyyy")}: {_sell.Close}|BAN {item.Date.ToString("dd/MM/yyyy")}: {item.Close}|Nam giu: {totalDays}|TP: {rate}%");
            }
        }

        private void PrintSellLast()
        {
            Console.WriteLine();
            Console.WriteLine($"=> So Lan Ban-Mua: {_countSell}| TakeProfit trung binh: {Math.Round(_lrateSell.Average(), 1)}%| Tong TakeProfit: {Math.Round(_lrateSell.Sum(), 1)}%");
        }
    }
}
