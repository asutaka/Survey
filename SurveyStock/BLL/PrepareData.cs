using Skender.Stock.Indicators;
using SurveyStock.DAL;
using SurveyStock.Model;
using SurveyStock.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyStock.BLL
{
    public static class PrepareData
    {
        public static Dictionary<string, List<Quote>> _dic1d = new Dictionary<string, List<Quote>>();
        public static Dictionary<string, List<Quote>> _dic1w = new Dictionary<string, List<Quote>>();
        //Ex
        public static Dictionary<string, List<QuoteEx>> _dicEx1d = new Dictionary<string, List<QuoteEx>>();
        public static Dictionary<string, List<QuoteEx>> _dicEx1w = new Dictionary<string, List<QuoteEx>>();
        private readonly static int _numThread = 50;

        public static void LoadData1d()
        {
            var dtStart = DateTime.Now;
            Console.WriteLine($"PrepareData.LoadData1d|INFO(START)|{dtStart}");
            var lData = sqliteComDB.GetData_Company();
            Parallel.ForEach(lData, new ParallelOptions { MaxDegreeOfParallelism = _numThread },
                item =>
            {
                if (item.status <= 0)
                    return;

                try
                {
                    _dic1d.Add(item.stock_code, sqliteDayDB.GetData(item.stock_code).Select(x => new Quote
                    {
                        Date = x.t.UnixTimeStampToDateTime(),
                        Open = x.o,
                        Close = x.c,
                        High = x.h,
                        Low = x.l,
                        Volume = x.v
                    }).ToList());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            Console.WriteLine($"PrepareData.LoadData1d|INFO(END)|{DateTime.Now}");
            Console.WriteLine($"PrepareData.LoadData1d|INFO(Total Time)|{(DateTime.Now - dtStart).TotalSeconds}");
        }

        public static void LoadData1W()
        {
            _dic1w = LoadData1wPartial(_dic1d);
        }

        private static Dictionary<string, List<Quote>> LoadData1wPartial(Dictionary<string, List<Quote>> dicDay)
        {
            var dicResult = new Dictionary<string, List<Quote>>();
            foreach (var item in dicDay)
            {
                var lData = new List<Quote>();
                var model = new Quote();
                var weekOfYear = 0;
                var isEndCandleStick = true;
                foreach (var itemData in item.Value)
                {
                    var week = itemData.Date.GetIso8601WeekOfYear();
                    if(week != weekOfYear)
                    {
                        if(weekOfYear > 0)
                        {
                            lData.Add(model);
                        }
                        weekOfYear = week;
                        model = itemData;
                        isEndCandleStick = true;
                        continue;
                    }

                    isEndCandleStick = false;
                    model.Volume += itemData.Volume;
                    if(itemData.High > model.High)
                    {
                        model.High = itemData.High;
                    }
                    if(itemData.Low < model.Low)
                    {
                        model.Low = itemData.Low;
                    }
                    model.Close = itemData.Close;
                }
                if (!isEndCandleStick)
                {
                    lData.Add(model);
                }
                dicResult.Add(item.Key, lData);
            }
            return dicResult;
        }
    }

    public static class PrepareIndicator
    {
        private readonly static int _numThread = 50;
        public static void Ma20_1d()
        {
            var dtStart = DateTime.Now;
            Console.WriteLine($"PrepareIndicator.Ma20_1d|INFO(START)|{dtStart}");
            Parallel.ForEach(PrepareData._dic1d, new ParallelOptions { MaxDegreeOfParallelism = _numThread },
               item =>
               {
                   var lsma20 = item.Value.GetSma(20);
                   PrepareData._dicEx1d.Add(item.Key, item.Value.Select((x, index) => new QuoteEx
                   {
                       Date = x.Date,
                       Open = x.Open,
                       High = x.High,
                       Low = x.Low,
                       Close = x.Close,
                       Volume = x.Volume,
                       Ma20 = lsma20.ElementAt(index).Sma ?? 0
                   }).ToList());
               });
            Console.WriteLine($"PrepareIndicator.Ma20_1d|INFO(END)|{DateTime.Now}");
            Console.WriteLine($"PrepareIndicator.Ma20_1d|INFO(Total Time)|{(DateTime.Now - dtStart).TotalSeconds}");
        }

        public static void Ma20_1w()
        {
            foreach (var item in PrepareData._dic1w)
            {
                var lsma20 = item.Value.GetSma(20);
                PrepareData._dicEx1w.Add(item.Key, item.Value.Select((x, index) => new QuoteEx
                {
                    Date = x.Date,
                    Open = x.Open,
                    High = x.High,
                    Low = x.Low,
                    Close = x.Close,
                    Volume = x.Volume,
                    Ma20 = lsma20.ElementAt(index).Sma ?? 0
                }).ToList());
            }
        }
    }
}
