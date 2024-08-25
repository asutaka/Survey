using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OfficeOpenXml;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        public async Task<(int, string)> TongCucThongKe(DateTime dt)
        {
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)EConfigDataType.TongCucThongKe);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var stream = await _apiService.TongCucThongKe(dt);
                if (stream is null
                    || stream.Length < 1000)
                    return (0, null);

                var dic = new Dictionary<int, string>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage(stream);
                var lSheet = package.Workbook.Worksheets;
                foreach (var sheet in lSheet)
                {
                    if (false) { }
                    else if (_lGDP.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        GDP(sheet, dt);
                    }
                    else if (_lChanNuoi.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        ChanNuoi(sheet, dt);
                    }
                    else if (_lGiaSX.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        GiaCa(sheet, dt, EKeyTongCucThongKe.GiaSX);
                    }
                    else if (_lGiaVT.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        GiaCa(sheet, dt, EKeyTongCucThongKe.GiaVT);
                    }
                    else if (_lGiaNK.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        GiaCa(sheet, dt, EKeyTongCucThongKe.GiaNK);
                    }
                    else if (_lGiaXK.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        GiaCa(sheet, dt, EKeyTongCucThongKe.GiaXK);
                    }
                    else if (_lIIP.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        IIP(sheet, dt);
                    }
                    else if (_lSPCongNghiep.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        SanPhamCongNghiep(sheet, dt);
                    }
                    else if (_lVonDauTu.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        VonDauTuNhaNuoc(sheet, dt);
                    }
                    else if (_lFDI.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        FDI(sheet, dt);
                    }
                    else if (_lBanLe.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        BanLe(sheet, dt);
                    }
                    else if (_lNK.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        NhapKhau(sheet, dt);
                    }
                    else if (_lXK.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        XuatKhau(sheet, dt);
                    }
                    else if (_lCPI.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        CPI(sheet, dt);
                    }
                    else if (_lVanTaiHangHoa.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        VanTaiHangHoa(sheet, dt);
                    }
                    else if (_lKhachQuocTe.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ", "").EndsWith(x.ToUpper().Replace(" ", ""))))
                    {
                        KhachQuocTe(sheet, dt);
                    }
                }

                if (dt.Month % 3 == 0)
                {
                    EveryQuarter(dt);
                }
                else
                {
                    var mes = EveryMonth(dt);
                    return (1, mes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucThongKe|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

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

            strBuilder.AppendLine($"*Nhóm ngành dệt may:");
            strBuilder.AppendLine($"1. Xuất khẩu dệt may: {Math.Round(dataXK.Item1, 1)} {unit1}");
            strBuilder.AppendLine($" + So với tháng trước: {dataXK.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {dataXK.Item3} %");
            strBuilder.AppendLine($"2. Nhập khẩu vải: {Math.Round(dataNK.Item1, 1)} {unit2}");
            strBuilder.AppendLine($" + So với tháng trước: {dataNK.Item2} %");
            strBuilder.AppendLine($" + Cùng kỳ: {dataNK.Item3} %");
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

        private string EveryMonth(DateTime dt)
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
            return strBuilder.ToString();
        }

        private string EveryQuarter(DateTime dt)
        {
            return string.Empty;
        }

        private List<string> _lIIP = new List<string>
        {
            "IIP",
            "IIPThang"
        };
        private void IIP(ExcelWorksheet sheet, DateTime dt)
        {
            try
            {
                var colQoQ = -1;
                var colQoQoY = -1;
                var isBanLe = false;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim() ?? string.Empty;
                        if (colQoQ < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Contains($"THANG {dt.Month}"))
                        {
                            colQoQ = j;
                            continue;
                        }

                        if (colQoQ < 0)
                            continue;

                        if (colQoQoY < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Contains($"THANG {dt.Month}"))
                        {
                            colQoQoY = j;
                            break;
                        }

                        if (colQoQoY < 0)
                            continue;

                        if (string.IsNullOrWhiteSpace(cellValueCur))
                            break;

                        var isDouble1 = double.TryParse(sheet.Cells[i, colQoQ].Value?.ToString().Trim().Replace(",", ""), out var val1);
                        var isDouble2 = double.TryParse(sheet.Cells[i, colQoQoY].Value?.ToString().Trim().Replace(",", ""), out var val2);
                        _thongkeRepo.InsertOne(new ThongKe
                        {
                            d = int.Parse($"{dt.Year}{dt.Month.To2Digit()}"),
                            key = (int)EKeyTongCucThongKe.IIP,
                            content = cellValueCur,
                            va = isDouble1 ? Math.Round(val1, 1) : 0,
                            va2 = isDouble2 ? Math.Round(val2, 1) : 0,
                        });
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.IIP|EXCEPTION| {ex.Message}");
            }
        }

        private List<string> _lSPCongNghiep = new List<string>
        {
            "SP CN",
            "SPCN",
            "SPCNThang"
        };
        private void SanPhamCongNghiep(ExcelWorksheet sheet, DateTime dt)
        {
            try
            {
                var colUocTinh = -1;
                var col = -1;
                var isBanLe = false;
                var curUnit = string.Empty;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim() ?? string.Empty;
                        if (colUocTinh < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Contains($"Uoc Tinh".ToUpper()))
                        {
                            colUocTinh = j;
                            break;
                        }

                        if (colUocTinh < 0)
                            continue;

                        if (col < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Contains($"THANG {dt.Month}"))
                        {
                            if (j == colUocTinh)
                            {
                                col = j;
                                break;
                            }
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(cellValueCur))
                            break;

                        var isDouble = double.TryParse(sheet.Cells[i, col].Value?.ToString().Trim().Replace(",", ""), out var val);
                        var curUnitLocal = sheet.Cells[i, j + 1].Value?.ToString().Trim().Replace("'", "").Replace("\"","");
                        curUnit = string.IsNullOrWhiteSpace(curUnitLocal) ? curUnit : curUnitLocal;

                        _thongkeRepo.InsertOne(new ThongKe
                        {
                            d = int.Parse($"{dt.Year}{dt.Month.To2Digit()}"),
                            key = (int)EKeyTongCucThongKe.SP_CongNghiep,
                            content = $"{cellValueCur}({curUnit})",
                            va = isDouble ? Math.Round(val, 1) : 0
                        });
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.SanPhamCongNghiep|EXCEPTION| {ex.Message}");
            }
        }

        private List<string> _lVonDauTu = new List<string>
        {
            "VDT",
            "Von Dau Tu",
            "VDT TTNSNN"
        };
        private void VonDauTuNhaNuoc(ExcelWorksheet sheet, DateTime dt)
        {
            try
            {
                var col = -1;
                var isTong = false;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim().RemoveSignVietnamese().ToUpper() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(cellValueCur.ToString()))
                            continue;

                        if (col < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Contains($"THANG {dt.Month}"))
                        {
                            col = j;
                            break;
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (!isTong)
                        {
                            isTong = InsertThongKe(EKeyTongCucThongKe.DauTuCong, "TONG SO", cellValueCur, i, col, dt, sheet);
                        }

                        if (isTong)
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.VonDauTuNhaNuoc|EXCEPTION| {ex.Message}");
            }
        }

        private List<string> _lFDI = new List<string>
        {
            "FDI"
        };
        private void FDI(ExcelWorksheet sheet, DateTime dt)
        {
            try
            {
                var col = -1;
                var isDiaPhuong = false;
                var isLanhTho = false;
                var colName = -1;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(cellValueCur.ToString()))
                            continue;

                        if (col < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Contains($"Von Dang Ky".ToUpper()))
                        {
                            col = j;
                            break;
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (!isDiaPhuong)
                        {
                            isDiaPhuong = cellValueCur.RemoveSignVietnamese().ToUpper().Contains("Dia Phuong".ToUpper());
                            if (isDiaPhuong)
                            {
                                colName = j;
                                break;
                            }
                        }

                        if (!isDiaPhuong)
                            continue;

                        if (!isLanhTho)
                        {
                            isLanhTho = cellValueCur.RemoveSignVietnamese().ToUpper().Contains("Lanh Tho".ToUpper());
                            if (isLanhTho)
                                return;
                        }

                        var isDouble = double.TryParse(sheet.Cells[i, col].Value?.ToString().Trim().Replace(",", ""), out var val);
                        _thongkeRepo.InsertOne(new ThongKe
                        {
                            d = int.Parse($"{dt.Year}{dt.Month.To2Digit()}"),
                            key = (int)EKeyTongCucThongKe.FDI,
                            content = sheet.Cells[i, colName + 1].Value?.ToString().Trim(),
                            va = isDouble ? Math.Round(val, 1) : 0
                        });
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.FDI|EXCEPTION| {ex.Message}");
            }
        }

        private List<string> _lBanLe = new List<string>
        {
            "Tongmuc"
        };

        private void BanLe(ExcelWorksheet sheet, DateTime dt)
        {
            try
            {
                var colUocTinh = -1;
                var col = -1;
                var isBanLe = false;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim().RemoveSignVietnamese().ToUpper() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(cellValueCur.ToString()))
                            continue;

                        if (colUocTinh < 0 && cellValueCur.Contains($"Uoc Tinh".ToUpper()))
                        {
                            colUocTinh = j;
                            break;
                        }
                        if (col < 0 && cellValueCur.Contains($"THANG {dt.Month}"))
                        {
                            if(j == colUocTinh)
                            {
                                col = j;
                                break;
                            }
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (!isBanLe)
                        {
                            isBanLe = InsertThongKe(EKeyTongCucThongKe.BanLe, "Ban Le", cellValueCur, i, col, dt, sheet);
                            if(isBanLe)
                                return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.BanLe|EXCEPTION| {ex.Message}");
            }
        }

        private List<string> _lXK = new List<string>
        {
            "XK",
            "Xuat Khau",
            "XK Thang"
        };
        private void XuatKhau(ExcelWorksheet sheet, DateTime dt)
        {
            try
            {
                var col = -1;
                var isTrongNuoc = false;
                var isNuocNgoai = false;
                var isThuySan = false;
                var isGao = false;
                var isXimang = false;
                var isHoaChat = false;
                var isSPHoaChat = false;
                var isSPChatDeo = false;
                var isCaoSu = false;
                var isGo = false;
                var isDetMay = false;
                var isSatThep = false;
                var isSPSatThep = false;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim().RemoveSignVietnamese().ToUpper() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(cellValueCur.ToString()))
                            continue;

                        if (col < 0 && cellValueCur.Equals($"THANG {dt.Month}"))
                        {
                            col = j;
                            break;
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (!isTrongNuoc)
                        {
                            isTrongNuoc = InsertThongKe(EKeyTongCucThongKe.XK_TrongNuoc, "Trong Nuoc", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isNuocNgoai)
                        {
                            isNuocNgoai = InsertThongKe(EKeyTongCucThongKe.XK_FDI, "NN", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isThuySan)
                        {
                            isThuySan = InsertThongKe(EKeyTongCucThongKe.XK_ThuySan, "Thuy San", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isGao)
                        {
                            isGao = InsertThongKe(EKeyTongCucThongKe.XK_Gao, "Gao", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isXimang)
                        {
                            isXimang = InsertThongKe(EKeyTongCucThongKe.XK_Ximang, "Xi Mang", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isHoaChat)
                        {
                            isHoaChat = InsertThongKe(EKeyTongCucThongKe.XK_HoaChat, "Hoa Chat", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isSPHoaChat)
                        {
                            isSPHoaChat = InsertThongKe(EKeyTongCucThongKe.XK_SPHoaChat, "Hoa Chat", cellValueCur, i, col, dt, sheet, "San Pham", offset: 1);
                        }

                        if (!isSPChatDeo)
                        {
                            isSPChatDeo = InsertThongKe(EKeyTongCucThongKe.XK_SPChatDeo, "Chat Deo", cellValueCur, i, col, dt, sheet, "San Pham", offset: 1);
                        }

                        if (!isCaoSu)
                        {
                            isCaoSu = InsertThongKe(EKeyTongCucThongKe.XK_CaoSu, "Cao Su", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isGo)
                        {
                            isGo = InsertThongKe(EKeyTongCucThongKe.XK_Go, "Go", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isDetMay)
                        {
                            isDetMay = InsertThongKe(EKeyTongCucThongKe.XK_DetMay, "Det", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isSatThep)
                        {
                            isSatThep = InsertThongKe(EKeyTongCucThongKe.XK_SatThep, "Sat Thep", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isSPSatThep)
                        {
                            isSPSatThep = InsertThongKe(EKeyTongCucThongKe.XK_SPSatThep, "Sat Thep", cellValueCur, i, col, dt, sheet, offset: 1, textCompare2: "San Pham");
                            if (isSPSatThep)
                                return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.XuatKhau|EXCEPTION| {ex.Message}");
            }
        }

        private List<string> _lNK = new List<string>
        {
            "NK",
            "Nhap Khau",
            "NK Thang"
        };
        private void NhapKhau(ExcelWorksheet sheet, DateTime dt)
        {
            try
            {
                var col = -1;
                var isTrongNuoc = false;
                var isNuocNgoai = false;
                var isThucAnGiaSuc = false;
                var isPhanBon = false;
                var isVai = false;
                var isSatThep = false;
                var isSPSatThep = false;
                var isOto = false;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim().RemoveSignVietnamese().ToUpper() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(cellValueCur.ToString()))
                            continue;

                        if (col < 0 && cellValueCur.Equals($"THANG {dt.Month}"))
                        {
                            col = j;
                            break;
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (!isTrongNuoc)
                        {
                            isTrongNuoc = InsertThongKe(EKeyTongCucThongKe.NK_TrongNuoc, "Trong Nuoc", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isNuocNgoai)
                        {
                            isNuocNgoai = InsertThongKe(EKeyTongCucThongKe.NK_FDI, "NN", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isThucAnGiaSuc)
                        {
                            isThucAnGiaSuc = InsertThongKe(EKeyTongCucThongKe.NK_ThucAnGiaSuc, "Thuc An Gia Suc", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isPhanBon)
                        {
                            isPhanBon = InsertThongKe(EKeyTongCucThongKe.NK_PhanBon, "Phan Bon", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isVai)
                        {
                            isVai = InsertThongKe(EKeyTongCucThongKe.NK_Vai, "Vai", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (cellValueCur.Contains("Phe Lieu".ToUpper()))
                            continue;

                        if (!isSatThep)
                        {
                            isSatThep = InsertThongKe(EKeyTongCucThongKe.NK_SatThep, "Sat Thep", cellValueCur, i, col, dt, sheet, offset: 1);
                        }

                        if (!isSPSatThep)
                        {
                            isSPSatThep = InsertThongKe(EKeyTongCucThongKe.NK_SPSatThep, "Sat Thep", cellValueCur, i, col, dt, sheet, "San Pham", offset: 1);
                        }

                        if (!isOto)
                        {
                            isOto = InsertThongKe(EKeyTongCucThongKe.NK_Oto, "O To", cellValueCur, i, col, dt, sheet, offset: 1);
                            if(isOto)
                                return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.NhapKhau|EXCEPTION| {ex.Message}");
            }
        }

        private List<string> _lCPI = new List<string>
        {
            "CPI"
        };
        private void CPI(ExcelWorksheet sheet, DateTime dt)
        {
            try
            {
                var col = -1;
                var colLastYear = -1;
                var isChiSoGiaTieuDung = false;
                var isGiaVang = false;
                var isUSD = false;
                var isLamPhat = false;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim().RemoveSignVietnamese().ToUpper() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(cellValueCur.ToString()))
                            continue;

                        if (col < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Equals($"THANG {dt.Month}"))
                        {
                            col = j;
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (colLastYear < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Equals($"THANG 12"))
                        {
                            colLastYear = j;
                            break;
                        }

                        if (colLastYear < 0)
                        {
                            continue;
                        }

                        if (!isChiSoGiaTieuDung)
                        {
                            isChiSoGiaTieuDung = InsertThongKe(EKeyTongCucThongKe.CPI_GiaTieuDung, "Chi So Gia Tieu Dung", cellValueCur, i, col, dt, sheet, col2: colLastYear);
                        }

                        if (!isGiaVang)
                        {
                            isGiaVang = InsertThongKe(EKeyTongCucThongKe.CPI_GiaVang, "Gia Vang", cellValueCur, i, col, dt, sheet, col2: colLastYear);
                        }

                        if (!isUSD)
                        {
                            isUSD = InsertThongKe(EKeyTongCucThongKe.CPI_DoLa, "Do La", cellValueCur, i, col, dt, sheet, col2: colLastYear);
                        }

                        if (!isLamPhat)
                        {
                            isLamPhat = InsertThongKe(EKeyTongCucThongKe.CPI_LamPhat, "Lam Phat", cellValueCur, i, col, dt, sheet, col2: col + 3);
                            if(isLamPhat)
                                return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.CPI|EXCEPTION| {ex.Message}");
            }
        }

        private List<string> _lVanTaiHangHoa = new List<string>
        {
            "VT HH",
            "Hang Hoa",
            "VanTai HH",
            "Van Tai HH"
        };
        private void VanTaiHangHoa(ExcelWorksheet sheet, DateTime dt)
        {
            try
            {
                var col = -1;
                var isTrongNuoc = false;
                var isNuocNgoai = false;
                var isDuongSat = false;
                var isDuongBien = false;
                var isDuongThuy = false;
                var isDuongBo = false;
                var isHangKhong = false;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim().RemoveSignVietnamese().ToUpper() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(cellValueCur.ToString()))
                            continue;

                        if (col < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Equals($"THANG {dt.Month}"))
                        {
                            col = j;
                            break;
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (!isTrongNuoc)
                        {
                            isTrongNuoc = InsertThongKe(EKeyTongCucThongKe.VanTai_TrongNuoc, "Trong Nuoc", cellValueCur, i, col, dt, sheet);
                        }

                        if (!isNuocNgoai)
                        {
                            isNuocNgoai = InsertThongKe(EKeyTongCucThongKe.VanTai_NuocNgoai, "Ngoai Nuoc", cellValueCur, i, col, dt, sheet);
                        }

                        if (!isDuongSat)
                        {
                            isDuongSat = InsertThongKe(EKeyTongCucThongKe.VanTai_DuongSat, "Duong Sat", cellValueCur, i, col, dt, sheet);
                        }

                        if (!isDuongBien)
                        {
                            isDuongBien = InsertThongKe(EKeyTongCucThongKe.VanTai_DuongBien, "Duong Bien", cellValueCur, i, col, dt, sheet);
                        }

                        if (!isDuongThuy)
                        {
                            isDuongThuy = InsertThongKe(EKeyTongCucThongKe.VanTai_DuongThuy, "Duong Thuy Noi Dia", cellValueCur, i, col, dt, sheet);
                        }

                        if (!isDuongBo)
                        {
                            isDuongBo = InsertThongKe(EKeyTongCucThongKe.VanTai_DuongBo, "Duong Bo", cellValueCur, i, col, dt, sheet);
                        }

                        if (!isHangKhong)
                        {
                            isHangKhong = InsertThongKe(EKeyTongCucThongKe.VanTai_HangKhong, "Hang Khong", cellValueCur, i, col, dt, sheet);
                            if(isHangKhong)
                                return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.VanTaiHangHoa|EXCEPTION| {ex.Message}");
            }
        }


        private List<string> _lKhachQuocTe = new List<string>
        {
            "KQT",
            "Du Lich"
        };
        private void KhachQuocTe(ExcelWorksheet sheet, DateTime dt)
        {
            try
            {
                var col = -1;
                var isTong = false;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim().RemoveSignVietnamese().ToUpper() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(cellValueCur.ToString()))
                            continue;

                        if (col < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Equals($"THANG {dt.Month}"))
                        {
                            col = j;
                            break;
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (!isTong)
                        {
                            isTong = InsertThongKe(EKeyTongCucThongKe.DuLich, "TONG SO", cellValueCur, i, col, dt, sheet);
                            if (isTong)
                                return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.KhachQuocTe|EXCEPTION| {ex.Message}");
            }
        }

        private List<string> _lGDP = new List<string>
        {
            "GDP-HH",
            "GDPHH"
        };
        private void GDP(ExcelWorksheet sheet, DateTime dt)
        {
            try
            {
                var quarter = dt.GetQuarter();
                var quarterStr = dt.GetQuarterStr();

                var colUocTinh = -1;
                var col = -1;
                var isBanLe = false;
                var curUnit = "Tỷ";
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim() ?? string.Empty;
                        if (colUocTinh < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Contains($"Uoc Tinh".ToUpper()))
                        {
                            colUocTinh = j;
                            break;
                        }

                        if (colUocTinh < 0)
                            continue;

                        if (col < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Contains($"Quy {quarterStr}".ToUpper()))
                        {
                            if (j == colUocTinh)
                            {
                                col = j;
                                break;
                            }
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(cellValueCur)
                            || j > 2)
                            continue;

                        var isDouble = double.TryParse(sheet.Cells[i, col].Value?.ToString().Trim().Replace(",", ""), out var val);

                        _thongkeQuyRepo.InsertOne(new ThongKeQuy
                        {
                            d = int.Parse($"{dt.Year}{quarter}"),
                            key = (int)EKeyTongCucThongKe.GDP,
                            content = $"{cellValueCur}({curUnit})",
                            va = isDouble ? Math.Round(val, 1) : 0
                        });
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.GDP|EXCEPTION| {ex.Message}");
            }
        }

        private List<string> _lChanNuoi = new List<string>
        {
            "ChanNuoi"
        };
        private void ChanNuoi(ExcelWorksheet sheet, DateTime dt)
        {
            try
            {
                var quarter = dt.GetQuarter();
                var quarterStr = dt.GetQuarterStr();

                var colUocTinh = -1;
                var col = -1;
                var isBanLe = false;
                var curUnit = "Nghìn Tấn";
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim() ?? string.Empty;
                        if (colUocTinh < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Contains($"Uoc Tinh".ToUpper()))
                        {
                            colUocTinh = j;
                            break;
                        }

                        if (colUocTinh < 0)
                            continue;

                        if (col < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Contains($"Quy {quarterStr}".ToUpper()))
                        {
                            if (j == colUocTinh)
                            {
                                col = j;
                                break;
                            }
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(cellValueCur)
                            || j > 1)
                            continue;

                        if(cellValueCur.RemoveSignVietnamese().ToUpper().Contains($"Thit Lon".ToUpper()))
                        {
                            var isDouble = double.TryParse(sheet.Cells[i, col].Value?.ToString().Trim().Replace(",", ""), out var val);

                            _thongkeQuyRepo.InsertOne(new ThongKeQuy
                            {
                                d = int.Parse($"{dt.Year}{quarter}"),
                                key = (int)EKeyTongCucThongKe.ChanNuoiLon,
                                content = $"{cellValueCur}({curUnit})",
                                va = isDouble ? Math.Round(val, 1) : 0
                            });
                            return;
                        }    
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.GDP|EXCEPTION| {ex.Message}");
            }
        }

        private List<string> _lGiaSX = new List<string>
        {
            "Gia SX"
        };
        private List<string> _lGiaVT = new List<string>
        {
            "Gia VT",
            "Gia Van Tai"
        };
        private List<string> _lGiaXK = new List<string>
        {
            "Gia XK"
        };
        private List<string> _lGiaNK = new List<string>
        {
            "Gia NK"
        };

        private void GiaCa(ExcelWorksheet sheet, DateTime dt, EKeyTongCucThongKe type)
        {
            try
            {
                var quarter = dt.GetQuarter();
                var quarterStr = dt.GetQuarterStr();

                var col = -1;
                var isBanLe = false;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    //loop all columns in a row
                    for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
                    {
                        //do something with the current cell value
                        var cellValueCur = sheet.Cells[i, j].Value?.ToString().Trim() ?? string.Empty;

                        if (col < 0 && cellValueCur.RemoveSignVietnamese().ToUpper().Contains($"Quy {quarterStr}".ToUpper()))
                        {
                            col = j;
                            break;
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(cellValueCur)
                            || j > 1)
                            continue;

                        var isDouble = double.TryParse(sheet.Cells[i, col].Value?.ToString().Trim().Replace(",", ""), out var val);
                        if (!isDouble || val <= 0)
                            continue;

                        _thongkeQuyRepo.InsertOne(new ThongKeQuy
                        {
                            d = int.Parse($"{dt.Year}{quarter}"),
                            key = (int)type,
                            content = cellValueCur,
                            va = isDouble ? Math.Round(val, 1) : 0
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.GiaCa|EXCEPTION| {ex.Message}");
            }
        }

        private bool InsertThongKe(EKeyTongCucThongKe eThongKe, string textCompare, string text, int i, int col, DateTime dt, ExcelWorksheet sheet, string textCompare2 = null, int offset = 0, int col2 = 0)
        {
            if (!text.Contains(textCompare.ToUpper()))
                return false;

            if (!string.IsNullOrWhiteSpace(textCompare2))
            {
                if (!text.Contains(textCompare2.ToUpper()))
                    return false;
            }

            var valStr = sheet.Cells[i, col + offset].Value?.ToString().Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(valStr))
            {
                var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                var isDouble2 = false;
                double val2 = 0;
                if(col2 > 0)
                {
                    isDouble2 = double.TryParse((sheet.Cells[i, col2 + offset].Value?.ToString().Trim() ?? string.Empty).Replace(",", ""), out val2);
                }    
                _thongkeRepo.InsertOne(new ThongKe
                {
                    d = int.Parse($"{dt.Year}{dt.Month.To2Digit()}"),
                    key = (int)eThongKe,
                    va = isDouble ? Math.Round(val, 1) : 0,
                    va2 = isDouble2 ? Math.Round(val2, 1) : 0
                });
            }
            return true;
        }
    }
}
