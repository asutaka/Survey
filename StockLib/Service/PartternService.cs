using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;

namespace StockLib.Service
{
    public interface IPartternService
    {
        Task SurveyIchimoku(string code);
        Task SurveySuperTrend(string code);
        Task GoldFish(string code);
        Task GoldFishInvert(string code);
    }
    public partial class PartternService : IPartternService
    {
        private readonly ILogger<PartternService> _logger;
        private readonly IAPIService _apiService;
        public PartternService(ILogger<PartternService> logger,
                                IAPIService apiService) 
        {
            _logger = logger;
            _apiService = apiService;
        }

        int _countBuy = 0;
        bool _flagBuy = false;
        Quote _buy = null;
        int _indexBuy = -1;
        List<decimal> _lrateBuy = new List<decimal>();
        private void PrintBuy(Quote item, int index, bool isBuy)
        {
            if (isBuy)
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

                Console.WriteLine($"|MUA {_buy.Date.ToString("dd/MM/yyyy")}: {_buy.Close}|BAN {item.Date.ToString("dd/MM/yyyy")}: {item.Close}|Nam giu: {totalDays}|TP: {rate}%");
            }
        }

        private void PrintBuyLast()
        {
            Console.WriteLine();
            Console.WriteLine($"=> So Lan Mua-Ban: {_countBuy}| TakeProfit trung binh: {Math.Round(_lrateBuy.Average(), 1)}%| Tong TakeProfit: {Math.Round(_lrateBuy.Sum(), 1)}%");
        }
    }
}
