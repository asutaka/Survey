using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        private (double, double, double) GetDataWithRate(List<ThongKe> lData, DateTime dt, EKeyTongCucThongKe key1, EKeyTongCucThongKe key2 = EKeyTongCucThongKe.None)
        {
            var filterByKey1 = Builders<ThongKe>.Filter.Eq(x => x.key, (int)key1);
            var lDataFilter1 = _thongkeRepo.GetByFilter(filterByKey1);

            var curVal1 = lDataFilter1.FirstOrDefault(x => x.d == int.Parse($"{dt.Year}{dt.Month.To2Digit()}"));
            var valQoQoY1 = lDataFilter1.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{dt.AddMonths(-1).Month.To2Digit()}"));
            var valQoQ1 = lDataFilter1.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}"));

            var curVal = curVal1?.va ?? 0;
            var valQoQoY = valQoQoY1?.va ?? 0;
            var valQoQ = valQoQ1?.va ?? 0;

            if (key2 != EKeyTongCucThongKe.None)
            {
                var filterByKey2 = Builders<ThongKe>.Filter.Eq(x => x.key, (int)key2);
                var lDataFilter2 = _thongkeRepo.GetByFilter(filterByKey2);
                var curVal2 = lDataFilter2.FirstOrDefault(x => x.d == int.Parse($"{dt.Year}{dt.Month.To2Digit()}"));
                var valQoQoY2 = lDataFilter2.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{dt.AddMonths(-1).Month.To2Digit()}"));
                var valQoQ2 = lDataFilter2.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}"));

                curVal += (curVal2?.va ?? 0);
                valQoQoY += (valQoQoY2?.va ?? 0);
                valQoQ += (valQoQ2?.va ?? 0);
            }

            var rateQoQoY = valQoQoY > 0 ? Math.Round(100 * (-1 + curVal / valQoQoY), 1) : 0;
            var rateQoQ = valQoQ > 0 ? Math.Round(100 * (-1 + curVal / valQoQ), 1) : 0;
            return (curVal, rateQoQoY, rateQoQ);
        }

        private string CPIStr(DateTime dt, List<ThongKe> lData)//Chuẩn
        {
            var strBuilder = new StringBuilder();
            var GiaTieuDung = lData.FirstOrDefault(x => x.key == (int)EKeyTongCucThongKe.CPI_GiaTieuDung);
            var GiaVang = lData.FirstOrDefault(x => x.key == (int)EKeyTongCucThongKe.CPI_GiaVang);
            var GiaUSD = lData.FirstOrDefault(x => x.key == (int)EKeyTongCucThongKe.CPI_DoLa);
            var LamPhat = lData.FirstOrDefault(x => x.key == (int)EKeyTongCucThongKe.CPI_LamPhat);

            strBuilder.AppendLine($"*CPI tháng {dt.Month}:");
            strBuilder.AppendLine($"1. Giá tiêu dùng:(tháng trước: {Math.Round((GiaTieuDung?.qoqoy ?? 0) - 100, 1)}%, cùng kỳ: {Math.Round((GiaTieuDung?.qoq ?? 0) - 100, 1)}%)");
            strBuilder.AppendLine($"2. Giá Vàng:(tháng trước: {Math.Round((GiaVang?.qoqoy ?? 0) - 100, 1)}%, cùng kỳ: {Math.Round((GiaVang?.qoq ?? 0) - 100, 1)}%)");
            strBuilder.AppendLine($"3. Đô la Mỹ:(tháng trước: {Math.Round((GiaUSD?.qoqoy ?? 0) - 100, 1)}%, cùng kỳ: {Math.Round((GiaUSD?.qoq ?? 0) - 100, 1)}%)");
            strBuilder.AppendLine($"4. Lạm phát:(tháng trước: {Math.Round((LamPhat?.qoqoy ?? 0), 1)}%, cùng kỳ: {Math.Round((LamPhat?.qoq ?? 0), 1)}%)");
            return strBuilder.ToString();
        }

        private string BanleStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var BanLe = GetDataWithRate(lData, dt, EKeyTongCucThongKe.BanLe);
            strBuilder.AppendLine($"*Nhóm ngành bán lẻ:");
            strBuilder.AppendLine($"1. Tổng mức bản lẻ hàng hóa: {Math.Round(BanLe.Item1 / 1000, 1)} nghìn tỷ");
            strBuilder.AppendLine($" + So với tháng trước: {BanLe.Item2}%");
            strBuilder.AppendLine($" + Cùng kỳ: {BanLe.Item3}%");

            return strBuilder.ToString();
        }

        private string CangbienStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var DuongBien = GetDataWithRate(lData, dt, EKeyTongCucThongKe.VanTai_DuongBien);
            var DuongBo = GetDataWithRate(lData, dt, EKeyTongCucThongKe.VanTai_DuongBo);
            var DuongHangKhong = GetDataWithRate(lData, dt, EKeyTongCucThongKe.VanTai_HangKhong);

            strBuilder.AppendLine($"*Nhóm ngành cảng biển, Logistic:");
            strBuilder.AppendLine($"1. Vận tải đường Biển:");
            strBuilder.AppendLine($" + So với tháng trước: {DuongBien.Item2}%");
            strBuilder.AppendLine($" + Cùng kỳ: {DuongBien.Item3}%");
            strBuilder.AppendLine($"2. Vận tải đường Bộ:");
            strBuilder.AppendLine($" + So với tháng trước: {DuongBo.Item2}%");
            strBuilder.AppendLine($" + Cùng kỳ: {DuongBo.Item3}%");
            strBuilder.AppendLine($"3. Vận tải đường Hàng Không:");
            strBuilder.AppendLine($" + So với tháng trước: {DuongHangKhong.Item2}%");
            strBuilder.AppendLine($" + Cùng kỳ: {DuongHangKhong.Item3}%");
            return strBuilder.ToString();
        }

        private string KCNStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var filterFDI = Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.FDI);
            var lDataFDI = _thongkeRepo.GetByFilter(filterFDI);
            var lDataCur = lDataFDI.Where(x => x.d == int.Parse($"{dt.Year}{dt.Month.To2Digit()}")).OrderByDescending(x => x.va).Take(5);
            strBuilder.AppendLine($"*Nhóm ngành KCN:");
            var iFDI = 1;
            foreach (var item in lDataCur)
            {
                var qoqoy = lDataFDI.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{(dt.AddMonths(-1).Month).To2Digit()}") && x.content.Replace(" ", "").Equals(item.content.Replace(" ", "")));
                var qoq = lDataFDI.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}") && x.content.Replace(" ", "").Equals(item.content.Replace(" ", "")));
                var rateQoQoY = (qoqoy is null || qoqoy.va <= 0) ? 0 : Math.Round(100 * (-1 + item.va / qoqoy.va));
                var rateQoQ = (qoq is null || qoq.va <= 0) ? 0 : Math.Round(100 * (-1 + item.va / qoq.va));

                var unit = "triệu USD";
                if (item.va >= 1000)
                {
                    unit = "tỷ USD";
                    item.va = Math.Round(item.va / 1000, 1);
                }


                strBuilder.AppendLine($"{iFDI++}. {item.content}({item.va} {unit})");
                strBuilder.AppendLine($"   + So với tháng trước: {rateQoQoY}%");
                strBuilder.AppendLine($"   + Cùng kỳ: {rateQoQ}%");
            }
            return strBuilder.ToString();
        }

        private string DienStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            FilterDefinition<ThongKe> filterIIP = null;
            var builderIIP = Builders<ThongKe>.Filter;
            var lFilterIIP = new List<FilterDefinition<ThongKe>>()
            {
                builderIIP.Eq(x => x.d, int.Parse($"{dt.Year}{dt.Month.To2Digit()}")),
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
            var lDataIIP = _thongkeRepo.GetByFilter(filterIIP);
            var Dien = lDataIIP.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains("Phan Phoi Dien".RemoveSpace().ToUpper()));

            strBuilder.AppendLine($"*Nhóm ngành điện:");
            strBuilder.AppendLine($" + So với tháng trước: {Math.Round((Dien?.qoqoy ?? 0) - 100, 1)}%");
            strBuilder.AppendLine($" + Cùng kỳ: {Math.Round((Dien?.qoq ?? 0) - 100, 1)}%");
            return strBuilder.ToString();
        }

        private string DautucongStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var DauTuCong = GetDataWithRate(lData, dt, EKeyTongCucThongKe.DauTuCong);
            strBuilder.AppendLine($"*Nhóm ngành đầu tư công:");
            strBuilder.AppendLine($"1. Giải ngân: {Math.Round(DauTuCong.Item1 / 1000, 1)} nghìn tỉ");
            strBuilder.AppendLine($" + So với tháng trước: {DauTuCong.Item2}%");
            strBuilder.AppendLine($" + Cùng kỳ: {DauTuCong.Item3}%");
            return strBuilder.ToString();
        }

        private string XNKStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var ThuySan = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_ThuySan);
            var CaPhe = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_CaPhe);
            var Gao = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_Gao);
            var HoaChat = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_HoaChat);
            var SPHoaChat = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_SPHoaChat);
            var ChatDeo = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_ChatDeo);
            var SPChatDeo = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_SPChatDeo);
            var CaoSu = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_CaoSu);
            var Go = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_Go);
            var DetMay = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_DetMay);
            var SatThep = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_SatThep);
            var SPSatThep = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_SPSatThep);
            var DayDien = GetDataWithRate(lData, dt, EKeyTongCucThongKe.XK_DayDien);
            strBuilder.AppendLine($"*Xuất khẩu:");
            strBuilder.AppendLine($"1. Thủy sản: {Math.Round(ThuySan.Item1, 1)} triệu USD(tháng liền trước: {ThuySan.Item2}%; cùng kỳ: {ThuySan.Item3}%)");
            strBuilder.AppendLine($"2. Cà phê: {Math.Round(CaPhe.Item1, 1)} triệu USD(tháng liền trước: {CaPhe.Item2}%; cùng kỳ: {CaPhe.Item3}%)");
            strBuilder.AppendLine($"3. Gạo: {Math.Round(Gao.Item1, 1)} triệu USD(tháng liền trước: {Gao.Item2}%; cùng kỳ: {Gao.Item3}%)");
            strBuilder.AppendLine($"4. Hóa chất: {Math.Round(HoaChat.Item1, 1)} triệu USD(tháng liền trước: {HoaChat.Item2}%; cùng kỳ: {HoaChat.Item3}%)");
            strBuilder.AppendLine($"5. SP hóa chất: {Math.Round(SPHoaChat.Item1, 1)} triệu USD(tháng liền trước: {SPHoaChat.Item2}%; cùng kỳ: {SPHoaChat.Item3}%)");
            strBuilder.AppendLine($"6. Chất dẻo: {Math.Round(ChatDeo.Item1, 1)} triệu USD(tháng liền trước: {ChatDeo.Item2}%; cùng kỳ: {ChatDeo.Item3}%)");
            strBuilder.AppendLine($"7. SP chất dẻo: {Math.Round(SPChatDeo.Item1, 1)} triệu USD(tháng liền trước: {SPChatDeo.Item2}%; cùng kỳ: {SPChatDeo.Item3}%)");
            strBuilder.AppendLine($"8. Cao su: {Math.Round(CaoSu.Item1, 1)} triệu USD(tháng liền trước: {CaoSu.Item2}%; cùng kỳ: {CaoSu.Item3}%)");
            strBuilder.AppendLine($"9. Gỗ: {Math.Round(Go.Item1, 1)} triệu USD(tháng liền trước: {Go.Item2}%; cùng kỳ: {Go.Item3}%)");
            strBuilder.AppendLine($"10. Dệt may: {Math.Round(DetMay.Item1, 1)} triệu USD(tháng liền trước: {DetMay.Item2}%; cùng kỳ: {DetMay.Item3}%)");
            strBuilder.AppendLine($"11. Sắt thép: {Math.Round(SatThep.Item1, 1)} triệu USD(tháng liền trước: {SatThep.Item2}%; cùng kỳ: {SatThep.Item3}%)");
            strBuilder.AppendLine($"12. SP sắt thép: {Math.Round(SPSatThep.Item1, 1)} triệu USD(tháng liền trước: {SPSatThep.Item2}%; cùng kỳ: {SPSatThep.Item3}%)");
            strBuilder.AppendLine($"13. Dây điện: {Math.Round(DayDien.Item1, 1)} triệu USD(tháng liền trước: {DayDien.Item2}%; cùng kỳ: {DayDien.Item3}%)");

            var Oto = GetDataWithRate(lData, dt, EKeyTongCucThongKe.NK_Oto);
            strBuilder.AppendLine($"*Nhập khẩu:");
            strBuilder.AppendLine($"1. Ô tô: {Math.Round(Oto.Item1, 1)} triệu USD(tháng liền trước: {Oto.Item2}%; cùng kỳ: {Oto.Item3}%)");

            return strBuilder.ToString();
        }

        private string TongCucThongKeThangPrint(DateTime dt)
        {
            var filter = Builders<ThongKe>.Filter.Eq(x => x.d, int.Parse($"{dt.Year}{dt.Month.To2Digit()}"));
            var lData = _thongkeRepo.GetByFilter(filter);
            if (!(lData?.Any() ?? false))
                return string.Empty;

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"[Thông báo] Tình hình kinh tế - xã hội tháng {dt.Month}");
            strBuilder.AppendLine();

            strBuilder.AppendLine(CPIStr(dt, lData));
            strBuilder.AppendLine(BanleStr(dt, lData));
            strBuilder.AppendLine(CangbienStr(dt, lData));
            strBuilder.AppendLine(KCNStr(dt, lData));
            strBuilder.AppendLine(DienStr(dt, lData));
            strBuilder.AppendLine(DautucongStr(dt, lData));
            strBuilder.AppendLine(XNKStr(dt, lData));
            return strBuilder.ToString();
        }
    }
}


