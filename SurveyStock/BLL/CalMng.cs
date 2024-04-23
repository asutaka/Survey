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
        public static void Ma20RateAboveBelow()
        {
            //Lay ma co ngay len san som nhat
            var dic = PrepareData._dicHoseEx1d.OrderByDescending(x => x.Value.Count());
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
                var itemStastitic = (itemDate, Math.Round(countRateValid / countValid, 1));
                lStastitic.Add(itemStastitic);
            }

            foreach (var item in lStastitic)
            {
                Console.WriteLine($"{item.Item1.ToString("dd/MM/yyyy")}: {item.Item2}");
            }


            //foreach (var item in PrepareData._dicHoseEx1d.OrderByDescending(x => x.Value.Count()))
            //{

            //}



            //var dic1d = new Dictionary<string, List<Quote>>();
            //var dicResult = new Dictionary<string, double>();
            //foreach (var item in PrepareData._dicHose1d)
            //{
            //    dic1d.Add(item.Key, item.Value);
            //}
            //foreach (var item in PrepareData._dicHNX1d)
            //{
            //    dic1d.Add(item.Key, item.Value);
            //}
            //foreach (var item in PrepareData._dicUpcom1d)
            //{
            //    dic1d.Add(item.Key, item.Value);
            //}
            //var count = dic1d.Count();
            //for (int i = 0; i < count; i++)
            //{
            //    var element = dic1d.ElementAt(i);
            //    var elementMa20 = PrepareIndicator._dicMa20_1d.ElementAt(i);
            //}

            //foreach (var item in dic1d)
            //{

            //}



            ////Lay ma co ngay len san som nhat
            //var itemOldest = PrepareData._dicHose1d.OrderByDescending(x => x.Value.Count()).First();
            ////Danh sach ngay check
            //var lDate = itemOldest.Value.Select(x => x.Date);
            ////Danh sách thống kê
            //var lStastitic = new List<(DateTime, double)>();

            //foreach (var itemDate in lDate)
            //{
            //    var countElementValid = 0;
            //    var countElementAboveMa20 = 0;
            //    var lElement = PrepareData._dicHose1d.Where(x => x.Value.Any(y => y.Date == itemDate));
            //    var lMa20 = PrepareIndicator._dicMa20_1d.Where(x => lElement.Any(y => y.Key == x.Key));
            //    var countElement = lElement.Count();
            //    for (int i = 0; i < countElement; i++)
            //    {
            //        var itemElement = lElement.ElementAt(i);
            //        var itemMa20 = lMa20.ElementAt(i).Value;
            //        var entityOfDate = itemElement.Value.First(x => x.Date == itemDate);
            //        var indexOf = itemElement.Value.IndexOf(entityOfDate);
            //        var entityMa20 = itemMa20.ElementAt(indexOf);
            //        if(entityMa20.Sma != null)
            //        {
            //            //hop le
            //            countElementValid++;
            //        }
            //        if(entityOfDate.Close > (decimal)(entityMa20.Sma ?? 0))
            //        {
            //            countElementAboveMa20++;
            //        }
            //    }
            //    lStastitic.Add((itemDate, Math.Round((float)countElementAboveMa20 / countElementValid, 1)));
            //}
            //foreach (var item in lStastitic)
            //{
            //    Console.WriteLine($"{item.Item1.ToString("dd/MM/YYYY")}: {item.Item2}%");
            //}
        }
    }
}
