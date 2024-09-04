using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        private (double, double, double) GetCurrentData_Quy(List<ThongKeQuy> lData, DateTime dt, EKeyTongCucThongKe key1)
        {
            var filterByKey1 = Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)key1);
            var lDataFilter1 = _thongkeQuyRepo.GetByFilter(filterByKey1);
            var curVal1 = lDataFilter1.FirstOrDefault(x => x.d == int.Parse($"{dt.Year}{dt.GetQuarter()}"));
            return (Math.Round((curVal1?.va ?? 0), 1), Math.Round((curVal1?.qoqoy ?? 0) - 100, 1), Math.Round((curVal1?.qoq ?? 0) - 100, 1));
        }

        private (double, double, double) GetDataWithRate_Quy(List<ThongKeQuy> lData, DateTime dt, EKeyTongCucThongKe key1, EKeyTongCucThongKe key2 = EKeyTongCucThongKe.None)
        {
            var filterByKey1 = Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)key1);
            var lDataFilter1 = _thongkeQuyRepo.GetByFilter(filterByKey1);

            var curVal1 = lDataFilter1.FirstOrDefault(x => x.d == int.Parse($"{dt.Year}{dt.GetQuarter()}"));
            var valQoQoY1 = lDataFilter1.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-3).Year}{dt.AddMonths(-3).GetQuarter()}"));
            var valQoQ1 = lDataFilter1.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.GetQuarter()}"));

            var curVal = curVal1?.va ?? 0;
            var valQoQoY = valQoQoY1?.va ?? 0;
            var valQoQ = valQoQ1?.va ?? 0;

            if (key2 != EKeyTongCucThongKe.None)
            {
                var filterByKey2 = Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)key2);
                var lDataFilter2 = _thongkeQuyRepo.GetByFilter(filterByKey2);
                var curVal2 = lDataFilter2.FirstOrDefault(x => x.d == int.Parse($"{dt.Year}{dt.GetQuarter()}"));
                var valQoQoY2 = lDataFilter2.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-3).Year}{dt.AddMonths(-3).GetQuarter()}"));
                var valQoQ2 = lDataFilter2.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.GetQuarter()}"));

                curVal += (curVal2?.va ?? 0);
                valQoQoY += (valQoQoY2?.va ?? 0);
                valQoQ += (valQoQ2?.va ?? 0);
            }

            var rateQoQoY = valQoQoY > 0 ? Math.Round(100 * (-1 + curVal / valQoQoY), 1) : 0;
            var rateQoQ = valQoQ > 0 ? Math.Round(100 * (-1 + curVal / valQoQ), 1) : 0;
            return (curVal, rateQoQoY, rateQoQ);
        }

        private string CangbienStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var DuongBien = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.VanTai_DuongBien);
            var DuongBo = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.VanTai_DuongBo);
            var DuongHangKhong = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.VanTai_HangKhong);

            strBuilder.AppendLine($"*Nhóm ngành cảng biển, Logistic:");
            strBuilder.AppendLine($"1. Vận tải đường Biển(cùng kỳ: {DuongBien.Item3}%)");

            var dataGiaVTBien = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_Bien);
            strBuilder.AppendLine($"    Giá Vận tải đường biển:(quý trước: {dataGiaVTBien.Item2}%, cùng kỳ: {dataGiaVTBien.Item3}%)");

            strBuilder.AppendLine($"5. Vận tải đường Hàng Không(cùng kỳ: {DuongHangKhong.Item3}%):");

            var dataGiaVTHangKhong = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_HangKhong);
            strBuilder.AppendLine($"    Giá Vận tải Hàng không:(quý trước: {dataGiaVTHangKhong.Item2}%, cùng kỳ: {dataGiaVTHangKhong.Item3}%)");

            var dataGiaBuuChinh = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_BuuChinh);
            strBuilder.AppendLine($"6. Giá Vận tải chuyển phát:(quý trước: {dataGiaBuuChinh.Item2}%, cùng kỳ: {dataGiaBuuChinh.Item3}%)");

            var dataGiaKhoBai = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_KhoBai);
            strBuilder.AppendLine($"7. Giá cước kho bãi:(quý trước: {dataGiaKhoBai.Item2}%, cùng kỳ: {dataGiaKhoBai.Item3}%)");

            return strBuilder.ToString();
        }

        private string DienStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            FilterDefinition<ThongKeQuy> filterIIP = null;
            var builderIIP = Builders<ThongKeQuy>.Filter;
            var lFilterIIP = new List<FilterDefinition<ThongKeQuy>>()
            {
                builderIIP.Eq(x => x.d, int.Parse($"{dt.Year}{dt.GetQuarter()}")),
                builderIIP.Eq(x => x.key, (int)EKeyTongCucThongKe.IIP_Dien),
            };
            foreach (var item in lFilterIIP)
            {
                if (filterIIP is null)
                {
                    filterIIP = item;
                    continue;
                }
                filterIIP &= item;
            }
            var lDataIIP = _thongkeQuyRepo.GetByFilter(filterIIP);
            var Dien = lDataIIP.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains("Phan Phoi Dien".RemoveSpace().ToUpper()));

            strBuilder.AppendLine($"*Nhóm ngành điện:");
            strBuilder.AppendLine($"1. SX-Phân phối điện:(cùng kỳ: {Math.Round((Dien?.qoq ?? 0) - 100, 1)}%)");

            var dataGiaNVL = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaNVL_Dien);
            strBuilder.AppendLine($"    Giá điện: (quý trước: {dataGiaNVL.Item2}%, cùng kỳ: {dataGiaNVL.Item3}%)");

            return strBuilder.ToString();
        }

        private string DautucongStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"*Nhóm ngành đầu tư công:");

            var DauTuCong = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.DauTuCong);
            strBuilder.AppendLine($"1. Giải ngân: {Math.Round(DauTuCong.Item1 / 1000, 1)} nghìn tỷ(cùng kỳ: {DauTuCong.Item3}%)");
            return strBuilder.ToString();
        }

        private string TongCucThongKeQuyPrint(DateTime dt)
        {
            var filter = Builders<ThongKeQuy>.Filter.Eq(x => x.d, int.Parse($"{dt.Year}{dt.GetQuarter()}"));
            var lData = _thongkeQuyRepo.GetByFilter(filter);
            if (!(lData?.Any() ?? false))
                return string.Empty;

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"[Thông báo] Tình hình kinh tế - xã hội quý {dt.GetQuarterStr()}/{dt.Year}");
            strBuilder.AppendLine();

            strBuilder.AppendLine(CangbienStr_Quy(dt, lData));
            strBuilder.AppendLine(DienStr_Quy(dt, lData));
            strBuilder.AppendLine(DautucongStr_Quy(dt, lData));
            return strBuilder.ToString();
        }
    }
}
