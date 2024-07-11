using MongoDB.Driver;
using SLib.Model;
using SLib.Model.APIModel;
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
        public async Task<(int, string)> SyncTuDoanhHNX()
        {
            var dt = DateTime.Now;
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)EConfigDataType.TuDoanhHNX);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, string.Empty);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var stream = await _apiService.GetTuDoanhHNX(EHnxExchange.NY);
                if (stream is null
                    || stream.Length < 1000)
                    return (0, string.Empty);

                var lData = _fileService.HNX(stream);
                var lOutput = InsertTuDoanh(lData);

                strOutput.AppendLine($"[Thông báo] Tự doanh HNX ngày {dt.ToString("dd/MM/yyyy")}:");
                strOutput.AppendLine($"*Số bản ghi được lưu: {lOutput.Count()} bản ghi");
                strOutput.AppendLine($"*Chi tiết:");
                lOutput = lOutput.OrderByDescending(x => x.net).ToList();
                var index = 1;
                foreach (var item in lOutput)
                {
                    strOutput.AppendLine($"{index++}. {item.s}({(item.net > 0 ? "Mua ròng" : "Bán ròng")} {Math.Abs(item.net * 1000).ToString("#,##0")}đ)");
                }
                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)EConfigDataType.TuDoanhHNX,
                    t = t
                });

                return (1, strOutput.ToString());
            }
            catch (Exception ex)
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
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)EConfigDataType.TuDoanhUpcom);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, string.Empty);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var stream = await _apiService.GetTuDoanhHNX(EHnxExchange.UP);
                if (stream is null
                    || stream.Length < 1000)
                    return (0, string.Empty);

                var lData = _fileService.HNX(stream);
                var lOutput = InsertTuDoanh(lData);

                strOutput.AppendLine($"[Thông báo] Tự doanh Upcom ngày {dt.ToString("dd/MM/yyyy")}:");
                strOutput.AppendLine($"*Số bản ghi được lưu: {lOutput.Count()} bản ghi");
                strOutput.AppendLine($"*Chi tiết:");
                lOutput = lOutput.OrderByDescending(x => x.net).ToList();
                var index = 1;
                foreach (var item in lOutput)
                {
                    strOutput.AppendLine($"{index++}. {item.s} ({(item.net > 0 ? "Mua ròng" : "Bán ròng")} {Math.Abs(item.net * 1000).ToString("#,##0")}đ)");
                }
                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)EConfigDataType.TuDoanhUpcom,
                    t = t
                });

                return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return (0, string.Empty);
        }

        public async Task<(int, List<string>)> SyncTuDoanhHSX()
        {
            var dt = DateTime.Now;
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)EConfigDataType.TuDoanhHose);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var stream = await _apiService.GetTuDoanhHSX();
                if (stream is null
                    || stream.Length < 1000)
                    return (0, null);

                var lData = _fileService.HSX(stream);
                var lOutput = InsertTuDoanh(lData);

                strOutput.AppendLine($"[Thông báo] Tự doanh HOSE ngày {dt.ToString("dd/MM/yyyy")}:");
                strOutput.AppendLine($"*Số bản ghi được lưu: {lOutput.Count()} bản ghi");
                strOutput.AppendLine($"*Chi tiết:");
                lOutput = lOutput.OrderByDescending(x => x.net).ToList();
                var countData = lOutput.Count();
                var lFlag = new List<int>();
                if (countData > 20)
                {
                    lFlag.Add(10);
                }
                if (countData > 60)
                {
                    lFlag.Add(50);
                }
                if (countData > 100)
                {
                    lFlag.Add(100);
                }

                var index = 1;
                var lMes = new List<string>();
                foreach (var item in lOutput)
                {
                    var content = $"{index}. {item.s} ({(item.net > 0 ? "Mua ròng" : "Bán ròng")} {Math.Abs(item.net * 1000).ToString("#,##0")}đ)";
                    if(item.net_pt > 0)
                    {
                        content += $" - Thỏa thuận mua {Math.Abs(item.net_pt * 1000).ToString("#,##0")}đ";
                    }
                    else if(item.net_pt < 0)
                    {
                        content += $" - Thỏa thuận bán {Math.Abs(item.net_pt * 1000).ToString("#,##0")}đ";
                    }    

                    strOutput.AppendLine(content);
                    if (lFlag.Contains(index))
                    {
                        lMes.Add(strOutput.ToString());
                        strOutput.Clear();
                    }
                    index++;
                }
                if (!string.IsNullOrWhiteSpace(strOutput.ToString()))
                {
                    lMes.Add(strOutput.ToString());
                }

                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)EConfigDataType.TuDoanhHose,
                    t = t
                });

                return (1, lMes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return (0, null);
        }

        private List<TuDoanh> InsertTuDoanh(List<TudoanhPDF> lInput)
        {
            var lstResult = new List<TuDoanh>();
            foreach (var item in lInput)
            {
                //Check Exists
                var lFind = _tudoanhRepo.GetWithFilter(1, 20, item.s, item.d);
                if ((lFind ?? new List<TuDoanh>()).Any())
                    continue;
                var model = new TuDoanh
                {
                    no = item.no,
                    d = item.d,
                    s = item.s,
                    net_deal = item.bva - item.sva,
                    net_pt = item.bva_pt - item.sva_pt,
                    t = item.t
                };
                model.net = model.net_deal + model.net_pt;
                lstResult.Add(model);
                _tudoanhRepo.InsertOne(model);
            }
            return lstResult;
        }
    }
}