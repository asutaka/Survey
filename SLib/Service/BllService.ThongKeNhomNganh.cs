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
        public async Task<(int, string)> SyncThongkeNhomNganh(E24hGDNNType type)
        {
            var dt = DateTime.Now;
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            var dTime = new DateTimeOffset(new DateTime(dt.Year, dt.Month, dt.Day)).ToUnixTimeSeconds();
            try
            {
                EConfigDataType mode;
                switch (type)
                {
                    case E24hGDNNType.week: mode = EConfigDataType.ThongKeNhomNganh_week; break;
                    case E24hGDNNType.month: mode = EConfigDataType.ThongKeNhomNganh_month; break;
                    default: mode = EConfigDataType.ThongKeNhomNganh_today; break;
                }

                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)mode);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();

                var res = await _apiService.GetDulieuNhomNganh(type);
                if (res is null
                    || !res.data.groups.Any()
                    || res.data.last_update < dTime)
                    return (0, null);

                var lData = _categoryRepo.GetAll();
                var lNhomNganhData = new List<NhomNganhAPIGroup>();
                foreach (var itemLv1 in res.data.groups)
                {
                    var findLv1 = lData.FirstOrDefault(x => x.code == itemLv1.icb_code);
                    if(findLv1 != null)
                    {
                        itemLv1.icb_name = findLv1.name;
                        lNhomNganhData.Add(itemLv1);
                        continue;
                    }

                    foreach (var itemLv2 in itemLv1.child)
                    {
                        var findLv2 = lData.FirstOrDefault(x => x.code == itemLv2.icb_code);
                        if (findLv2 != null)
                        {
                            itemLv2.icb_name = findLv2.name;
                            lNhomNganhData.Add(itemLv2);
                            continue;
                        }

                        foreach (var itemLv3 in itemLv2.child)
                        {
                            var findLv3 = lData.FirstOrDefault(x => x.code == itemLv3.icb_code);
                            if (findLv3 != null)
                            {
                                itemLv3.icb_name = findLv3.name;
                                lNhomNganhData.Add(itemLv3);
                                continue;
                            }

                            foreach (var itemLv4 in itemLv3.child)
                            {
                                var findLv4 = lData.FirstOrDefault(x => x.code == itemLv4.icb_code);
                                if (findLv4 != null)
                                {
                                    itemLv4.icb_name = findLv4.name;
                                    lNhomNganhData.Add(itemLv4);
                                }
                            }
                        }
                    }
                }


                if(!lNhomNganhData.Any())
                    return (0, null);

                lNhomNganhData = lNhomNganhData.OrderByDescending(x => (float)x.total_stock_increase / x.total_stock).Take(5).ToList();


                var head = string.Empty;
                switch (type)
                {
                    case E24hGDNNType.week:
                        head = $"[Thông báo] Nhóm ngành quan tâm trong Tuần:"; break;
                    case E24hGDNNType.month:
                        head = $"[Thông báo] Nhóm ngành quan tâm trong Tháng:"; break;
                    default:
                        head = $"[Thông báo] Nhóm ngành quan tâm ngày {dt.ToString("dd/MM/yyyy")}:"; break;
                }
                strOutput.AppendLine(head);
                var index = 1;
                foreach (var item in lNhomNganhData)
                {
                    var content = $"{index}. {item.icb_name}({Math.Round((float)item.total_stock_increase * 100 / item.total_stock, 1)}%)";
                    strOutput.AppendLine(content);
                    index++;
                }

                switch (type)
                {
                    case E24hGDNNType.week:
                        {
                            _configRepo.InsertOne(new ConfigData
                            {
                                ty = (int)EConfigDataType.ThongKeNhomNganh_week,
                                t = t
                            });
                            break;
                        }
                    case E24hGDNNType.month:
                        {
                            _configRepo.InsertOne(new ConfigData
                            {
                                ty = (int)EConfigDataType.ThongKeNhomNganh_month,
                                t = t
                            });
                            break;
                        }
                    default:
                        {
                            _configRepo.InsertOne(new ConfigData
                            {
                                ty = (int)EConfigDataType.ThongKeNhomNganh_today,
                                t = t
                            });
                            break;
                        }
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
