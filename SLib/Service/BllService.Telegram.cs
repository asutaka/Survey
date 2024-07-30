using MongoDB.Driver;
using SLib.Model;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLib.Service
{
    public partial class BllService
    {
        public string TongTuDoanhStr()
        {
            var output = new StringBuilder();
            //var dt = DateTime.Now;
            //if (dt.Hour < 20)
            //{
            //    dt = dt.AddDays(-1);
            //}

            //FilterDefinition<TuDoanh> filter = null;
            //var builder = Builders<TuDoanh>.Filter;
            //var lFilter = new List<FilterDefinition<TuDoanh>>
            //    {
            //        builder.Eq(x => x.d, new DateTimeOffset(new DateTime(dt.Year, dt.Month, dt.Day), TimeSpan.FromHours(0)).ToUnixTimeSeconds())
            //    };
            //foreach (var item in lFilter)
            //{
            //    if (filter is null)
            //    {
            //        filter = item;
            //        continue;
            //    }
            //    filter &= item;
            //}
            //var lTuDoanh = _tudoanhRepo.GetWithFilter(1, 1000, filter);
            //if (lTuDoanh is null
            //    || !lTuDoanh.Any())
            //{
            //    output.AppendLine("[Tự doanh] Không có dữ liệu tự doanh");
            //    return output.ToString();
            //}

            //var TongMua = lTuDoanh.Sum(x => x.bva);
            //var TongBan = lTuDoanh.Sum(x => x.sva);
            //var div = TongMua - TongBan;
            //var mode = div >= 0 ? "Mua ròng" : "Bán ròng";
            //output.AppendLine($"[Tổng tự doanh({dt.ToString("dd/MM/yyyy")})] {mode} {Math.Abs((decimal)div * 1000).ToString("#,##0")}đ");
            //var lCal = lTuDoanh.Select(x => new
            //{
            //    MaCK = x.s,
            //    GiaTri = (x.bva - x.sva)
            //});
            //var MuaDongStr = string.Join(", ", lCal.OrderByDescending(x => x.GiaTri).Take(10).Select(x => $"{x.MaCK}({Math.Abs((decimal)x.GiaTri * 1000).ToString("#,##0")}đ)"));
            //var BanDongStr = string.Join(", ", lCal.OrderBy(x => x.GiaTri).Take(10).Select(x => $"{x.MaCK}({Math.Abs((decimal)x.GiaTri * 1000).ToString("#,##0")}đ)"));
            //output.AppendLine($">> Top 10 mua ròng: {MuaDongStr}");
            //output.AppendLine($">> Top 10 bán ròng: {BanDongStr}");

            return output.ToString();
        }

        public string TongGDNNStr()
        {
            var output = new StringBuilder();
            //var dt = DateTime.Now;
            //if (dt.Hour < 20)
            //{
            //    dt = dt.AddDays(-1);
            //}

            //FilterDefinition<Foreign> filter = null;
            //var builder = Builders<Foreign>.Filter;
            //var lFilter = new List<FilterDefinition<Foreign>>
            //    {
            //        builder.Eq(x => x.d, new DateTimeOffset(new DateTime(dt.Year, dt.Month, dt.Day), TimeSpan.FromHours(0)).ToUnixTimeSeconds())
            //    };
            //foreach (var item in lFilter)
            //{
            //    if (filter is null)
            //    {
            //        filter = item;
            //        continue;
            //    }
            //    filter &= item;
            //}
            //var lGDNN = _foreignRepo.GetByFilter(filter, 1, 1000);
            //if (lGDNN is null
            //    || !lGDNN.Any())
            //{
            //    output.AppendLine("[GD-NN] Không có dữ liệu giao dịch nước ngoài");
            //    return output.ToString();
            //}

            //var TongMua = lGDNN.Sum(x => x.fbvat);
            //var TongBan = lGDNN.Sum(x => x.fsvat);
            //var div = TongMua - TongBan;
            //var mode = div >= 0 ? "Mua ròng" : "Bán ròng";
            //output.AppendLine($"[Tổng GDNN({dt.ToString("dd/MM/yyyy")})] {mode} {Math.Abs(div).ToString("#,##0")}đ");
            //var MuaDongStr = string.Join(", ", lGDNN.OrderByDescending(x => x.nbsva).Take(10).Select(x => $"{x.s}({Math.Abs(x.nbsva).ToString("#,##0")}đ)"));
            //var BanDongStr = string.Join(", ", lGDNN.OrderBy(x => x.nbsva).Take(10).Select(x => $"{x.s}({Math.Abs(x.nbsva).ToString("#,##0")}đ)"));
            //output.AppendLine($">> Top 10 mua ròng: {MuaDongStr}");
            //output.AppendLine($">> Top 10 bán ròng: {BanDongStr}");

            return output.ToString();
        }

        public string TuDoanhBuildStr(string code)
        {
            var output = new StringBuilder();
            //try
            //{
            //    var dt = DateTime.Now;
            //    var firstMonth = new DateTime(dt.Year, dt.Month, 1);
            //    var firstWeek = dt.AddDays((int)DayOfWeek.Monday - (int)dt.DayOfWeek);

            //    FilterDefinition<TuDoanh> filter = null;
            //    var builder = Builders<TuDoanh>.Filter;
            //    var lFilter = new List<FilterDefinition<TuDoanh>>
            //    {
            //        builder.Eq(x => x.s, code),
            //        builder.Gte(x => x.d, new DateTimeOffset(firstMonth, TimeSpan.FromHours(0)).ToUnixTimeSeconds())
            //    };
            //    foreach (var item in lFilter)
            //    {
            //        if (filter is null)
            //        {
            //            filter = item;
            //            continue;
            //        }
            //        filter &= item;
            //    }

            //    var lTuDoanh = _tudoanhRepo.GetWithFilter(1, 30, filter);
            //    if (lTuDoanh is null
            //        || !lTuDoanh.Any())
            //    {
            //        output.AppendLine("[Tự doanh] Không có dữ liệu tự doanh");
            //        return output.ToString();
            //    }

            //    //Ngày gần nhất
            //    var dtTuDoanhMax = lTuDoanh.Max(x => x.d);
            //    var TuDoanhLast = lTuDoanh.FirstOrDefault(x => x.d >= dtTuDoanhMax);
            //    var divLast = TuDoanhLast.bvo - TuDoanhLast.svo;
            //    var modeLast = divLast >= 0 ? "Mua ròng" : "Bán ròng";
            //    output.AppendLine($"[Tự doanh ngày gần nhất: {TuDoanhLast.d.UnixTimeStampToDateTime().ToString("dd/MM/yyyy")}]");
            //    output.AppendLine($"(MUA: {TuDoanhLast.bvo.ToString("#,##0")}|BÁN: {TuDoanhLast.svo.ToString("#,##0")}) ==> {modeLast} {Math.Abs(divLast).ToString("#,##0")} cổ phiếu");
            //    //Trong Tuần
            //    var lTuDoanhWeek = lTuDoanh.Where(x => x.d >= new DateTimeOffset(new DateTime(firstWeek.Year, firstWeek.Month, firstWeek.Day), TimeSpan.FromHours(0)).ToUnixTimeSeconds());
            //    var Tuan_Mua = lTuDoanhWeek.Sum(x => x.bvo);
            //    var Tuan_Ban = lTuDoanhWeek.Sum(x => x.svo);
            //    var divTuan = Tuan_Mua - Tuan_Ban;
            //    var modeTuan = divTuan >= 0 ? "Mua ròng" : "Bán ròng";
            //    output.AppendLine($">> Trong Tuần: (MUA: {Tuan_Mua.ToString("#,##0")}|BÁN: {Tuan_Ban.ToString("#,##0")}) ==> {modeTuan} {Math.Abs(divTuan).ToString("#,##0")} cổ phiếu");
            //    //Trong Tháng
            //    var Thang_Mua = lTuDoanh.Sum(x => x.bvo);
            //    var Thang_Ban = lTuDoanh.Sum(x => x.svo);
            //    var divThang = Thang_Mua - Thang_Ban;
            //    var modeThang = divThang >= 0 ? "Mua ròng" : "Bán ròng";
            //    output.AppendLine($">> Trong Tháng: (MUA: {Thang_Mua.ToString("#,##0")}|BÁN: {Thang_Ban.ToString("#,##0")}) ==> {modeThang} {Math.Abs(divThang).ToString("#,##0")} cổ phiếu");
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"TelegramService.TuDoanhBuildStr|EXCEPTION| {ex.Message}");
            //}
            return output.ToString();
        }

        public string ForeignBuildStr(string code)
        {
            var output = new StringBuilder();
            //try
            //{
            //    var dt = DateTime.Now;
            //    var firstMonth = new DateTime(dt.Year, dt.Month, 1);
            //    var firstWeek = dt.AddDays((int)DayOfWeek.Monday - (int)dt.DayOfWeek);

            //    FilterDefinition<Foreign> filter = null;
            //    var builder = Builders<Foreign>.Filter;
            //    var lFilter = new List<FilterDefinition<Foreign>>
            //    {
            //        builder.Eq(x => x.s, code),
            //        builder.Gte(x => x.d, new DateTimeOffset(firstMonth, TimeSpan.FromHours(0)).ToUnixTimeSeconds())
            //    };
            //    foreach (var item in lFilter)
            //    {
            //        if (filter is null)
            //        {
            //            filter = item;
            //            continue;
            //        }
            //        filter &= item;
            //    }

            //    var lForeign = _foreignRepo.GetByFilter(filter, 1, 30);
            //    if (lForeign is null
            //        || !lForeign.Any())
            //    {
            //        output.AppendLine("[GD-NN] Không có dữ liệu Mua bán nước Ngoài");
            //        output.AppendLine("[Cung cầu] Không có dữ liệu cung cầu");
            //        return output.ToString();
            //    }

            //    //Ngày gần nhất
            //    var dtForeignMax = lForeign.Max(x => x.d);
            //    var ForeignLast = lForeign.FirstOrDefault(x => x.d >= dtForeignMax);
            //    var modeLast = ForeignLast.nbsvo >= 0 ? "Mua ròng" : "Bán ròng";
            //    output.AppendLine($"[GD-NN ngày: {ForeignLast.d.UnixTimeStampToDateTime().ToString("dd/MM/yyyy")}]");
            //    output.AppendLine($"(MUA: {ForeignLast.fbvot.ToString("#,##0")}|BÁN: {ForeignLast.fsvot.ToString("#,##0")}) ==> {modeLast} {Math.Abs(ForeignLast.nbsvo).ToString("#,##0")} cổ phiếu");
            //    //Trong Tuần
            //    var lForeignWeek = lForeign.Where(x => x.d >= new DateTimeOffset(new DateTime(firstWeek.Year, firstWeek.Month, firstWeek.Day), TimeSpan.FromHours(0)).ToUnixTimeSeconds());
            //    var Tuan_Mua = lForeignWeek.Sum(x => x.fbvot);
            //    var Tuan_Ban = lForeignWeek.Sum(x => x.fsvot);
            //    var divTuan = Tuan_Mua - Tuan_Ban;
            //    var modeTuan = divTuan >= 0 ? "Mua ròng" : "Bán ròng";
            //    output.AppendLine($">> Trong Tuần: (MUA: {Tuan_Mua.ToString("#,##0")}|BÁN: {Tuan_Ban.ToString("#,##0")}) ==> {modeTuan} {Math.Abs(divTuan).ToString("#,##0")} cổ phiếu");
            //    //Trong Tháng
            //    var Thang_Mua = lForeign.Sum(x => x.fbvot);
            //    var Thang_Ban = lForeign.Sum(x => x.fsvot);
            //    var divThang = Thang_Mua - Thang_Ban;
            //    var modeThang = divThang >= 0 ? "Mua ròng" : "Bán ròng";
            //    output.AppendLine($">> Trong Tháng: (MUA: {Thang_Mua.ToString("#,##0")}|BÁN: {Thang_Ban.ToString("#,##0")}) ==> {modeThang} {Math.Abs(divThang).ToString("#,##0")} cổ phiếu");

            //    //////////////////////////////////////////////////
            //    ///Cung cầu
            //    output.AppendLine();
            //    output.AppendLine($"[Cung cầu ngày: {ForeignLast.d.UnixTimeStampToDateTime().ToString("dd/MM/yyyy")}]");
            //    output.AppendLine($"    + SL Mua/ Bán: {ForeignLast.tbt.ToString("#,##0")}/ {ForeignLast.tst.ToString("#,##0")}");
            //    output.AppendLine($"    + KL Mua/ Bán: {ForeignLast.tbtvo.ToString("#,##0")}/ {ForeignLast.tstvo.ToString("#,##0")}");
            //    output.AppendLine($"    + KL Khớp/ Giá trị: {ForeignLast.tmvo.ToString("#,##0")}/ {ForeignLast.tmva.ToString("#,##0")}đ");
            //    //Tuần
            //    output.AppendLine($">> Trong Tuần:");
            //    output.AppendLine($"    + SL Mua/ Bán: {lForeignWeek.Sum(x => x.tbt).ToString("#,##0")}/ {lForeignWeek.Sum(x => x.tst).ToString("#,##0")}");
            //    output.AppendLine($"    + KL Mua/ Bán: {lForeignWeek.Sum(x => x.tbtvo).ToString("#,##0")}/ {lForeignWeek.Sum(x => x.tstvo).ToString("#,##0")}");
            //    output.AppendLine($"    + KL Khớp/ Giá trị: {lForeignWeek.Sum(x => x.tmvo).ToString("#,##0")}/ {lForeignWeek.Sum(x => x.tmva).ToString("#,##0")}đ");

            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"TelegramService.ForeignBuildStr|EXCEPTION| {ex.Message}");
            //}
            return output.ToString();
        }

        public string ThongKeThiTruongStr()
        {
            var output = new StringBuilder();
            //try
            //{
            //    var lReport = _reportRepo.GetAll();
            //    //day
            //    var lReportDay = lReport.Where(x => x.mode == (int)ETimeMode.Day);
            //    var countDayPositive = lReportDay.Count(x => x.stock.Close >= (decimal)(x.bb.Sma));
            //    var countDay = lReportDay.Count();
            //    output.AppendLine($"[Thống kê thị trường ngày: {lReportDay.Last().d.ToString("dd/MM/yyyy")}]");
            //    output.AppendLine($">> Số cp trên MA20 Ngày: {Math.Round((float)countDayPositive*100/countDay, 1)}%");
            //    //week
            //    var lReportWeek = lReport.Where(x => x.mode == (int)ETimeMode.Week);
            //    var countWeekPositive = lReportWeek.Count(x => x.stock.Close >= (decimal)(x.bb.Sma));
            //    var countWeek = lReportWeek.Count();
            //    output.AppendLine($">> Số cp trên MA20 Tuần: {Math.Round((float)countWeekPositive * 100 / countWeek, 1)}%");
            //}
            //catch(Exception ex)
            //{
            //    _logger.LogError($"TelegramService.ThongKeThiTruongStr|EXCEPTION| {ex.Message}");
            //}

            return output.ToString();
        }
    }
}
