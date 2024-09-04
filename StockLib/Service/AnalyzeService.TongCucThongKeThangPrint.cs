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

        private (double, double, double) GetDataSPCN(DateTime dt, string key1, string key2 = "", string key3 = "")
        {
            var filter = Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.SP_CongNghiep);
            var lDataSP = _thongkeRepo.GetByFilter(filter);
            var lDataCur = lDataSP.Where(x => x.d == int.Parse($"{dt.Year}{dt.Month.To2Digit()}"));
            var entity1 = lDataSP.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(key1.RemoveSpace().ToUpper()));
            var curVal = entity1?.va ?? 0;
            var valQoQ = entity1?.qoq ?? 0;
            var valYear = entity1?.qoqoy ?? 0;

            if(!string.IsNullOrWhiteSpace(key2))
            {
                var entity2 = lDataSP.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(key2.RemoveSpace().ToUpper()));
                var curVal2 = entity2?.va ?? 0;
                if(curVal2 > 0)
                {
                   
                    var valQoQ2 = entity2?.qoq ?? 0;
                    if(valQoQ <= 0 || valQoQ2 <= 0)
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
                
                if(!string.IsNullOrWhiteSpace(key3))
                {
                    var entity3 = lDataSP.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(key3.RemoveSpace().ToUpper()));
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
            }    

            return (Math.Round(curVal, 1), Math.Round(valQoQ - 100, 1), Math.Round(valYear - 100, 1));
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

            var data = GetDataSPCN(dt, "Sua Tuoi");
            strBuilder.AppendLine($"2. Sản lượng sữa tươi: {data.Item1}(Triệu lít)");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item2}%");
            strBuilder.AppendLine($" + Lũy kế so với cùng kỳ: {data.Item3}%");

            var data2 = GetDataSPCN(dt, "Sua Bot");
            strBuilder.AppendLine($"3. Sản lượng sữa bột: {data2.Item1}(Nghìn tấn)");
            strBuilder.AppendLine($" + Cùng kỳ: {data2.Item2}%");
            strBuilder.AppendLine($" + Lũy kế so với cùng kỳ: {data2.Item3}%");

            var data3 = GetDataSPCN(dt, "Bia");
            strBuilder.AppendLine($"4. Sản lượng bia: {data3.Item1}(Triệu lít)");
            strBuilder.AppendLine($" + Cùng kỳ: {data3.Item2}%");
            strBuilder.AppendLine($" + Lũy kế so với cùng kỳ: {data3.Item3}%");
            return strBuilder.ToString();
        }

        private string CangbienStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var DuongBien = GetDataWithRate(lData, dt, EKeyTongCucThongKe.VanTai_DuongBien);
            var DuongBo = GetDataWithRate(lData, dt, EKeyTongCucThongKe.VanTai_DuongBo);
            var DuongThuy = GetDataWithRate(lData, dt, EKeyTongCucThongKe.VanTai_DuongThuy);
            var DuongSat = GetDataWithRate(lData, dt, EKeyTongCucThongKe.VanTai_DuongSat);
            var DuongHangKhong = GetDataWithRate(lData, dt, EKeyTongCucThongKe.VanTai_HangKhong);

            strBuilder.AppendLine($"*Nhóm ngành cảng biển, Logistic:");
            strBuilder.AppendLine($"1. Vận tải đường Biển:");
            strBuilder.AppendLine($" + So với tháng trước: {DuongBien.Item2}%");
            strBuilder.AppendLine($" + Cùng kỳ: {DuongBien.Item3}%");
            strBuilder.AppendLine($"2. Vận tải đường Bộ:");
            strBuilder.AppendLine($" + So với tháng trước: {DuongBo.Item2}%");
            strBuilder.AppendLine($" + Cùng kỳ: {DuongBo.Item3}%");
            strBuilder.AppendLine($"3. Vận tải đường Thủy:");
            strBuilder.AppendLine($" + So với tháng trước: {DuongThuy.Item2}%");
            strBuilder.AppendLine($" + Cùng kỳ: {DuongThuy.Item3}%");
            strBuilder.AppendLine($"4. Vận tải đường Sắt:");
            strBuilder.AppendLine($" + So với tháng trước: {DuongSat.Item2}%");
            strBuilder.AppendLine($" + Cùng kỳ: {DuongSat.Item3}%");
            strBuilder.AppendLine($"5. Vận tải đường Hàng Không:");
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

        private string DulichStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var DuLich = GetDataWithRate(lData, dt, EKeyTongCucThongKe.DuLich);
            strBuilder.AppendLine($"*Nhóm ngành du lịch:");
            strBuilder.AppendLine($"1. Khách quốc tế: {Math.Round(DuLich.Item1 / 1000000, 1)} triệu lượt khách");
            strBuilder.AppendLine($" + So với tháng trước: {DuLich.Item2}%");
            strBuilder.AppendLine($" + Cùng kỳ: {DuLich.Item3}%");
            return strBuilder.ToString();
        }

        private string DuongStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"*Nhóm ngành đường:");
            var data = GetDataSPCN(dt, "Duong");
            strBuilder.AppendLine($"1. Sản lượng đường: {data.Item1}(nghìn tấn)");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item2}%");
            strBuilder.AppendLine($" + Lũy kế so với cùng kỳ: {data.Item3}%");
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
            strBuilder.AppendLine(DulichStr(dt, lData));
            strBuilder.AppendLine(DuongStr(dt, lData));
            return strBuilder.ToString();
        }
    }
}
