using MongoDB.Driver;
using StockLibrary.Mapping;
using StockLibrary.Model;
using StockLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace StockLibrary.Service
{
    public partial class BllService
    {
        public async Task<(int, List<string>)> SyncGDNuocNgoai()
        {
            var dt = DateTime.Now;
            if (dt.DayOfWeek == DayOfWeek.Saturday
                || dt.DayOfWeek == DayOfWeek.Sunday
                || dt.Hour < 17)
                return (0, null);

            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            var filter = Builders<ConfigData>.Filter.Eq(x => x.type, (int)EConfigDataType.GDNN);
            var lConfig = _configRepo.GetByFilter(filter);
            if (lConfig.Any())
            {
                if (lConfig.Any(x => x.time == t))
                    return (0, null);

                _configRepo.DeleteMany(filter);
            }

            var lStock = _stockRepo.GetAll();
            var date = new DateTimeOffset(dt.Year, dt.Month, dt.Day, 0, 0, 0, TimeSpan.FromHours(0)).ToUnixTimeSeconds();
            var lForeign = _foreignRepo.GetWithFilter(1, 1, "", date);
            var flag = string.Empty;
            if (lForeign != null
                && lForeign.Any())
            {
                flag = lForeign.ElementAt(0).s;
            }

            if (!string.IsNullOrWhiteSpace(flag))
            {
                var lComplete = new List<string>();
                foreach (var itemStock in lStock)
                {
                    lComplete.Add(itemStock.MaCK);
                    if (itemStock.MaCK.Equals(flag))
                    {
                        break;
                    }
                }
                lStock = lStock.Where(x => !lComplete.Any(y => y == x.MaCK)).ToList();
            }

            var count = 0;
            foreach (var item in lStock)
            {
                try
                {
                    Thread.Sleep(200);
                    var foreignResult = await _dataService.GetForeign(item.MaCK, 1, 3, dt.ToString("dd/MM/yyyy"), dt.ToString("dd/MM/yyyy"));
                    if (foreignResult is null || foreignResult.data is null)
                        continue;

                    count += InsertGDNuocNgoai(foreignResult.ToForeign());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            #region Print
            var lMes = new List<string>();
            var strOutput = new StringBuilder();
            strOutput.AppendLine($"[Thông báo] GDNN ngày {dt.ToString("dd/MM/yyyy")}:");
            strOutput.AppendLine($"*Số bản ghi được lưu: {count} bản ghi");
            strOutput.AppendLine($"*Chi tiết:");
            lMes.Add(strOutput.ToString());
            strOutput.Clear();

            var d = new DateTimeOffset(new DateTime(dt.Year, dt.Month, dt.Day), TimeSpan.FromHours(0)).ToUnixTimeSeconds();

            var lBuy = _foreignRepo.GetByFilterDESC(Builders<Foreign>.Filter.Eq(x => x.d, d), 1, 30);
            var index = 1;
            strOutput.AppendLine($"*Top mua ròng:");
            foreach (var item in lBuy)
            {
                var net = item.fbvat - item.fsvat;
                strOutput.AppendLine($"{index}. {item.s} " +
                   $"GT: {(item.fbvat * 1000).ToString("#,##0")}/{(item.fsvat * 1000).ToString("#,##0")}| {(net > 0 ? "Mua ròng" : "Bán ròng")} {Math.Abs(net * 1000).ToString("#,##0")}đ");
                index++;
            }
            if (!string.IsNullOrWhiteSpace(strOutput.ToString()))
            {
                lMes.Add(strOutput.ToString());
            }

            var lSell = _foreignRepo.GetByFilterASC(Builders<Foreign>.Filter.Eq(x => x.d, d), 1, 30);
            index = 1;
            strOutput.AppendLine($"*Top bán ròng:");
            foreach (var item in lSell)
            {
                var net = item.fbvat - item.fsvat;
                strOutput.AppendLine($"{index}. {item.s} " +
                   $"GT: {(item.fbvat * 1000).ToString("#,##0")}/{(item.fsvat * 1000).ToString("#,##0")}| {(net > 0 ? "Mua ròng" : "Bán ròng")} {Math.Abs(net * 1000).ToString("#,##0")}đ");
                index++;
            }
            if (!string.IsNullOrWhiteSpace(strOutput.ToString()))
            {
                lMes.Add(strOutput.ToString());
            }

            return (1, lMes);
            #endregion
        }

        private int InsertGDNuocNgoai(List<Foreign> lInput)
        {
            var count = 0;
            foreach (var item in lInput)
            {
                //Check Exists
                var lFind = _foreignRepo.GetWithFilter(1, 20, item.s, item.d);
                if ((lFind ?? new List<Foreign>()).Any())
                    continue;
                _foreignRepo.InsertOne(item);
                count++;
            }
            return count;
        }
    }
}
