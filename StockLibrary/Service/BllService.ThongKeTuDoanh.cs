using MongoDB.Driver;
using StockLibrary.Model;
using StockLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLibrary.Service
{
    public partial class BllService
    {
       public async Task<(int, string)> SyncTuDoanhHNX()
        {
            var dt = DateTime.Now;
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.type, (int)EConfigDataType.TuDoanhHNX);
                var lConfig = _configRepo.GetByFilter(filter);
                if(lConfig.Any())
                {
                    if (lConfig.Any(x => x.time == t))
                        return (0, string.Empty);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var stream = await _dataService.GetTuDoanhHNX(EHnxExchange.NY);
                if(stream is null 
                    || stream.Length < 1000)
                    return (0, string.Empty);

                var lData = _fileService.HNX(stream);
                var count = InsertTuDoanh(lData);

                strOutput.AppendLine($"[Thông báo] Tự doanh HNX ngày {dt.ToString("dd/MM/yyyy")}:");
                strOutput.AppendLine($"*Số bản ghi được lưu: {count} bản ghi");
                strOutput.AppendLine($"*Chi tiết:");
                lData = lData.OrderByDescending(x => x.giatri_mua - x.giatri_ban).ToList();
                var index = 1;
                foreach (var item in lData)
                {
                    var net = item.giatri_mua - item.giatri_ban;
                    strOutput.AppendLine($"{index++}. {item.ma_ck} KL: {item.kl_mua.ToString("#,##0")}/{item.kl_ban.ToString("#,##0")}| " +
                        $"GT: {(item.giatri_mua * 1000).ToString("#,##0")}/{(item.giatri_ban * 1000).ToString("#,##0")}| {(net > 0 ? "Mua ròng" : "Bán ròng")} {Math.Abs(net * 1000).ToString("#,##0")}đ");
                }
                _configRepo.InsertOne(new ConfigData
                {
                    type = (int)EConfigDataType.TuDoanhHNX,
                    time = t
                });

                return (1, strOutput.ToString());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return (0, string.Empty);
        }

        public async Task<(int, string)> SyncTuDoanhUp()
        {
            var dt = DateTime.Now;
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.type, (int)EConfigDataType.TuDoanhUpcom);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.time == t))
                        return (0, string.Empty);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var stream = await _dataService.GetTuDoanhHNX(EHnxExchange.UP);
                if (stream is null
                    || stream.Length < 1000)
                    return (0, string.Empty);

                var lData = _fileService.HNX(stream);
                var count = InsertTuDoanh(lData);

                strOutput.AppendLine($"[Thông báo] Tự doanh Upcom ngày {dt.ToString("dd/MM/yyyy")}:");
                strOutput.AppendLine($"*Số bản ghi được lưu: {count} bản ghi");
                strOutput.AppendLine($"*Chi tiết:");
                lData = lData.OrderByDescending(x => x.giatri_mua - x.giatri_ban).ToList();
                var index = 1;
                foreach (var item in lData)
                {
                    var net = item.giatri_mua - item.giatri_ban;
                    strOutput.AppendLine($"{index++}. {item.ma_ck} KL: {item.kl_mua.ToString("#,##0")}/{item.kl_ban.ToString("#,##0")}| " +
                       $"GT: {(item.giatri_mua * 1000).ToString("#,##0")}/{(item.giatri_ban * 1000).ToString("#,##0")}| {(net > 0 ? "Mua ròng" : "Bán ròng")} {Math.Abs(net * 1000).ToString("#,##0")}đ");
                }
                _configRepo.InsertOne(new ConfigData
                {
                    type = (int)EConfigDataType.TuDoanhUpcom,
                    time = t
                });

                return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return (0, string.Empty);
        }

        private int InsertTuDoanh(List<TuDoanh> lInput)
        {
            var count = 0;
            foreach (var item in lInput)
            {
                //Check Exists
                var lFind = _tudoanhRepo.GetWithFilter(1, 20, item.ma_ck, item.d);
                if ((lFind ?? new List<TuDoanh>()).Any())
                    continue;
                _tudoanhRepo.InsertOne(item);
                count++;
            }
            return count;
        }

        public async Task<(int, List<string>)> SyncTuDoanhHSX()
        {
            var dt = DateTime.Now;
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.type, (int)EConfigDataType.TuDoanhHose);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.time == t))
                        return (0, null);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var stream = await _dataService.GetTuDoanhHSX();
                if (stream is null
                    || stream.Length < 1000)
                    return (0, null);

                var lData = _fileService.HSX(stream);
                var count = InsertTuDoanh(lData);

                strOutput.AppendLine($"[Thông báo] Tự doanh HOSE ngày {dt.ToString("dd/MM/yyyy")}:");
                strOutput.AppendLine($"*Số bản ghi được lưu: {count} bản ghi");
                strOutput.AppendLine($"*Chi tiết:");
                lData = lData.OrderByDescending(x => x.giatri_mua - x.giatri_ban).ToList();
                var countData = lData.Count();
                var lFlag = new List<int>();
                if(countData > 20)
                {
                    lFlag.Add(10);
                }
                if(countData > 60)
                {
                    lFlag.Add(50);
                }
                if(countData > 100)
                {
                    lFlag.Add(100);
                }

                var index = 1;
                var lMes = new List<string>();
                foreach (var item in lData)
                {
                    var net = item.giatri_mua - item.giatri_ban;
                    strOutput.AppendLine($"{index}. {item.ma_ck} KL: {item.kl_mua.ToString("#,##0")}/{item.kl_ban.ToString("#,##0")}| " +
                       $"GT: {(item.giatri_mua * 1000).ToString("#,##0")}/{(item.giatri_ban * 1000).ToString("#,##0")}| {(net > 0 ? "Mua ròng" : "Bán ròng")} {Math.Abs(net * 1000).ToString("#,##0")}đ");
                    if(lFlag.Contains(index))
                    {
                        lMes.Add(strOutput.ToString());
                        strOutput.Clear();
                    }
                    index++;
                }
                if(!string.IsNullOrWhiteSpace(strOutput.ToString()))
                {
                    lMes.Add(strOutput.ToString());
                }

                _configRepo.InsertOne(new ConfigData
                {
                    type = (int)EConfigDataType.TuDoanhHose,
                    time = t
                });

                return (1, lMes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return (0, null);
        }
    }
}
