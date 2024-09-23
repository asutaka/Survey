using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyGoldFish(string code)
        {
            try
            {
                _code = code;

                //var lData = await _apiService.SSI_GetDataStock_Alltime(code);
                var lData = await _apiService.SSI_GetDataStock(code);
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
                    if (_flagBuy && sell)
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
    }
}
