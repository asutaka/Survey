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

        private (double, double, double) GetDataSPCN_Quy(DateTime dt, string key1, string key2 = "", string key3 = "")
        {
            var filter = Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.SP_CongNghiep);
            var lDataSP = _thongkeQuyRepo.GetByFilter(filter);
            var lDataCur = lDataSP.Where(x => x.d == int.Parse($"{dt.Year}{dt.GetQuarter()}"));
            var entity1 = lDataCur.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(key1.RemoveSpace().ToUpper()));
            var curVal = entity1?.va ?? 0;
            var valQoQ = entity1?.qoq ?? 0;
            var valYear = entity1?.qoqoy ?? 0;

            if (!string.IsNullOrWhiteSpace(key2))
            {
                var entity2 = lDataCur.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(key2.RemoveSpace().ToUpper()));
                var curVal2 = entity2?.va ?? 0;
                if (curVal2 > 0)
                {
                    var valQoQ2 = entity2?.qoq ?? 0;
                    if (valQoQ <= 0 || valQoQ2 <= 0)
                    {
                        valQoQ = 0;
                    }
                    else
                    {
                        var prevVal = (curVal * 100 / valQoQ) + ((entity2?.va ?? 0) * 100 / valQoQ2);
                        valQoQ = Math.Round((curVal + curVal2) * 100 / prevVal, 1);
                    }

                    var valYear2 = entity2?.qoqoy ?? 0;
                    if (valYear <= 0 || valYear2 <= 0)
                    {
                        valYear = 0;
                    }
                    else
                    {
                        var prevVal = (curVal * 100 / valYear) + ((entity2?.va ?? 0) * 100 / valYear2);
                        valYear = Math.Round((curVal + curVal2) * 100 / prevVal, 1);
                    }
                    curVal += curVal2;
                }
            }

            if (!string.IsNullOrWhiteSpace(key3))
            {
                var entity3 = lDataCur.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(key3.RemoveSpace().ToUpper()));
                var curVal3 = entity3?.va ?? 0;
                if (curVal3 > 0)
                {

                    var valQoQ3 = entity3?.qoq ?? 0;
                    if (valQoQ <= 0 || valQoQ3 <= 0)
                    {
                        valQoQ = 0;
                    }
                    else
                    {
                        var prevVal = (curVal * 100 / valQoQ) + ((entity3?.va ?? 0) * 100 / valQoQ3);
                        valQoQ = Math.Round((curVal + curVal3) * 100 / prevVal, 1);
                    }

                    var valYear3 = entity3?.qoqoy ?? 0;
                    if (valYear <= 0 || valYear3 <= 0)
                    {
                        valYear = 0;
                    }
                    else
                    {
                        var prevVal = (curVal * 100 / valYear) + ((entity3?.va ?? 0) * 100 / valYear3);
                        valYear = Math.Round((curVal + curVal3) * 100 / prevVal, 1);
                    }
                    curVal += curVal3;
                }
            }

            return (Math.Round(curVal, 1), Math.Round(valQoQ - 100, 1), Math.Round(valYear - 100, 1));
        }

        private string BanleStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var BanLe = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.BanLe);
            strBuilder.AppendLine($"*Nhóm ngành bán lẻ:");
            strBuilder.AppendLine($"1. Tổng mức bản lẻ hàng hóa: {Math.Round(BanLe.Item1 / 1000, 1)} nghìn tỷ(cùng kỳ: {BanLe.Item3}%)");

            var data = GetDataSPCN_Quy(dt, "Sua Tuoi");
            strBuilder.AppendLine($"2. Sản lượng sữa tươi: {data.Item1} triệu lít(cùng kỳ: {data.Item2}%)");

            var data2 = GetDataSPCN_Quy(dt, "Sua Bot");
            strBuilder.AppendLine($"3. Sản lượng sữa bột: {data2.Item1} nghìn tấn(cùng kỳ: {data2.Item2}%)");

            var data3 = GetDataSPCN_Quy(dt, "Bia");
            strBuilder.AppendLine($"4. Sản lượng bia: {data3.Item1} triệu lít(cùng kỳ: {data3.Item2}%)");
            return strBuilder.ToString();
        }

        private string CangbienStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var DuongBien = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.VanTai_DuongBien);
            var DuongBo = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.VanTai_DuongBo);
            var DuongThuy = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.VanTai_DuongThuy);
            var DuongSat = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.VanTai_DuongSat);
            var DuongHangKhong = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.VanTai_HangKhong);

            strBuilder.AppendLine($"*Nhóm ngành cảng biển, Logistic:");
            strBuilder.AppendLine($"1. Vận tải đường Biển(cùng kỳ: {DuongBien.Item3}%)");

            var dataGiaVTBien = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_Bien);
            strBuilder.AppendLine($"    Giá Vận tải đường biển:(quý trước: {dataGiaVTBien.Item2}%, cùng kỳ: {dataGiaVTBien.Item3}%)");

            strBuilder.AppendLine($"2. Vận tải đường Bộ(cùng kỳ: {DuongBo.Item3}%):");

            var dataGiaVTBo = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_DuongBo);
            strBuilder.AppendLine($"    Giá Vận tải đường bộ:(quý trước: {dataGiaVTBo.Item2}%, cùng kỳ: {dataGiaVTBo.Item3}%)");

            strBuilder.AppendLine($"3. Vận tải đường Thủy(cùng kỳ: {DuongThuy.Item3}%):");

            var dataGiaVTThuy = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_DuongThuy);
            strBuilder.AppendLine($"    Giá Vận tải Thủy:(quý trước: {dataGiaVTThuy.Item2}%, cùng kỳ: {dataGiaVTThuy.Item3}%)");

            strBuilder.AppendLine($"4. Vận tải đường Sắt(cùng kỳ: {DuongSat.Item3}%):");

            var dataGiaVTSat = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_DuongSat);
            strBuilder.AppendLine($"    Giá Vận tải đường Sắt:(quý trước: {dataGiaVTSat.Item2}%, cùng kỳ: {dataGiaVTSat.Item3}%)");

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
                builderIIP.Eq(x => x.key, (int)EKeyTongCucThongKe.IIP),
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

        private string PhanbonStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"*Nhóm ngành phân bón:");

            var data1 = GetDataSPCN_Quy(dt, "U re", "Phan hon hop");
            strBuilder.AppendLine($"2. Sản lượng phân bón: {data1.Item1} nghìn tấn(cùng kỳ: {data1.Item2}%)");

            return strBuilder.ToString();
        }

        private string DulichStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var DuLich = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.DuLich);
            strBuilder.AppendLine($"*Nhóm ngành du lịch: ");
            strBuilder.AppendLine($"1. Khách quốc tế: {Math.Round(DuLich.Item1 / 1000000, 1)} triệu lượt(cùng kỳ: {DuLich.Item3}%)");
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

            strBuilder.AppendLine(BanleStr_Quy(dt, lData));
            strBuilder.AppendLine(CangbienStr_Quy(dt, lData));
            strBuilder.AppendLine(DienStr_Quy(dt, lData));
            strBuilder.AppendLine(DautucongStr_Quy(dt, lData));
            strBuilder.AppendLine(PhanbonStr_Quy(dt, lData));
            strBuilder.AppendLine(DulichStr_Quy(dt, lData));
            return strBuilder.ToString();
        }
    }
}
