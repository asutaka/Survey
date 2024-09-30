using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Skender.Stock.Indicators;
using StockLib.DAL;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public interface IPartternService
    {
        Task SurveySuperTrend(string code);
        Task SurveyGoldFish(string code);
        Task SurveyVCP(string code);
        Task SurveyW(string code);
        Task SurveyDanZagerCustom(string code);
        Task Survey3C(string code);
        Task SurveyMa20(string code);
        void RankChungKhoan();
        void TotalDays();
    }
    public partial class PartternService : IPartternService
    {
        private readonly ILogger<PartternService> _logger;
        private readonly IAPIService _apiService;
        private readonly IStockRepo _stockRepo;
        public PartternService(ILogger<PartternService> logger,
                                IAPIService apiService,
                                IStockRepo stockRepo) 
        {
            _logger = logger;
            _apiService = apiService;
            _stockRepo = stockRepo;
        }

        string _code = string.Empty;
        int _countBuy = 0;
        bool _flagBuy = false;
        Quote _buy = null;
        int _indexBuy = -1;
        List<decimal> _lrateBuy = new List<decimal>();
        List<decimal> _lHold = new List<decimal>();
        List<Quote> _lPivot = new List<Quote>();
        List<(string, DateTime)> _lTime = new List<(string, DateTime)>();
        bool _flagRate10 = false;
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
                _lHold.Add(totalDays);
                _countBuy++;

                Console.WriteLine($"|MUA {_buy.Date.ToString("dd/MM/yyyy")}: {_buy.Close}|BAN {item.Date.ToString("dd/MM/yyyy")}: {item.Close}|Nam giu: {totalDays}|TP: {rate}%");
            }
        }

        List<(string, int, decimal, decimal, decimal, decimal, decimal)> _lCode = new List<(string, int, decimal, decimal, decimal, decimal, decimal)>();
        private void PrintBuyLast()
        {
            Console.WriteLine();
            var countRate = _lrateBuy.Count();
            if (countRate == 0)
            {
                Console.WriteLine("Khong co diem mua!");
                Reset();
                return;
            }
                
            var avg = Math.Round(_lrateBuy.Average(), 1);
            var sum = Math.Round(_lrateBuy.Sum(), 1);
            decimal wincount = _lrateBuy.Count(x => x >= 7);
            decimal lossCount = _lrateBuy.Count(x => x <= -5);

            _lCode.Add((_code, 1, avg, sum, (decimal)_lHold.Average(), Math.Round(wincount * 100 / countRate), Math.Round(lossCount * 100 / countRate)));
            Console.WriteLine($"=> So Lan Mua-Ban: {_countBuy}| TakeProfit trung binh: {avg}%| Tong TakeProfit: {sum}%");


            if (_lPivot.Count() == 0)
            {
                Reset();
                return;
            }    

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
            if (lSB.Count() == 0)
            {
                Reset();
                return;
            }

            var avgSB = Math.Round(lSB.Average(), 1);
            var sumSB = Math.Round(lSB.Sum(), 1);
            _lCode.Add((_code, 0, avgSB, sumSB, 0, 0, 0));
            Console.WriteLine($"=> Ban-Mua:TakeProfit trung binh: {avgSB}%| Tong TakeProfit: {sumSB}%");

            //reset 
            Reset();
        }

        private void Reset()
        {
            //Insert Time
            _lTime.AddRange(_lPivot.Select(x => (_code, x.Date)));

            _code = string.Empty;
            _countBuy = 0;
            _flagBuy = false;
            _buy = null;
            _indexBuy = -1;
            _lrateBuy.Clear();
            _lHold.Clear();
            _lPivot.Clear();
        }

        public void RankChungKhoan()
        {
            var lTop = _lCode.Where(x => x.Item2 == 1 && x.Item3 != x.Item4).OrderByDescending(x => x.Item3).Take(100).ToList();
            lTop = lTop.Where(x => x.Item6 >= x.Item7).ToList();
            var sBuilder = new StringBuilder();
            var i = 1;
            foreach (var item in lTop)
            {
                var SB = _lCode.Where(x => x.Item2 == 0).FirstOrDefault(x => x.Item1 == item.Item1);
                sBuilder.AppendLine($"{i++}.{item.Item1}|AVG: {item.Item3}%|Total: {item.Item4}%|Nam_giu_tb: {Math.Round(item.Item5)}| Win: {item.Item6}| Loss: {item.Item7}");
                //sBuilder.AppendLine($"{i++}.{item.Item1}|AVG: {item.Item3}%|Total: {item.Item4}%|Nam_giu_tb: {Math.Round(item.Item5)}| AVG Loss: {SB.Item3}%| Total Loss: {SB.Item4}%");
                //Update Stock
                //var stock = _stockRepo.GetEntityByFilter(Builders<Stock>.Filter.Eq(x => x.s, item.Item1));
                //if (stock == null)
                //    continue;
                //if (stock.indicator is null)
                //{
                //    stock.indicator = new List<IndicatorModel>();
                //}
                //stock.indicator.Add(new IndicatorModel
                //{
                //    type = (int)EIndicator.GoldFish,
                //    rank = i - 1,
                //    avg_rate = (double)item.Item3,
                //    avg_num = (int)Math.Round(item.Item5),
                //    win_rate = (double)item.Item6,
                //    loss_rate = (double)item.Item7
                //});
                //_stockRepo.Update(stock);
            }
            Console.WriteLine(sBuilder.ToString());
        }

        public void TotalDays()
        {
            var lSort = _lTime.OrderBy(x => x.Item2);
            foreach (var item in lSort)
            {
                Console.WriteLine($"{item.Item2}: {item.Item1}");
            }
        }
    }
}
