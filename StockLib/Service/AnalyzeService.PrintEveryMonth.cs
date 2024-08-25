﻿using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        private (double, double, double) GetSpecialData(List<ThongKe> lData, DateTime dt, EKeyTongCucThongKe key)
        {
            var Cur = lData.FirstOrDefault(x => x.key == (int)key);
            var filterByKey = Builders<ThongKe>.Filter.Eq(x => x.key, (int)key);
            var lDataFilter = _thongkeRepo.GetByFilter(filterByKey);
            var ValLastMonth = lDataFilter.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{dt.AddMonths(-1).Month.To2Digit()}"));
            var ValMonthLastYear = lDataFilter.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}"));
            var RateLastMonth = (ValLastMonth?.va ?? 0) > 0 ? Math.Round(100 * (-1 + (Cur?.va ?? 0) / (ValLastMonth?.va ?? 0)), 1) : 0;
            var RateMonthLastYear = (ValMonthLastYear?.va ?? 0) > 0 ? Math.Round(100 * (-1 + (Cur?.va ?? 0) / (ValMonthLastYear?.va ?? 0)), 1) : 0;
            return ((Cur?.va ?? 0), RateLastMonth, RateMonthLastYear);
        }

        private (double, double, double) GetSpecialData2(List<ThongKe> lData, DateTime dt, EKeyTongCucThongKe key, EKeyTongCucThongKe key2)
        {
            var Cur1 = lData.FirstOrDefault(x => x.key == (int)key);
            var Cur2 = lData.FirstOrDefault(x => x.key == (int)key2);
            var Cur = (Cur1?.va ?? 0) + (Cur2?.va ?? 0);

            var filterByKey = Builders<ThongKe>.Filter.Eq(x => x.key, (int)key);
            var lDataFilter = _thongkeRepo.GetByFilter(filterByKey);

            var filterByKey2 = Builders<ThongKe>.Filter.Eq(x => x.key, (int)key);
            var lDataFilter2 = _thongkeRepo.GetByFilter(filterByKey2);

            var ValLastMonth1 = lDataFilter.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{dt.AddMonths(-1).Month.To2Digit()}"));
            var ValLastMonth2 = lDataFilter2.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{dt.AddMonths(-1).Month.To2Digit()}"));
            var ValLastMonth = ValLastMonth1?.va ?? 0 + ValLastMonth2?.va ?? 0;

            var ValMonthLastYear1 = lDataFilter.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}"));
            var ValMonthLastYear2 = lDataFilter2.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}"));
            var ValMonthLastYear = ValMonthLastYear1?.va ?? 0 + ValMonthLastYear2?.va ?? 0;

            var RateLastMonth = ValLastMonth > 0 ? Math.Round(100 * (-1 + Cur / ValLastMonth), 1) : 0;
            var RateMonthLastYear = ValMonthLastYear > 0 ? Math.Round(100 * (-1 + Cur / ValMonthLastYear), 1) : 0;
            return (Cur, RateLastMonth, RateMonthLastYear);
        }

        private (double, double, double) GetSpecialData(DateTime dt, string textContant)
        {
            var filter = Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.SP_CongNghiep);
            var lDataSP = _thongkeRepo.GetByFilter(filter);
            var lDataCur = lDataSP.Where(x => x.d == int.Parse($"{dt.Year}{dt.Month.To2Digit()}")).OrderByDescending(x => x.va);
            var lDataKey = lDataSP.Where(x => x.content.RemoveSignVietnamese().ToUpper().Contains(textContant.ToUpper()));
            var entity = lDataCur.FirstOrDefault(x => x.content.RemoveSignVietnamese().ToUpper().Contains(textContant.ToUpper()));

            var ValLastMonth = lDataKey.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{dt.AddMonths(-1).Month.To2Digit()}"));
            var ValMonthLastYear = lDataKey.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}"));
            var RateLastMonth = (ValLastMonth?.va ?? 0) > 0 ? Math.Round(100 * (-1 + (entity?.va ?? 0) / (ValLastMonth?.va ?? 0)), 1) : 0;
            var RateMonthLastYear = (ValMonthLastYear?.va ?? 0) > 0 ? Math.Round(100 * (-1 + (entity?.va ?? 0) / (ValMonthLastYear?.va ?? 0)), 1) : 0;
            return (Math.Round((entity?.va ?? 0), 1), RateLastMonth, RateMonthLastYear);
        }

        private (double, double, double) GetSpecialData(DateTime dt, string textContant, string textContant2)
        {
            var filter = Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.SP_CongNghiep);
            var lDataSP = _thongkeRepo.GetByFilter(filter);
            var lDataCur = lDataSP.Where(x => x.d == int.Parse($"{dt.Year}{dt.Month.To2Digit()}")).OrderByDescending(x => x.va);
            var lDataKey1 = lDataSP.Where(x => x.content.RemoveSignVietnamese().ToUpper().Contains(textContant.ToUpper()));
            var entity1 = lDataCur.FirstOrDefault(x => x.content.RemoveSignVietnamese().ToUpper().Contains(textContant.ToUpper()));
            var lDataKey2 = lDataSP.Where(x => x.content.RemoveSignVietnamese().ToUpper().Contains(textContant2.ToUpper()));
            var entity2 = lDataCur.FirstOrDefault(x => x.content.RemoveSignVietnamese().ToUpper().Contains(textContant2.ToUpper()));
            var Cur = (entity1?.va ?? 0) + (entity2?.va ?? 0);

            var ValLastMonth1 = lDataKey1.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{dt.AddMonths(-1).Month.To2Digit()}"));
            var ValMonthLastYear1 = lDataKey1.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}"));
            var ValLastMonth2 = lDataKey2.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{dt.AddMonths(-1).Month.To2Digit()}"));
            var ValMonthLastYear2 = lDataKey2.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}"));

            var ValLastMonth = (ValLastMonth1?.va ?? 0) + (ValLastMonth2?.va ?? 0);
            var ValMonthLastYear = (ValMonthLastYear1?.va ?? 0) + (ValMonthLastYear2?.va ?? 0);

            var RateLastMonth = ValLastMonth > 0 ? Math.Round(100 * (-1 + Cur / ValLastMonth), 1) : 0;
            var RateMonthLastYear = ValMonthLastYear > 0 ? Math.Round(100 * (-1 + Cur / ValMonthLastYear), 1) : 0;
            return (Math.Round(Cur, 1), RateLastMonth, RateMonthLastYear);
        }

        private (double, double, double) GetSpecialData(DateTime dt, string textContant, string textContant2, string textContant3)
        {
            var filter = Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.SP_CongNghiep);
            var lDataSP = _thongkeRepo.GetByFilter(filter);
            var lDataCur = lDataSP.Where(x => x.d == int.Parse($"{dt.Year}{dt.Month.To2Digit()}")).OrderByDescending(x => x.va).Take(5);
            var lDataKey1 = lDataSP.Where(x => x.content.RemoveSignVietnamese().ToUpper().Contains(textContant.ToUpper()));
            var entity1 = lDataCur.FirstOrDefault(x => x.content.RemoveSignVietnamese().ToUpper().Contains(textContant.ToUpper()));
            var lDataKey2 = lDataSP.Where(x => x.content.RemoveSignVietnamese().ToUpper().Contains(textContant2.ToUpper()));
            var entity2 = lDataCur.FirstOrDefault(x => x.content.RemoveSignVietnamese().ToUpper().Contains(textContant2.ToUpper()));
            var lDataKey3 = lDataSP.Where(x => x.content.RemoveSignVietnamese().ToUpper().Contains(textContant3.ToUpper()));
            var entity3 = lDataCur.FirstOrDefault(x => x.content.RemoveSignVietnamese().ToUpper().Contains(textContant3.ToUpper()));
            var Cur = (entity1?.va ?? 0) + (entity2?.va ?? 0) + (entity3?.va ?? 0);

            var ValLastMonth1 = lDataKey1.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{dt.AddMonths(-1).Month.To2Digit()}"));
            var ValMonthLastYear1 = lDataKey1.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}"));
            var ValLastMonth2 = lDataKey2.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{dt.AddMonths(-1).Month.To2Digit()}"));
            var ValMonthLastYear2 = lDataKey2.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}"));
            var ValLastMonth3 = lDataKey3.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{dt.AddMonths(-1).Month.To2Digit()}"));
            var ValMonthLastYear3 = lDataKey3.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}"));

            var ValLastMonth = (ValLastMonth1?.va ?? 0) + (ValLastMonth2?.va ?? 0) + (ValLastMonth3?.va ?? 0);
            var ValMonthLastYear = (ValMonthLastYear1?.va ?? 0) + (ValMonthLastYear2?.va ?? 0) + (ValLastMonth3?.va ?? 0);

            var RateLastMonth = ValLastMonth > 0 ? Math.Round(100 * (-1 + Cur / ValLastMonth), 1) : 0;
            var RateMonthLastYear = ValMonthLastYear > 0 ? Math.Round(100 * (-1 + Cur / ValMonthLastYear), 1) : 0;
            return (Math.Round(Cur, 1), RateLastMonth, RateMonthLastYear);
        }

        private string CPIStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var GiaTieuDung = lData.FirstOrDefault(x => x.key == (int)EKeyTongCucThongKe.CPI_GiaTieuDung);
            var GiaVang = lData.FirstOrDefault(x => x.key == (int)EKeyTongCucThongKe.CPI_GiaVang);
            var GiaUSD = lData.FirstOrDefault(x => x.key == (int)EKeyTongCucThongKe.CPI_DoLa);
            var LamPhat = lData.FirstOrDefault(x => x.key == (int)EKeyTongCucThongKe.CPI_LamPhat);

            strBuilder.AppendLine($"*CPI tháng {dt.Month}:");
            strBuilder.AppendLine($"1. Chỉ số giá tiêu dùng:");
            strBuilder.AppendLine($" + Từ đầu năm: {Math.Round((GiaTieuDung?.va2 ?? 0) - 100, 1)} %");
            strBuilder.AppendLine($" + Cùng kỳ: {Math.Round((GiaTieuDung?.va ?? 0) - 100, 1)} %");
            strBuilder.AppendLine($"2. Giá Vàng:");
            strBuilder.AppendLine($" + Từ đầu năm: {Math.Round((GiaVang?.va2 ?? 0) - 100, 1)} %");
            strBuilder.AppendLine($" + Cùng kỳ: {Math.Round((GiaVang?.va ?? 0) - 100, 1)} %");
            strBuilder.AppendLine($"3. Đô la Mỹ:");
            strBuilder.AppendLine($" + Từ đầu năm: {Math.Round((GiaUSD?.va2 ?? 0) - 100, 1)} %");
            strBuilder.AppendLine($" + Cùng kỳ: {Math.Round((GiaUSD?.va ?? 0) - 100, 1)} %");
            strBuilder.AppendLine($"4. Lạm phát: {Math.Round(LamPhat?.va2 ?? 0, 1)} %");
            return strBuilder.ToString();
        }

        private string CanCanThuongMaiStr(DateTime dt, List<ThongKe> lData)
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

            strBuilder.AppendLine($"*Xuất nhập khẩu tháng {dt.Month}:");
            strBuilder.AppendLine($"+ Xuất Khẩu: {Math.Round(TongXuatKhau / 1000, 1)} tỷ USD(FDI: {Math.Round(100 * ((XuatKhauFDI?.va ?? 0) / TongXuatKhau))} %)");
            strBuilder.AppendLine($"+ Nhập Khẩu: {Math.Round(TongNhapKhau / 1000, 1)} tỷ USD(FDI: {Math.Round(100 * ((NhapKhauFDI?.va ?? 0) / TongNhapKhau))} %)");
            strBuilder.AppendLine($"+ Cán cân thương mại: {Math.Round((TongXuatKhau - TongNhapKhau) / 1000, 1)} tỷ USD");
            return strBuilder.ToString();
        }

        private string DulichStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var DuLich = GetSpecialData(lData, dt, EKeyTongCucThongKe.DuLich);
            strBuilder.AppendLine($"*Nhóm ngành du lịch:");
            strBuilder.AppendLine($"1. Khách quốc tế: {Math.Round(DuLich.Item1 / 1000000, 1)} triệu lượt khách");
            strBuilder.AppendLine($" + So với tháng trước: {DuLich.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {DuLich.Item3} %");
            return strBuilder.ToString();
        }

        private string BanleStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var BanLe = GetSpecialData(lData, dt, EKeyTongCucThongKe.BanLe);
            strBuilder.AppendLine($"*Nhóm ngành bán lẻ:");
            strBuilder.AppendLine($"1. Tổng mức bản lẻ hàng hóa: {Math.Round(BanLe.Item1 / 1000, 1)} nghìn tỷ");
            strBuilder.AppendLine($" + So với tháng trước: {BanLe.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {BanLe.Item3} %");

            var data = GetSpecialData(dt, "Sua Tuoi");
            strBuilder.AppendLine($"2. Sản lượng sữa tươi: {data.Item1}(Triệu lít)");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");

            var data2 = GetSpecialData(dt, "Sua Bot");
            strBuilder.AppendLine($"3. Sản lượng sữa bột: {data2.Item1}(Nghìn tấn)");
            strBuilder.AppendLine($" + So với tháng trước: {data2.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data2.Item3} %");

            var data3 = GetSpecialData(dt, "Bia");
            strBuilder.AppendLine($"4. Sản lượng bia: {data2.Item1}(Triệu lít)");
            strBuilder.AppendLine($" + So với tháng trước: {data2.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data2.Item3} %");
            return strBuilder.ToString();
        }

        private string DautucongStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var DauTuCong = GetSpecialData(lData, dt, EKeyTongCucThongKe.DauTuCong);
            strBuilder.AppendLine($"*Nhóm ngành đầu tư công:");
            strBuilder.AppendLine($"1. Giải ngân: {Math.Round(DauTuCong.Item1 / 1000, 1)} nghìn tỉ");
            strBuilder.AppendLine($" + So với tháng trước: {DauTuCong.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {DauTuCong.Item3} %");
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
            var Dien = lDataIIP.FirstOrDefault(x => x.content.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith("Phan Phoi Dien".Replace(" ", "").ToUpper()));

            strBuilder.AppendLine($"*Nhóm ngành điện:");
            strBuilder.AppendLine($" + So với tháng trước: {Dien.va} %");
            strBuilder.AppendLine($" + Cùng kỳ: {Dien.va2} %");
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
                var prev = lDataFDI.FirstOrDefault(x => x.d == int.Parse($"{dt.AddMonths(-1).Year}{(dt.AddMonths(-1).Month - 1).To2Digit()}") && x.content.Replace(" ", "").Equals(item.content.Replace(" ", "")));
                var last = lDataFDI.FirstOrDefault(x => x.d == int.Parse($"{dt.AddYears(-1).Year}{dt.Month.To2Digit()}") && x.content.Replace(" ", "").Equals(item.content.Replace(" ", "")));
                var ratePrev = (prev is null || prev.va <= 0) ? 0 : Math.Round(100 * (-1 + item.va / prev.va));
                var rateLast = (last is null || last.va <= 0) ? 0 : Math.Round(100 * (-1 + item.va / last.va));

                var unit = "triệu USD";
                if (item.va >= 1000)
                {
                    unit = "tỷ USD";
                    item.va = Math.Round(item.va / 1000, 1);
                }


                strBuilder.AppendLine($"{iFDI++}. {item.content}({item.va} {unit})");
                strBuilder.AppendLine($"   + So với tháng trước: {ratePrev} %");
                strBuilder.AppendLine($"   + Cùng kỳ: {rateLast} %");
            }
            return strBuilder.ToString();
        }

        private string CangbienStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var DuongBien = GetSpecialData(lData, dt, EKeyTongCucThongKe.VanTai_DuongBien);
            var DuongBo = GetSpecialData(lData, dt, EKeyTongCucThongKe.VanTai_DuongBo);
            var DuongThuy = GetSpecialData(lData, dt, EKeyTongCucThongKe.VanTai_DuongThuy);
            var DuongSat = GetSpecialData(lData, dt, EKeyTongCucThongKe.VanTai_DuongSat);
            var DuongHangKhong = GetSpecialData(lData, dt, EKeyTongCucThongKe.VanTai_HangKhong);

            strBuilder.AppendLine($"*Nhóm ngành cảng biển, Logistic:");
            strBuilder.AppendLine($"1. Vận tải đường Biển:");
            strBuilder.AppendLine($" + So với tháng trước: {DuongBien.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {DuongBien.Item3} %");
            strBuilder.AppendLine($"2. Vận tải đường Bộ:");
            strBuilder.AppendLine($" + So với tháng trước: {DuongBo.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {DuongBo.Item3} %");
            strBuilder.AppendLine($"3. Vận tải đường Thủy:");
            strBuilder.AppendLine($" + So với tháng trước: {DuongThuy.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {DuongThuy.Item3} %");
            strBuilder.AppendLine($"4. Vận tải đường Sắt:");
            strBuilder.AppendLine($" + So với tháng trước: {DuongSat.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {DuongSat.Item3} %");
            strBuilder.AppendLine($"5. Vận tải đường Hàng Không:");
            strBuilder.AppendLine($" + So với tháng trước: {DuongHangKhong.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {DuongHangKhong.Item3} %");
            return strBuilder.ToString();
        }

        private string GoStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialData(lData, dt, EKeyTongCucThongKe.XK_Go);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }

            strBuilder.AppendLine($"*Nhóm ngành gỗ:");
            strBuilder.AppendLine($"1. Xuất khẩu: {Math.Round(data.Item1, 1)} {unit}");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");

            var filter = Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.IIP);
            var lDataIIP = _thongkeRepo.GetByFilter(filter);
            var lDataCur = lDataIIP.Where(x => x.d == int.Parse($"{dt.Year}{dt.Month.To2Digit()}")).OrderByDescending(x => x.va).Take(5);
            var entity = lDataCur.FirstOrDefault(x => x.content.RemoveSignVietnamese().ToUpper().Contains("Go ".ToUpper()));
            strBuilder.AppendLine($"2. Chế biến gỗ:");
            strBuilder.AppendLine($" + So với tháng trước: {entity?.va ?? 0} %");
            strBuilder.AppendLine($" + Cùng kỳ: {entity?.va2 ?? 0} %");

            var entity2 = lDataCur.FirstOrDefault(x => x.content.RemoveSignVietnamese().ToUpper().Contains("Giuong".ToUpper()));
            strBuilder.AppendLine($"3. Giường, tủ, bàn ghế:");
            strBuilder.AppendLine($" + So với tháng trước: {entity2?.va ?? 0} %");
            strBuilder.AppendLine($" + Cùng kỳ: {entity2?.va2 ?? 0} %");

            return strBuilder.ToString();
        }

        private string CaosuStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialData(lData, dt, EKeyTongCucThongKe.XK_CaoSu);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành cao su:");
            strBuilder.AppendLine($"1. Xuất khẩu: {Math.Round(data.Item1, 1)} {unit}");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");

            var filter = Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.IIP);
            var lDataIIP = _thongkeRepo.GetByFilter(filter);
            var lDataCur = lDataIIP.Where(x => x.d == int.Parse($"{dt.Year}{dt.Month.To2Digit()}")).OrderByDescending(x => x.va).Take(5);
            var entity = lDataCur.FirstOrDefault(x => x.content.RemoveSignVietnamese().ToUpper().Contains("Cao Su".ToUpper()));
            strBuilder.AppendLine($"2. Sản xuất nhựa và cao su:");
            strBuilder.AppendLine($" + So với tháng trước: {entity?.va ?? 0} %");
            strBuilder.AppendLine($" + Cùng kỳ: {entity?.va2 ?? 0} %");
            return strBuilder.ToString();
        }

        private string ThuysanStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialData(lData, dt, EKeyTongCucThongKe.XK_ThuySan);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành thủy sản:");
            strBuilder.AppendLine($"1. Xuất khẩu: {Math.Round(data.Item1, 1)} {unit}");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");

            var data1 = GetSpecialData(dt, "Thuy Hai San");
            strBuilder.AppendLine($"2. Sản lượng thủy sản chế biến: {data1.Item1}(nghìn tấn)");
            strBuilder.AppendLine($" + So với tháng trước: {data1.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data1.Item3} %");
            return strBuilder.ToString();
        }

        private string XimangStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialData(lData, dt, EKeyTongCucThongKe.XK_Ximang);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành xi măng:");
            strBuilder.AppendLine($"1. Xuất khẩu: {Math.Round(data.Item1, 1)} {unit}");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");

            var data1 = GetSpecialData(dt, "Xi mang");
            strBuilder.AppendLine($"2. Sản lượng xi măng: {data1.Item1}(triệu tấn)");
            strBuilder.AppendLine($" + So với tháng trước: {data1.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data1.Item3} %");
            return strBuilder.ToString();
        }

        private string DuongStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"*Nhóm ngành đường:");
            var data = GetSpecialData(dt, "Duong");
            strBuilder.AppendLine($"1. Sản lượng đường: {data.Item1}(nghìn tấn)");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");
            return strBuilder.ToString();
        }

        private string ChannuoiStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialData(lData, dt, EKeyTongCucThongKe.NK_ThucAnGiaSuc);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành chăn nuôi:");
            strBuilder.AppendLine($"1. Nhập khẩu thức ăn gia súc: {Math.Round(data.Item1, 1)} {unit}");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");
            return strBuilder.ToString();
        }

        private string PhanbonStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialData(lData, dt, EKeyTongCucThongKe.NK_PhanBon);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành phân bón:");
            strBuilder.AppendLine($"1. Nhập khẩu phân bón: {Math.Round(data.Item1, 1)} {unit}");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");

            var data1 = GetSpecialData(dt, "U re", "Phan hon hop");
            strBuilder.AppendLine($"2. Sản lượng phân bón: {data1.Item1}(nghìn tấn)");
            strBuilder.AppendLine($" + So với tháng trước: {data1.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data1.Item3} %");
            return strBuilder.ToString();
        }

        private string DetmayStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var dataXK = GetSpecialData(lData, dt, EKeyTongCucThongKe.XK_DetMay);
            var unit1 = "triệu USD";
            if (dataXK.Item1 >= 1000)
            {
                unit1 = "tỷ USD";
                dataXK.Item1 = Math.Round(dataXK.Item1 / 1000, 1);
            }
            var dataNK = GetSpecialData(lData, dt, EKeyTongCucThongKe.NK_Vai);
            var unit2 = "triệu USD";
            if (dataNK.Item1 >= 1000)
            {
                unit2 = "tỷ USD";
                dataNK.Item1 = Math.Round(dataNK.Item1 / 1000, 1);
            }

            var filter = Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.IIP);
            var lDataIIP = _thongkeRepo.GetByFilter(filter);
            var lDataCur = lDataIIP.Where(x => x.d == int.Parse($"{dt.Year}{dt.Month.To2Digit()}")).OrderByDescending(x => x.va).Take(5);
            var entity = lDataCur.FirstOrDefault(x => x.content.RemoveSignVietnamese().ToUpper().Contains("Trang Phuc".ToUpper()));

            strBuilder.AppendLine($"*Nhóm ngành dệt may:");
            strBuilder.AppendLine($"1. Xuất khẩu dệt may: {Math.Round(dataXK.Item1, 1)} {unit1}");
            strBuilder.AppendLine($" + So với tháng trước: {dataXK.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {dataXK.Item3} %");
            strBuilder.AppendLine($"2. Nhập khẩu vải: {Math.Round(dataNK.Item1, 1)} {unit2}");
            strBuilder.AppendLine($" + So với tháng trước: {dataNK.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {dataNK.Item3} %");
            strBuilder.AppendLine($"3. Sản xuất trang phục:");
            strBuilder.AppendLine($" + So với tháng trước: {entity?.va ?? 0} %");
            strBuilder.AppendLine($" + Cùng kỳ: {entity?.va2 ?? 0} %");
            return strBuilder.ToString();
        }

        private string OtoStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialData(lData, dt, EKeyTongCucThongKe.NK_Oto);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành ô tô:");
            strBuilder.AppendLine($"1. Nhập khẩu ô tô: {Math.Round(data.Item1, 1)} {unit}");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");
            return strBuilder.ToString();
        }

        private string NongnghiepStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialData(lData, dt, EKeyTongCucThongKe.XK_Gao);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành nông nghiệp:");
            strBuilder.AppendLine($"1. Xuất khẩu gạo: {Math.Round(data.Item1, 1)} {unit}");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");
            return strBuilder.ToString();
        }

        private string NhuaStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialData(lData, dt, EKeyTongCucThongKe.XK_SPChatDeo);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành nhựa:");
            strBuilder.AppendLine($"1. Xuất khẩu SP chất dẻo: {Math.Round(data.Item1, 1)} {unit}");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");

            var filter = Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.IIP);
            var lDataIIP = _thongkeRepo.GetByFilter(filter);
            var lDataCur = lDataIIP.Where(x => x.d == int.Parse($"{dt.Year}{dt.Month.To2Digit()}")).OrderByDescending(x => x.va).Take(5);
            var entity = lDataCur.FirstOrDefault(x => x.content.RemoveSignVietnamese().ToUpper().Contains("Cao Su".ToUpper()));
            strBuilder.AppendLine($"2. Sản xuất nhựa và cao su:");
            strBuilder.AppendLine($" + So với tháng trước: {entity?.va ?? 0} %");
            strBuilder.AppendLine($" + Cùng kỳ: {entity?.va2 ?? 0} %");

            return strBuilder.ToString();
        }

        private string ThepStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialData2(lData, dt, EKeyTongCucThongKe.XK_SatThep, EKeyTongCucThongKe.XK_SPSatThep);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }

            strBuilder.AppendLine($"*Nhóm ngành thép:");
            strBuilder.AppendLine($"1. Xuất khẩu: {Math.Round(data.Item1, 1)} {unit}");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");

            var data2 = GetSpecialData(lData, dt, EKeyTongCucThongKe.NK_SatThep);
            var unit2 = "triệu USD";
            if (data2.Item1 >= 1000)
            {
                unit2 = "tỷ USD";
                data2.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"2. Nhập khẩu sắt thép: {Math.Round(data2.Item1, 1)} {unit2}");
            strBuilder.AppendLine($" + So với tháng trước: {data2.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data2.Item3} %");

            var data3 = GetSpecialData(lData, dt, EKeyTongCucThongKe.NK_SPSatThep);
            var unit3 = "triệu USD";
            if (data3.Item1 >= 1000)
            {
                unit3 = "tỷ USD";
                data3.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"3. Nhập khẩu SP sắt thép: {Math.Round(data3.Item1, 1)} {unit3}");
            strBuilder.AppendLine($" + So với tháng trước: {data3.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data3.Item3} %");

            var data4 = GetSpecialData(dt, "Sat", "Thep Can", "Thep Thanh");
            strBuilder.AppendLine($"4. Sản lượng Thép: {data4.Item1}(nghìn tấn)");
            strBuilder.AppendLine($" + So với tháng trước: {data4.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data4.Item3} %");
            return strBuilder.ToString();
        }

        private string HoachatStr(DateTime dt, List<ThongKe> lData)
        {
            var strBuilder = new StringBuilder();
            var data = GetSpecialData2(lData, dt, EKeyTongCucThongKe.XK_HoaChat, EKeyTongCucThongKe.XK_SPHoaChat);
            var unit = "triệu USD";
            if (data.Item1 >= 1000)
            {
                unit = "tỷ USD";
                data.Item1 = Math.Round(data.Item1 / 1000, 1);
            }
            strBuilder.AppendLine($"*Nhóm ngành hóa chất:");
            strBuilder.AppendLine($"1. Xuất khẩu: {Math.Round(data.Item1, 1)} {unit}");
            strBuilder.AppendLine($" + So với tháng trước: {data.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {data.Item3} %");
            return strBuilder.ToString();
        }

        private string PrintEveryMonth(DateTime dt)
        {
            var filter = Builders<ThongKe>.Filter.Eq(x => x.d, int.Parse($"{dt.Year}{dt.Month.To2Digit()}"));
            var lData = _thongkeRepo.GetByFilter(filter);
            if (!(lData?.Any() ?? false))
                return string.Empty;

            var strBuilder = new StringBuilder();
            if (dt.Month % 3 > 0)
            {
                strBuilder.AppendLine($"[Thông báo] Tình hình kinh tế - xã hội tháng {dt.Month}");
                strBuilder.AppendLine();
            }

            strBuilder.AppendLine(CPIStr(dt, lData));
            strBuilder.AppendLine(CanCanThuongMaiStr(dt, lData));
            strBuilder.AppendLine(ThepStr(dt, lData));
            strBuilder.AppendLine(BanleStr(dt, lData));
            strBuilder.AppendLine(ThuysanStr(dt, lData));
            strBuilder.AppendLine(CangbienStr(dt, lData));
            strBuilder.AppendLine(KCNStr(dt, lData));
            strBuilder.AppendLine(DienStr(dt, lData));
            strBuilder.AppendLine(DautucongStr(dt, lData));
            strBuilder.AppendLine(XimangStr(dt, lData));
            strBuilder.AppendLine(CaosuStr(dt, lData));
            strBuilder.AppendLine(DetmayStr(dt, lData));
            strBuilder.AppendLine(PhanbonStr(dt, lData));
            strBuilder.AppendLine(HoachatStr(dt, lData));
            strBuilder.AppendLine(NhuaStr(dt, lData));
            strBuilder.AppendLine(DulichStr(dt, lData));
            strBuilder.AppendLine(GoStr(dt, lData));
            strBuilder.AppendLine(ChannuoiStr(dt, lData));
            strBuilder.AppendLine(OtoStr(dt, lData));
            strBuilder.AppendLine(NongnghiepStr(dt, lData));
            strBuilder.AppendLine(DuongStr(dt, lData));
            return strBuilder.ToString();
        }
    }
}
