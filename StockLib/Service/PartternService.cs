using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using static iTextSharp.text.pdf.AcroFields;

namespace StockLib.Service
{
    public interface IPartternService
    {
        Task SurveyIchimoku(string code);
        Task SurveySuperTrend(string code);
        Task GoldFish(string code);
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
        List<Quote> _lPivot = new List<Quote>();
        private void PrintBuy(Quote item, int index, bool isBuy)
        {
            _lPivot.Add(item);
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

                //Console.WriteLine($"|MUA {_buy.Date.ToString("dd/MM/yyyy")}: {_buy.Close}|BAN {item.Date.ToString("dd/MM/yyyy")}: {item.Close}|Nam giu: {totalDays}|TP: {rate}%");
            }
        }

        private void PrintBuyLast()
        {
            Console.WriteLine();
            Console.WriteLine($"=> So Lan Mua-Ban: {_countBuy}| TakeProfit trung binh: {Math.Round(_lrateBuy.Average(), 1)}%| Tong TakeProfit: {Math.Round(_lrateBuy.Sum(), 1)}%");

            _lPivot.RemoveAt(0);
            var count = _lPivot.Count;
            var lSB = new List<decimal>();
            for (int i = 0; i < count; i = i + 2) 
            {
                var j = i + 1;
                if(j >= count)
                {
                    break;
                }
                var itemFirst = _lPivot[i];
                var itemLast = _lPivot[j];

                var rate = Math.Round(100 * (-1 + itemLast.Close / itemFirst.Close), 1);
                lSB.Add(rate);

                //Console.WriteLine($"|MUA {itemFirst.Date.ToString("dd/MM/yyyy")}: {itemFirst.Close}|BAN {itemLast.Date.ToString("dd/MM/yyyy")}: {itemLast.Close}|TP: {rate}%");
            }
            Console.WriteLine($"=> Ban-Mua:TakeProfit trung binh: {Math.Round(lSB.Average(), 1)}%| Tong TakeProfit: {Math.Round(lSB.Sum(), 1)}%");
        }
    }
}
