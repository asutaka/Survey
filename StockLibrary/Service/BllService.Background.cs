using MongoDB.Driver;
using Skender.Stock.Indicators;
using StockLibrary.Model;
using StockLibrary.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLibrary.Service
{
    public partial class BllService
    {
        private readonly int _numThread = 5;
        public void BackgroundWork()
        {

            FilterDefinition<Report> filter;
            var builder = Builders<Report>.Filter;
            filter = builder.And(builder.Gte(x => x.t, 0));
            _reportRepo.DeleteMany(filter); 
            var lStock = _stockRepo.GetAll();
            Parallel.ForEach(lStock, new ParallelOptions { MaxDegreeOfParallelism = _numThread },
               itemStock =>
               {
                   try
                   {
                       var lForeign = _foreignRepo.GetWithFilter(1, 10000, itemStock.MaCK, 0);
                       if (lForeign is null)
                           return;

                       var lForeignConvert = lForeign.OrderBy(x => x.d).Select(x => new Quote
                       {
                           Date = x.d.UnixTimeStampToDateTime(),
                           Open = x.o,
                           Close = x.c,
                           High = x.h,
                           Low = x.l,
                           Volume = x.tmvo
                       });

                       var date = DateTime.Now;
                       var lWeek = new List<Quote>();
                       var lCurrentYear = lForeignConvert.Where(x => x.Date.Year == date.Year)
                       .GroupBy(i => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(i.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                       if(lCurrentYear.Any())
                       {
                           lWeek.AddRange(BuildlWeek(lCurrentYear));
                       }

                       var lPrevYear = lForeignConvert.Where(x => x.Date.Year == date.Year - 1)
                       .GroupBy(i => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(i.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                       if(lPrevYear.Any())
                       {
                           lWeek.AddRange(BuildlWeek(lPrevYear));
                       }
                       //Ngay
                       if(lForeignConvert.Count() >= 30)
                       {
                           var lEma10 = lForeignConvert.GetEma(10);
                           var lIchi = lForeignConvert.GetIchimoku();
                           var lBB = lForeignConvert.GetBollingerBands();
                           var lMacd = lForeignConvert.GetMacd();

                           var model = new Report
                           {
                               mode = (int)ETimeMode.Day,
                               s = itemStock.MaCK,
                               d = lForeignConvert.Max(x => x.Date),
                               ema10 = lEma10.Last(),
                               ichi = lIchi.Last(),
                               bb = lBB.Last(),
                               macd = lMacd.Last(),
                               t = DateTimeOffset.Now.ToUnixTimeSeconds()
                           };
                           //insert
                           _reportRepo.InsertOne(model);
                       }
                       
                       //Tuan
                       if(lWeek.Count() >= 30)
                       {
                           var lEma10 = lWeek.GetEma(10);
                           var lIchi = lWeek.GetIchimoku();
                           var lBB = lWeek.GetBollingerBands();
                           var lMacd = lWeek.GetMacd();

                           var model = new Report
                           {
                               mode = (int)ETimeMode.Week,
                               s = itemStock.MaCK,
                               d = lWeek.Max(x => x.Date),
                               ema10 = lEma10.Last(),
                               ichi = lIchi.Last(),
                               bb = lBB.Last(),
                               macd = lMacd.Last(),
                               t = DateTimeOffset.Now.ToUnixTimeSeconds()
                           };
                           //insert
                           _reportRepo.InsertOne(model);
                       }
                   }
                   catch(Exception ex)
                   {
                       Console.WriteLine(ex.ToString());
                   }
               });


            List<Quote> BuildlWeek(IEnumerable<IGrouping<int, Quote>> lInput)
            {
                var lWeek = new List<Quote>();
                foreach (var item in lInput)
                {
                    var lQuote = item.ToList();
                    if (lQuote is null
                    || !lQuote.Any())
                        continue;

                    lWeek.Add(new Quote
                    {
                        Date = lQuote.Min(x => x.Date),
                        Open = lQuote.MinBy(x => x.Date).Open,
                        Close = lQuote.MaxBy(x => x.Date).Close,
                        High = lQuote.Max(x => x.High),
                        Low = lQuote.Min(x => x.Low),
                        Volume = lQuote.Sum(x => x.Volume)
                    });
                }
                return lWeek;
            }
        } 
    }
}


