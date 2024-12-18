﻿using Microsoft.Extensions.Logging;
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
                var flagFirst = dt.Day > 15 ? 1 : 2;
                var builder = Builders<ConfigData>.Filter;
                var t = long.Parse($"{dt.Year}{((flagFirst == 2) ? (dt.Month - 1).To2Digit() : dt.Month.To2Digit())}{flagFirst}");
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)mode);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);
                }
                var isXuatKhau = mode == EConfigDataType.TongCucHaiQuan_XK;
                var str = isXuatKhau ? "Xuất khẩu hàng hóa từ ngày" : "Nhập khẩu hàng hóa từ ngày";

                var strOutput = new StringBuilder();
                var haiquan = await _apiService.TongCucHaiQuan();
                if(haiquan is null)
                    return (0, null);
                haiquan.arr.Reverse();

                var last = lConfig.MaxBy(x => x.t);
                var va = 0;
                foreach (var item in haiquan.arr)
                {
                    if (!item.TIEU_DE.RemoveSpace().RemoveSignVietnamese().Contains(str.RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                        continue;

                    var strSplit = item.NGAY_SO_BO.Split("/");
                    var flagDetect = int.Parse(strSplit[0]) > 15 ? 1 : 2;
                    var year = int.Parse(strSplit[2]);
                    var month = int.Parse(strSplit[1]);
                    if (flagDetect == 2)
                    {
                        month--;
                    }

                    var time = int.Parse($"{year}{month.To2Digit()}{flagDetect}");
                    if (time <= (last?.t ?? 0))
                        return (0, null);

                    var stream = await _apiService.TongCucHaiQuan(item.FILE_SO_BO);
                    if (stream is null) return (0, null);

                    var lHaiQuan = _fileService.TongCucHaiQuan(stream, isXuatKhau);
                    if (lHaiQuan?.Any() ?? false)
                    {
                        foreach (var itemHaiQuan in lHaiQuan)
                        {
                            itemHaiQuan.d = time;
                            _haiquanRepo.InsertOne(itemHaiQuan);
                        }
                    }

                    t = time;
                }

                var mes = TongCucHaiQuanPrint(dt, isXuatKhau);
                if(last is null)
                {
                    _configRepo.InsertOne(new ConfigData
                    {
                        ty = (int)mode,
                        t = t
                    });
                }
                else
                {
                    last.t = t;
                    _configRepo.Update(last);
                }

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
