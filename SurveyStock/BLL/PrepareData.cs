using Skender.Stock.Indicators;
using SurveyStock.DAL;
using SurveyStock.Model;
using SurveyStock.Util;
using System.Collections.Generic;
using System.Linq;

namespace SurveyStock.BLL
{
    public static class PrepareData
    {
        public static Dictionary<string, List<Quote>> _dicHose1d = new Dictionary<string, List<Quote>>();
        public static Dictionary<string, List<Quote>> _dicHNX1d = new Dictionary<string, List<Quote>>();
        public static Dictionary<string, List<Quote>> _dicUpcom1d = new Dictionary<string, List<Quote>>();

        public static Dictionary<string, List<Quote>> _dicHose1w = new Dictionary<string, List<Quote>>();
        public static Dictionary<string, List<Quote>> _dicHNX1w = new Dictionary<string, List<Quote>>();
        public static Dictionary<string, List<Quote>> _dicUpcom1w = new Dictionary<string, List<Quote>>();

        public static Dictionary<string, List<Quote>> _dicHose1h = new Dictionary<string, List<Quote>>();
        public static Dictionary<string, List<Quote>> _dicHNX1h = new Dictionary<string, List<Quote>>();
        public static Dictionary<string, List<Quote>> _dicUpcom1h = new Dictionary<string, List<Quote>>();
        //Ex
        public static Dictionary<string, List<QuoteEx>> _dicHoseEx1d = new Dictionary<string, List<QuoteEx>>();
        public static Dictionary<string, List<QuoteEx>> _dicHNXEx1d = new Dictionary<string, List<QuoteEx>>();
        public static Dictionary<string, List<QuoteEx>> _dicUpcomEx1d = new Dictionary<string, List<QuoteEx>>();

        public static void Instance()
        {
            LoadData1d();
            LoadData1W();
            LoadData1h();
        }

        private static void LoadData1h()
        {
            var lData = sqliteComDB.GetData_Company();
            foreach (var item in lData)
            {
                if (item.status <= 0)
                    continue;
                if (item.stock_exchange == (int)EStockExchange.Hose)
                {
                    _dicHose1h.Add(item.stock_code, sqliteHourDB.GetData(item.stock_code).Select(x => new Quote
                    {
                        Date = x.t.UnixTimeStampToDateTime(),
                        Open = x.o,
                        Close = x.c,
                        High = x.h,
                        Low = x.l,
                        Volume = x.v
                    }).ToList());
                }
                else if (item.stock_exchange == (int)EStockExchange.HNX)
                {
                    _dicHNX1h.Add(item.stock_code, sqliteHourDB.GetData(item.stock_code).Select(x => new Quote
                    {
                        Date = x.t.UnixTimeStampToDateTime(),
                        Open = x.o,
                        Close = x.c,
                        High = x.h,
                        Low = x.l,
                        Volume = x.v
                    }).ToList());
                }
                else
                {
                    _dicUpcom1h.Add(item.stock_code, sqliteHourDB.GetData(item.stock_code).Select(x => new Quote
                    {
                        Date = x.t.UnixTimeStampToDateTime(),
                        Open = x.o,
                        Close = x.c,
                        High = x.h,
                        Low = x.l,
                        Volume = x.v
                    }).ToList());
                }
            }
        }

        private static void LoadData1d()
        {
            var lData = sqliteComDB.GetData_Company();
            foreach (var item in lData)
            {
                if (item.status <= 0)
                    continue;
                if (item.stock_exchange == (int)EStockExchange.Hose)
                {
                    _dicHose1d.Add(item.stock_code, sqliteDayDB.GetData(item.stock_code).Select(x => new Quote { 
                        Date = x.t.UnixTimeStampToDateTime(),
                        Open = x.o,
                        Close = x.c,
                        High = x.h,
                        Low = x.l,
                        Volume = x.v
                    }).ToList());
                }
                else if (item.stock_exchange == (int)EStockExchange.HNX)
                {
                    _dicHNX1d.Add(item.stock_code, sqliteDayDB.GetData(item.stock_code).Select(x => new Quote
                    {
                        Date = x.t.UnixTimeStampToDateTime(),
                        Open = x.o,
                        Close = x.c,
                        High = x.h,
                        Low = x.l,
                        Volume = x.v
                    }).ToList());
                }
                else
                {
                    _dicUpcom1d.Add(item.stock_code, sqliteDayDB.GetData(item.stock_code).Select(x => new Quote
                    {
                        Date = x.t.UnixTimeStampToDateTime(),
                        Open = x.o,
                        Close = x.c,
                        High = x.h,
                        Low = x.l,
                        Volume = x.v
                    }).ToList());
                }
            }
        }

        private static void LoadData1W()
        {
            _dicHose1w = LoadData1wPartial(_dicHose1d);
            _dicHNX1w = LoadData1wPartial(_dicHNX1d);
            _dicUpcom1w = LoadData1wPartial(_dicUpcom1d);
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
        public static void Instance()
        {
            Ma20_1d();
        }
        private static void Ma20_1d()
        {
            foreach (var item in PrepareData._dicHose1d)
            {
                var lsma20 = item.Value.GetSma(20);
                PrepareData._dicHoseEx1d.Add(item.Key, item.Value.Select((x, index) => new QuoteEx {
                   Date = x.Date,
                   Open = x.Open,
                   High = x.High,
                   Low = x.Low,
                   Close = x.Close,
                   Volume = x.Volume,
                   Ma20 = lsma20.ElementAt(index).Sma??0
                }).ToList());
            }
            foreach (var item in PrepareData._dicHNX1d)
            {
                var lsma20 = item.Value.GetSma(20);
                PrepareData._dicHNXEx1d.Add(item.Key, item.Value.Select((x, index) => new QuoteEx
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
            foreach (var item in PrepareData._dicUpcom1d)
            {
                var lsma20 = item.Value.GetSma(20);
                PrepareData._dicUpcomEx1d.Add(item.Key, item.Value.Select((x, index) => new QuoteEx
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
