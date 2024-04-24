using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyStock.BLL
{
    public static class CalMng
    {
        public static void Ma20RateAboveBelow_1d()
        {
            //Lay ma co ngay len san som nhat
            var dic = PrepareData._dicEx1d.OrderByDescending(x => x.Value.Count());
            var itemOldest = dic.First();
            var lDate = itemOldest.Value.Select(x => x.Date);
            var countDate = lDate.Count();
            //Danh sách thống kê
            var lStastitic = new List<(DateTime, double)>();
            for (int i = 0; i < countDate; i++)
            {
                var itemDate = lDate.ElementAt(i);

                var lEntityOfDate = dic.Where(x => x.Value.Any(y => y.Date == itemDate))
                .SelectMany(p => p.Value)
                .Where(c => c.Date == itemDate)
                .Select(x => x);

                float countValid = lEntityOfDate.Count();
                var countRateValid = lEntityOfDate.Count(x => x.Ma20 > 0 && x.Close >= (decimal)x.Ma20);
                var itemStastitic = (itemDate, Math.Round(countRateValid / countValid, 2));
                lStastitic.Add(itemStastitic);
            }

            foreach (var item in lStastitic)
            {
                Console.WriteLine($"{item.Item1.ToString("dd/MM/yyyy")}: {item.Item2}");
            }
        }

        public static void Ma20RateAboveBelow_1w()
        {
            //Lay ma co ngay len san som nhat
            var dic = PrepareData._dicEx1w.OrderByDescending(x => x.Value.Count());
            var itemOldest = dic.First();
            var lDate = itemOldest.Value.Select(x => x.Date);
            var countDate = lDate.Count();
            //Danh sách thống kê
            var lStastitic = new List<(DateTime, double)>();
            for (int i = 0; i < countDate; i++)
            {
                var itemDate = lDate.ElementAt(i);

                var lEntityOfDate = dic.Where(x => x.Value.Any(y => y.Date == itemDate))
                .SelectMany(p => p.Value)
                .Where(c => c.Date == itemDate)
                .Select(x => x);

                float countValid = lEntityOfDate.Count();
                var countRateValid = lEntityOfDate.Count(x => x.Ma20 > 0 && x.Close >= (decimal)x.Ma20);
                var itemStastitic = (itemDate, Math.Round(countRateValid / countValid, 2));
                lStastitic.Add(itemStastitic);
            }

            foreach (var item in lStastitic)
            {
                Console.WriteLine($"{item.Item1.ToString("dd/MM/yyyy")}: {item.Item2}");
            }
        }
    }
}
