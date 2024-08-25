using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        private string PrintEveryQuarter(DateTime dt)
        {
            var quarter = dt.GetQuarter();
            var quarterStr = dt.GetQuarterStr();

            var filter = Builders<ThongKe>.Filter.Eq(x => x.d, int.Parse($"{dt.Year}{dt.Month.To2Digit()}"));
            var lData = _thongkeRepo.GetByFilter(filter);
            if (!(lData?.Any() ?? false))
                return string.Empty;

            var strBuilder = new StringBuilder();
            if (dt.Month % 3 > 0)
            {
                strBuilder.AppendLine($"[Thông báo] Tình hình kinh tế - xã hội quý {quarterStr}");
                strBuilder.AppendLine();
            }

            //strBuilder.AppendLine(CPIStr(dt, lData));
            //strBuilder.AppendLine(CanCanThuongMaiStr(dt, lData));
            //strBuilder.AppendLine(ThepStr(dt, lData));
            //strBuilder.AppendLine(BanleStr(dt, lData));
            strBuilder.AppendLine(ThuysanQuyStr(dt, lData));
            //strBuilder.AppendLine(CangbienStr(dt, lData));
            //strBuilder.AppendLine(KCNStr(dt, lData));
            //strBuilder.AppendLine(DienStr(dt, lData));
            //strBuilder.AppendLine(DautucongStr(dt, lData));
            //strBuilder.AppendLine(XimangStr(dt, lData));
            //strBuilder.AppendLine(CaosuStr(dt, lData));
            //strBuilder.AppendLine(DetmayStr(dt, lData));
            //strBuilder.AppendLine(PhanbonStr(dt, lData));
            //strBuilder.AppendLine(HoachatStr(dt, lData));
            //strBuilder.AppendLine(NhuaStr(dt, lData));
            //strBuilder.AppendLine(DulichStr(dt, lData));
            //strBuilder.AppendLine(GoStr(dt, lData));
            strBuilder.AppendLine(ChanNuoiQuyStr(dt, lData));
            //strBuilder.AppendLine(OtoStr(dt, lData));
            //strBuilder.AppendLine(NongnghiepStr(dt, lData));
            //strBuilder.AppendLine(DuongStr(dt, lData));
            return strBuilder.ToString();
        }

        private string ThuysanQuyStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialDataQuy(lData, dt, EKeyTongCucThongKe.ThuySan);
            var unit = "Nghìn tấn";
            strBuilder.AppendLine($"*Nhóm ngành thủy sản:");
            strBuilder.AppendLine($"1. Sản lượng thủy sản: {data.Item1}(nghìn tấn)");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");
            return strBuilder.ToString();
        }
        private string ChanNuoiQuyStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialDataQuy(lData, dt, EKeyTongCucThongKe.ChanNuoiLon);
            var unit = "Nghìn tấn";
            strBuilder.AppendLine($"*Nhóm ngành chăn nuôi:");
            strBuilder.AppendLine($"1. Chăn nuôi heo: {data.Item1}(nghìn tấn)");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");
            return strBuilder.ToString();
        }


        private (double, double, double) GetSpecialDataQuy(List<ThongKe> lData, DateTime dt, EKeyTongCucThongKe key)
        {
            var quarter = dt.GetQuarter();
            quarter--;
            if (quarter <= 0)
                quarter = 4;

            var Cur = lData.FirstOrDefault(x => x.key == (int)key);
            var filterByKey = Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)key);
            var lDataFilter = _thongkeQuyRepo.GetByFilter(filterByKey);
            var ValLastQuarter = lDataFilter.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{quarter}"));
            var ValQuarterLastYear = lDataFilter.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{quarter}"));
            var RateLastQuarter = (ValLastQuarter?.va ?? 0) > 0 ? Math.Round(100 * (-1 + (Cur?.va ?? 0) / (ValLastQuarter?.va ?? 0)), 1) : 0;
            var RateQuarterLastYear = (ValQuarterLastYear?.va ?? 0) > 0 ? Math.Round(100 * (-1 + (Cur?.va ?? 0) / (ValQuarterLastYear?.va ?? 0)), 1) : 0;
            return ((Cur?.va ?? 0), RateLastQuarter, RateQuarterLastYear);
        }
    }
}
