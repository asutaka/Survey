using MongoDB.Driver;
using SLib.Model;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task<(int, string)> SyncThongkeGDNN(E24hGDNNType type)
        {
            var dt = DateTime.Now;
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            var dTime = new DateTimeOffset(new DateTime(dt.Year, dt.Month, dt.Day)).ToUnixTimeSeconds();
            try
            {
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)EConfigDataType.GDNN);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var lData = new List<Foreign>();
                lData.AddRange(await _apiService.GetGDNN24H(E24HGDNNMode.HSX, type));
                lData.AddRange(await _apiService.GetGDNN24H(E24HGDNNMode.HNX, type));
                lData.AddRange(await _apiService.GetGDNN24H(E24HGDNNMode.UPCOM, type));
                if (!lData.Any())
                    return (0, null);

                var head = string.Empty;
                switch(type)
                {
                    case E24hGDNNType.week:
                        head = $"[Thông báo] GDNN trong Tuần:"; break;
                    case E24hGDNNType.month:
                        head = $"[Thông báo] GDNN trong Tháng:"; break;
                    default:
                        head = $"[Thông báo] GDNN ngày {dt.ToString("dd/MM/yyyy")}:"; break;
                }
                strOutput.AppendLine(head);
                strOutput.AppendLine($"*Top 10 mua ròng:");
                var lTopBuy = lData.OrderByDescending(x => x.net_val).Take(10);
                var lTopSell = lData.OrderBy(x => x.net_val).Take(10);
                var index = 1;
                foreach (var item in lTopBuy)
                {
                    var content = $"{index}. {item.s} (Mua ròng {Math.Abs(item.net_val).ToString("#,##0.00")} tỷ)";
                    strOutput.AppendLine(content);
                    index++;
                }

                strOutput.AppendLine();
                strOutput.AppendLine($"*Top 10 bán ròng:");
                index = 1;
                foreach (var item in lTopSell)
                {
                    var content = $"{index}. {item.s} (Bán ròng {Math.Abs(item.net_val).ToString("#,##0.00")} tỷ)";
                    strOutput.AppendLine(content);
                    index++;
                }

                if(type == E24hGDNNType.month)
                {
                    _configRepo.InsertOne(new ConfigData
                    {
                        ty = (int)EConfigDataType.GDNN,
                        t = t
                    });
                }

                return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return (0, null);
        }
    }
}
