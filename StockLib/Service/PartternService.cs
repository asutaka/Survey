using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using System.Text;

namespace StockLib.Service
{
    public interface IPartternService
    {
        Task SurveyIchimoku(string code);
        Task SurveySuperTrend(string code);
        Task SurveyGoldFish(string code);
        Task SurveyVCP(string code);
        void RankChungKhoan();
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

        string _code = string.Empty;
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

        List<(string, int, decimal, decimal)> _lCode = new List<(string, int, decimal, decimal)>();
        private void PrintBuyLast()
        {
            Console.WriteLine();
            var avg = _lrateBuy.Count() == 0 ? 0 : Math.Round(_lrateBuy.Average(), 1);
            var sum = _lrateBuy.Count() == 0 ? 0 : Math.Round(_lrateBuy.Sum(), 1);
            _lCode.Add((_code, 1, avg, sum));
            Console.WriteLine($"=> So Lan Mua-Ban: {_countBuy}| TakeProfit trung binh: {avg}%| Tong TakeProfit: {sum}%");


            if (_lPivot.Count() == 0)
                return;

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

            var avgSB = Math.Round(lSB.Average(), 1);
            var sumSB = Math.Round(lSB.Sum(), 1);
            _lCode.Add((_code, 0, avgSB, sumSB));
            Console.WriteLine($"=> Ban-Mua:TakeProfit trung binh: {avgSB}%| Tong TakeProfit: {sumSB}%");
        }

        public void RankChungKhoan()
        {
            var lTop20 = _lCode.Where(x => x.Item2 == 1).OrderByDescending(x => x.Item3).Take(20);
            var sBuilder = new StringBuilder();
            var i = 1;
            foreach (var item in lTop20)
            {
                var SB = _lCode.Where(x => x.Item2 == 0).FirstOrDefault(x => x.Item1 == item.Item1);
                sBuilder.AppendLine($"{i++}.{item.Item1}|AVG: {item.Item3}%|Total: {item.Item4}%| AVG Loss: {SB.Item3}%| Total Loss: {SB.Item4}%");
            }
            Console.WriteLine(sBuilder.ToString());
        }
    }
}
