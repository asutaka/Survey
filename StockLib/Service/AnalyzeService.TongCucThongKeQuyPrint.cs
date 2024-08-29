﻿using MongoDB.Driver;
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

        private string CanCanThuongMaiStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var NhapKhauTrongNuoc = lData.FirstOrDefault(x => x.key == (int)EKeyTongCucThongKe.NK_TrongNuoc);
            var NhapKhauFDI = lData.FirstOrDefault(x => x.key == (int)EKeyTongCucThongKe.NK_FDI);
            var TongNhapKhau = (NhapKhauTrongNuoc?.va ?? 0) + (NhapKhauFDI?.va ?? 0);
            if (TongNhapKhau <= 0)
            {
                TongNhapKhau = 1;
            }

            var XuatKhauTrongNuoc = lData.FirstOrDefault(x => x.key == (int)EKeyTongCucThongKe.XK_TrongNuoc);
            var XuatKhauFDI = lData.FirstOrDefault(x => x.key == (int)EKeyTongCucThongKe.XK_FDI);
            var TongXuatKhau = (XuatKhauTrongNuoc?.va ?? 0) + (XuatKhauFDI?.va ?? 0);
            if (TongXuatKhau <= 0)
            {
                TongXuatKhau = 1;
            }

            strBuilder.AppendLine($"*Xuất nhập khẩu:");
            strBuilder.AppendLine($"+ Xuất Khẩu: {Math.Round(TongXuatKhau / 1000, 1)} tỷ USD(FDI: {Math.Round(100 * ((XuatKhauFDI?.va ?? 0) / TongXuatKhau))} %)");
            strBuilder.AppendLine($"+ Nhập Khẩu: {Math.Round(TongNhapKhau / 1000, 1)} tỷ USD(FDI: {Math.Round(100 * ((NhapKhauFDI?.va ?? 0) / TongNhapKhau))} %)");
            strBuilder.AppendLine($"+ Cán cân thương mại: {Math.Round((TongXuatKhau - TongNhapKhau) / 1000, 1)} tỷ USD");
            return strBuilder.ToString();
        }

        private string ThepStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.XK_SatThep, EKeyTongCucThongKe.XK_SPSatThep);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }

            strBuilder.AppendLine($"*Nhóm ngành thép:");
            strBuilder.AppendLine($"1. Xuất khẩu: {Math.Round(data.Item1, 1)} {unit} (cùng kỳ: {data.Item3}%)");

            var dataGiaXK = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaXK_SatThep);
            strBuilder.AppendLine($"    Giá xuất khẩu: (quý trước: {dataGiaXK.Item2}%, cùng kỳ: {dataGiaXK.Item3}%)");

            var data2 = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.NK_SatThep);
            var unit2 = "triệu USD";
            if (data2.Item1 >= 1000)
            {
                unit2 = "tỷ USD";
                data2.Item1 = Math.Round(data2.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"2. Nhập khẩu sắt thép: {Math.Round(data2.Item1, 1)} {unit2}(cùng kỳ: {data2.Item3}%)");

            var data3 = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.NK_SPSatThep);
            var unit3 = "triệu USD";
            if (data3.Item1 >= 1000)
            {
                unit3 = "tỷ USD";
                data3.Item1 = Math.Round(data3.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"3. Nhập khẩu SP sắt thép: {Math.Round(data3.Item1, 1)} {unit3}(cùng kỳ: {data3.Item3}%)");

            var dataGiaNK = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaNK_SatThep);
            strBuilder.AppendLine($"    Giá nhập khẩu:(quý trước: {dataGiaNK.Item2}%, cùng kỳ: {dataGiaNK.Item3}%)");

            var data4 = GetDataSPCN_Quy(dt, "Sat", "Thep Can", "Thep Thanh");
            strBuilder.AppendLine($"4. Sản lượng Thép: {data4.Item1} nghìn tấn(cùng kỳ: {data4.Item2}%)");
            return strBuilder.ToString();
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

        private string ThuysanStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"*Nhóm ngành thủy sản:");
            var dataGDP = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GDP_ThuySan);
            strBuilder.AppendLine($"1. GDP Thủy sản: {dataGDP.Item1.ToString("#,##0")} tỷ({Math.Round(dataGDP.Item3 + 100, 1)}% GDP)");

            var data = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.XK_ThuySan);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
           
            strBuilder.AppendLine($"2. Xuất khẩu: {Math.Round(data.Item1, 1)} {unit}(cùng kỳ: {data.Item3}%)");

            var dataGiaXK = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaXK_ThuySan);
            strBuilder.AppendLine($"    Giá xuất khẩu:(quý trước: {dataGiaXK.Item2}%, cùng kỳ: {dataGiaXK.Item3}%)");

            var dataSLCa = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.ThuySan_Ca);
            strBuilder.AppendLine($"3. Sản lượng Cá: {dataSLCa.Item1} nghìn tấn(cùng kỳ: {dataSLCa.Item3}%)");

            var dataSLTom = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.ThuySan_Tom);
            strBuilder.AppendLine($"4. Sản lượng Tôm: {dataSLTom.Item1} nghìn tấn(cùng kỳ: {dataSLTom.Item3}%)");
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
            var dataGDP = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GDP_VanTaiKhoBai);
            strBuilder.AppendLine($"1. GDP Vận tải-kho bãi: {dataGDP.Item1.ToString("#,##0")} tỷ({Math.Round(dataGDP.Item3 + 100, 1)}% GDP)");

            strBuilder.AppendLine($"2. Vận tải đường Biển(cùng kỳ: {DuongBien.Item3}%)");

            var dataGiaVTBien = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_Bien);
            strBuilder.AppendLine($"    Giá Vận tải đường biển:(quý trước: {dataGiaVTBien.Item2}%, cùng kỳ: {dataGiaVTBien.Item3}%)");

            strBuilder.AppendLine($"3. Vận tải đường Bộ(cùng kỳ: {DuongBo.Item3}%):");

            var dataGiaVTBo = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_DuongBo);
            strBuilder.AppendLine($"    Giá Vận tải đường bộ:(quý trước: {dataGiaVTBo.Item2}%, cùng kỳ: {dataGiaVTBo.Item3}%)");

            strBuilder.AppendLine($"4. Vận tải đường Thủy(cùng kỳ: {DuongThuy.Item3}%):");

            var dataGiaVTThuy = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_DuongThuy);
            strBuilder.AppendLine($"    Giá Vận tải Thủy:(quý trước: {dataGiaVTThuy.Item2}%, cùng kỳ: {dataGiaVTThuy.Item3}%)");

            strBuilder.AppendLine($"5. Vận tải đường Sắt(cùng kỳ: {DuongSat.Item3}%):");

            var dataGiaVTSat = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_DuongSat);
            strBuilder.AppendLine($"    Giá Vận tải đường Sắt:(quý trước: {dataGiaVTSat.Item2}%, cùng kỳ: {dataGiaVTSat.Item3}%)");

            strBuilder.AppendLine($"6. Vận tải đường Hàng Không(cùng kỳ: {DuongHangKhong.Item3}%):");

            var dataGiaVTHangKhong = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_HangKhong);
            strBuilder.AppendLine($"    Giá Vận tải Hàng không:(quý trước: {dataGiaVTHangKhong.Item2}%, cùng kỳ: {dataGiaVTHangKhong.Item3}%)");

            var dataGiaBuuChinh = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_BuuChinh);
            strBuilder.AppendLine($"7. Giá Vận tải chuyển phát:(quý trước: {dataGiaBuuChinh.Item2}%, cùng kỳ: {dataGiaBuuChinh.Item3}%)");

            var dataGiaKhoBai = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaVT_KhoBai);
            strBuilder.AppendLine($"8. Giá cước kho bãi:(quý trước: {dataGiaKhoBai.Item2}%, cùng kỳ: {dataGiaKhoBai.Item3}%)");

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

            var dataGDP = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GDP_XayDung);
            strBuilder.AppendLine($"1. GDP Xây dựng: {dataGDP.Item1.ToString("#,##0")} tỷ({Math.Round(dataGDP.Item3 + 100, 1)}% GDP)");

            var DauTuCong = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.DauTuCong);
            strBuilder.AppendLine($"2. Giải ngân: {Math.Round(DauTuCong.Item1 / 1000, 1)} nghìn tỷ(cùng kỳ: {DauTuCong.Item3}%)");
            return strBuilder.ToString();
        }

        private string XimangStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.XK_Ximang);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành xi măng:");
            strBuilder.AppendLine($"1. Xuất khẩu: {Math.Round(data.Item1, 1)} {unit}(cùng kỳ: {data.Item3}%)");

            var data1 = GetDataSPCN_Quy(dt, "Xi mang");
            strBuilder.AppendLine($"2. Sản lượng xi măng: {data1.Item1} triệu tấn(cùng kỳ: {data1.Item2}%)");
            return strBuilder.ToString();
        }

        private string CaosuStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.XK_CaoSu);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành cao su:");
            strBuilder.AppendLine($"1. Xuất khẩu: {Math.Round(data.Item1, 1)} {unit}(cùng kỳ: {data.Item3}%)");

            var dataGiaXK = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaXK_CaoSu);
            strBuilder.AppendLine($"    Giá xuất khẩu:(quý trước: {dataGiaXK.Item2}%, cùng kỳ: {dataGiaXK.Item3}%)");

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
            var CaoSu = lDataIIP.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains("Cao Su".RemoveSpace().ToUpper()));
            strBuilder.AppendLine($"2. Sản xuất nhựa và cao su: (cùng kỳ: {Math.Round((CaoSu?.qoq ?? 0) - 100, 1)}%)");
            return strBuilder.ToString();
        }

        private string DetmayStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var dataXK = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.XK_DetMay);
            var unit1 = "triệu USD";
            if (dataXK.Item1 >= 1000)
            {
                unit1 = "tỷ USD";
                dataXK.Item1 = Math.Round(dataXK.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành dệt may:");
            strBuilder.AppendLine($"1. Xuất khẩu dệt may: {Math.Round(dataXK.Item1, 1)} {unit1}(cùng kỳ: {dataXK.Item3}%)");

            var dataGiaXK = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaXK_DetMay);
            strBuilder.AppendLine($"    Giá xuất khẩu:(quý trước: {dataGiaXK.Item2}%, cùng kỳ: {dataGiaXK.Item3}%)");

            var dataNK = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.NK_Vai);
            var unit2 = "triệu USD";
            if (dataNK.Item1 >= 1000)
            {
                unit2 = "tỷ USD";
                dataNK.Item1 = Math.Round(dataNK.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"2. Nhập khẩu vải: {Math.Round(dataNK.Item1, 1)} {unit2}(cùng kỳ: {dataNK.Item3}%)");

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
            var DetMay = lDataIIP.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains("Trang Phuc".RemoveSpace().ToUpper()));
            strBuilder.AppendLine($"3. Sản xuất trang phục: (cùng kỳ: {Math.Round((DetMay?.qoq ?? 0) - 100, 1)}%)");
            return strBuilder.ToString();
        }

        private string PhanbonStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.NK_PhanBon);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành phân bón:");
            strBuilder.AppendLine($"1. Nhập khẩu phân bón: {Math.Round(data.Item1, 1)} {unit}(cùng kỳ: {data.Item3}%)");

            var data1 = GetDataSPCN_Quy(dt, "U re", "Phan hon hop");
            strBuilder.AppendLine($"2. Sản lượng phân bón: {data1.Item1} nghìn tấn(cùng kỳ: {data1.Item2}%)");

            var dataGiaXK = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaXK_PhanBon);
            strBuilder.AppendLine($"3. Giá xuất khẩu:(quý trước: {dataGiaXK.Item2}%, cùng kỳ: {dataGiaXK.Item3}%)");
            return strBuilder.ToString();
        }

        private string HoachatStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.XK_HoaChat, EKeyTongCucThongKe.XK_SPHoaChat);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành hóa chất:");
            strBuilder.AppendLine($"1. Xuất khẩu: {Math.Round(data.Item1, 1)} {unit}(cùng kỳ: {data.Item3}%)");
            return strBuilder.ToString();
        }

        private string NhuaStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.XK_SPChatDeo);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành nhựa:");
            strBuilder.AppendLine($"1. Xuất khẩu SP chất dẻo: {Math.Round(data.Item1, 1)} {unit}(cùng kỳ: {data.Item3}%)");

            var dataGiaXK = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaXK_SPChatDeo);
            strBuilder.AppendLine($"    Giá xuất khẩu:(quý trước: {dataGiaXK.Item2}%, cùng kỳ: {dataGiaXK.Item3}%)");

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
            var CaoSu = lDataIIP.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains("Cao Su".RemoveSpace().ToUpper()));
            strBuilder.AppendLine($"2. Sản xuất nhựa và cao su:(cùng kỳ: {Math.Round((CaoSu?.qoq ?? 0) - 100, 1)}%)");
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

        private string GoStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.XK_Go);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }

            strBuilder.AppendLine($"*Nhóm ngành gỗ:");
            strBuilder.AppendLine($"1. Xuất khẩu: {Math.Round(data.Item1, 1)} {unit}(cùng kỳ: {data.Item3}%)");

            var dataGiaXK = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaXK_Go);
            strBuilder.AppendLine($"    Giá xuất khẩu:(quý trước: {dataGiaXK.Item2}%, cùng kỳ: {dataGiaXK.Item3}%)");

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
            var Go = lDataIIP.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains("Go ".RemoveSpace().ToUpper()));
            strBuilder.AppendLine($"2. Chế biến gỗ: (cùng kỳ: {Math.Round((Go?.qoq ?? 0) - 100, 1)}%)");

            var Giuong = lDataIIP.FirstOrDefault(x => x.content.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains("San Xuat Giuong".RemoveSpace().ToUpper()));
            strBuilder.AppendLine($"3. Giường, tủ, bàn ghế: (cùng kỳ: {Math.Round((Giuong?.qoq ?? 0) - 100, 1)}%)");
            return strBuilder.ToString();
        }

        private string OtoStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.NK_Oto);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành ô tô:");
            strBuilder.AppendLine($"1. Nhập khẩu ô tô: {Math.Round(data.Item1, 1)} {unit}(cùng kỳ: {data.Item3}%)");

            var dataGiaNK = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaNK_Oto);
            strBuilder.AppendLine($"    Giá nhập khẩu:(quý trước: {dataGiaNK.Item2}%, cùng kỳ: {dataGiaNK.Item3}%)");
            return strBuilder.ToString();
        }

        private string NongnghiepStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetDataWithRate_Quy(lData, dt, EKeyTongCucThongKe.XK_Gao);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành nông nghiệp:");
            strBuilder.AppendLine($"1. Xuất khẩu gạo: {Math.Round(data.Item1, 1)} {unit}(cùng kỳ: {data.Item3}%)");

            var dataGiaXK = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GiaXK_Gao);
            strBuilder.AppendLine($"    Giá xuất khẩu:(quý trước: {dataGiaXK.Item2}%, cùng kỳ: {dataGiaXK.Item3}%)");
            return strBuilder.ToString();
        }

        private string NganHangStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"*Nhóm ngành tài chính - ngân hàng - bảo hiểm:");
            var dataGDP = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GDP_NganHangBaoHiem);
            strBuilder.AppendLine($"1. GDP tài chính-ngân hàng-bảo hiểm: {dataGDP.Item1.ToString("#,##0")} tỷ({Math.Round(dataGDP.Item3 + 100, 1)}% GDP)");
            return strBuilder.ToString();
        }

        private string BatDongSanStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"*Nhóm ngành Bất động sản:");
            var dataGDP = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.GDP_BatDongSan);
            strBuilder.AppendLine($"1. GDP Bất động sản: {dataGDP.Item1.ToString("#,##0")} tỷ({Math.Round(dataGDP.Item3 + 100, 1)}% GDP)");
            return strBuilder.ToString();
        }

        private string ChanNuoiStr_Quy(DateTime dt, List<ThongKeQuy> lData)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"*Nhóm ngành Chăn nuôi:");
            var dataSLLon = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.ChanNuoi_Lon);
            strBuilder.AppendLine($"1. Sản lượng Lợn: {dataSLLon.Item1} nghìn tấn(cùng kỳ: {dataSLLon.Item3}%)");

            var dataSLSua = GetCurrentData_Quy(lData, dt, EKeyTongCucThongKe.ChanNuoi_Sua);
            strBuilder.AppendLine($"2. Sản lượng Sữa: {dataSLSua.Item1} triệu lít(cùng kỳ: {dataSLSua.Item3}%)");
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

            strBuilder.AppendLine(CanCanThuongMaiStr_Quy(dt, lData));
            strBuilder.AppendLine(ThepStr_Quy(dt, lData));
            strBuilder.AppendLine(BanleStr_Quy(dt, lData));
            strBuilder.AppendLine(ThuysanStr_Quy(dt, lData));
            strBuilder.AppendLine(CangbienStr_Quy(dt, lData));
            strBuilder.AppendLine(DienStr_Quy(dt, lData));
            strBuilder.AppendLine(DautucongStr_Quy(dt, lData));
            strBuilder.AppendLine(XimangStr_Quy(dt, lData));
            strBuilder.AppendLine(NhuaStr_Quy(dt, lData));
            strBuilder.AppendLine(CaosuStr_Quy(dt, lData));
            strBuilder.AppendLine(DetmayStr_Quy(dt, lData));
            strBuilder.AppendLine(PhanbonStr_Quy(dt, lData));
            strBuilder.AppendLine(HoachatStr_Quy(dt, lData));
            strBuilder.AppendLine(DulichStr_Quy(dt, lData));
            strBuilder.AppendLine(GoStr_Quy(dt, lData));
            strBuilder.AppendLine(OtoStr_Quy(dt, lData));
            strBuilder.AppendLine(NongnghiepStr_Quy(dt, lData));
            strBuilder.AppendLine(NganHangStr_Quy(dt, lData));
            strBuilder.AppendLine(BatDongSanStr_Quy(dt, lData));
            strBuilder.AppendLine(ChanNuoiStr_Quy(dt, lData));
            return strBuilder.ToString();
        }
    }
}
