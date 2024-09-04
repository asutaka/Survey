using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        public async Task<(int, string)> TongCucHaiQuan(DateTime dt, EConfigDataType mode)
        {
            try
            {
                var builder = Builders<ConfigData>.Filter;
                var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)mode);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);

                    _configRepo.DeleteMany(filter);
                }
                var isXuatKhau = mode == EConfigDataType.TongCucHaiQuan_XK;
                var str = isXuatKhau ? "Xuất khẩu hàng hóa từ ngày" : "Nhập khẩu hàng hóa từ ngày";

                var strOutput = new StringBuilder();
                var haiquan = await _apiService.TongCucHaiQuan();
                if(haiquan is null)
                    return (0, null);

                var last = lConfig.MaxBy(x => x.t);
                var va = 0;
                foreach (var item in haiquan.arr)
                {
                    if (!item.TIEU_DE.RemoveSpace().RemoveSignVietnamese().Contains(str.RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                        continue;

                    var strSplit = item.NGAY_SO_BO.Split("/");

                    var time = int.Parse($"{strSplit[2]}{strSplit[1]}{strSplit[0]}");
                    if (time <= (last?.va ?? 0)) 
                        return (0, null);

                    va = time;
                    var stream = await _apiService.TongCucHaiQuan(item.FILE_SO_BO);
                    if (stream is null) return (0, null);

                    var timeReport = 0;
                    if (int.Parse(strSplit[0]) <= 15)
                    {
                        var month = int.Parse(strSplit[1]);
                        month--;
                        timeReport = int.Parse($"{strSplit[2]}{month.To2Digit()}16");
                    }
                    else
                    {
                        timeReport = int.Parse($"{strSplit[2]}{strSplit[1]}01");
                    }

                    var lHaiQuan = _fileService.TongCucHaiQuan(stream, isXuatKhau);
                    if (lHaiQuan?.Any() ?? false)
                    {
                        foreach (var itemHaiQuan in lHaiQuan)
                        {
                            itemHaiQuan.d = timeReport;
                            _haiquanRepo.InsertOne(itemHaiQuan);
                        }
                    }
                }

                var mes = TongCucHaiQuanPrint(dt, isXuatKhau);
                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)mode,
                    t = t,
                    va = va
                });

                return (1, mes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucHaiQuan|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }
    }
}
