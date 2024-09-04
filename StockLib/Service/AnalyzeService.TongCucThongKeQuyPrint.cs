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

        private string TongCucThongKeQuyPrint(DateTime dt)
        {
            var filter = Builders<ThongKeQuy>.Filter.Eq(x => x.d, int.Parse($"{dt.Year}{dt.GetQuarter()}"));
            var lData = _thongkeQuyRepo.GetByFilter(filter);
            if (!(lData?.Any() ?? false))
                return string.Empty;

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"[Thông báo] Tổng hợp giá quý {dt.GetQuarterStr()}/{dt.Year}");

            var dataGiaNVL = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaNVL_Dien);
            strBuilder.AppendLine($"1. Giá điện: (quý trước: {dataGiaNVL.Item2}%, cùng kỳ: {dataGiaNVL.Item3}%)");

            var dataGiaVTBien = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_Bien);
            strBuilder.AppendLine($"2. Giá Vận tải đường biển:(quý trước: {dataGiaVTBien.Item2}%, cùng kỳ: {dataGiaVTBien.Item3}%)");

            var dataGiaVTHangKhong = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_HangKhong);
            strBuilder.AppendLine($"3. Giá Vận tải Hàng không:(quý trước: {dataGiaVTHangKhong.Item2}%, cùng kỳ: {dataGiaVTHangKhong.Item3}%)");

            var dataGiaBuuChinh = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_BuuChinh);
            strBuilder.AppendLine($"4. Giá Vận tải chuyển phát:(quý trước: {dataGiaBuuChinh.Item2}%, cùng kỳ: {dataGiaBuuChinh.Item3}%)");

            var dataGiaKhoBai = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_KhoBai);
            strBuilder.AppendLine($"5. Giá cước kho bãi:(quý trước: {dataGiaKhoBai.Item2}%, cùng kỳ: {dataGiaKhoBai.Item3}%)");

            return strBuilder.ToString();
        }
    }
}
