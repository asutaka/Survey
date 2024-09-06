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
        public async Task<(int, string)> TongCucThongKeThang(DateTime dt)
        {
            if (dt.Day <= 10)
            {
                dt = dt.AddMonths(-1);
            }
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}28");
            try
            {
                var mode = EConfigDataType.TongCucThongKeThang;
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)mode);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);
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
                    else if (_lIIP.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        IIP(sheet, dt);
                    }
                    else if (_lVonDauTu.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        VonDauTuNhaNuoc(sheet, dt);
                    }
                    else if (_lFDI.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        FDI(sheet, dt);
                    }
                    else if (_lBanLe.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        BanLe(sheet, dt);
                    }
                    else if (_lCPI.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        CPI(sheet, dt);
                    }
                    else if (_lVanTaiHanhKhach.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        VanTaiHanhKhach(sheet, dt);
                    }
                    else if (_lVanTaiHangHoa.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        VanTaiHangHoa(sheet, dt);
                    }
                }

                var mes = TongCucThongKeThangPrint(dt);
                var last = lConfig.Last();
                if (last is null)
                {
                    _configRepo.InsertOne(new ConfigData
                    {
                        ty = (int)mode,
                        t = t
                    });
                }
                else
                {
                    last.t = t;
                    _configRepo.Update(last);
                }

                return (1, mes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucThongKeThang|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        public async Task<(int, string)> TongCucThongKeThangTest(DateTime dt)
        {
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)EConfigDataType.TongCucThongKeThang);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var stream = await _apiService.TongCucThongKeTest(dt);
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
                    else if (_lIIP.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        IIP(sheet, dt);
                    }
                    else if (_lVonDauTu.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        VonDauTuNhaNuoc(sheet, dt);
                    }
                    else if (_lFDI.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        FDI(sheet, dt);
                    }
                    else if (_lBanLe.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        BanLe(sheet, dt);
                    }
                    else if (_lCPI.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        CPI(sheet, dt);
                    }
                    else if (_lVanTaiHanhKhach.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        VanTaiHanhKhach(sheet, dt);
                    }
                    else if (_lVanTaiHangHoa.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        VanTaiHangHoa(sheet, dt);
                    }
                }

                //var mes = TongCucThongKeThangPrint(dt);
                //return (1, mes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucThongKeThang|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        private List<string> _lIIP = new List<string>
        {
            "IIP",
            "IIPThang"
        };
        private void IIP(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord(EKeyTongCucThongKe.IIP_Dien, dt, sheet, colContent: 1, colVal: -1, colQoQ: 4, colQoQoY: 5, colUnit: -1, textCompare: "Phan Phoi Dien");
        }

        private List<string> _lVonDauTu = new List<string>
        {
            "VDT",
            "Von Dau Tu",
            "NSNN",
            "NSNN Thang",
            "Von DT"
        };
        private void VonDauTuNhaNuoc(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord(EKeyTongCucThongKe.DauTuCong, dt, sheet, colContent: 1, colVal: 4, colQoQ: 7, colQoQoY: 6, colUnit: -1, "Tong So");
        }

        private List<string> _lFDI = new List<string>
        {
            "FDI"
        };
        private void FDI(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeSomeRecord(EKeyTongCucThongKe.FDI, dt, sheet, colContent: 2, colVal: 4, colQoQ: -1, colQoQoY: -1, colUnit: -1, keyStart: "Dia Phuong", colKeyStart: 1, keyEnd: "Lanh Tho", colKeyEnd: 1);
        }

        private List<string> _lBanLe = new List<string>
        {
            "Tongmuc"
        };

        private void BanLe(ExcelWorksheet sheet, DateTime dt)
        {
            var res = InsertThongKeOnlyRecord(EKeyTongCucThongKe.BanLe, dt, sheet, colContent: 1, colVal: 3, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Ban Le");
            if(!res)
            {
                if(dt.Month == 1)
                {
                    InsertThongKeOnlyRecord(EKeyTongCucThongKe.BanLe, dt, sheet, colContent: 2, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Ban Le");
                }
                else
                {
                    InsertThongKeOnlyRecord(EKeyTongCucThongKe.BanLe, dt, sheet, colContent: 2, colVal: 4, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Ban Le");
                }
            }
        }

        private List<string> _lCPI = new List<string>
        {
            "CPI"
        };
        private void CPI(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord(EKeyTongCucThongKe.CPI_GiaTieuDung, dt, sheet, colContent: 1, colVal: -1, colQoQ: 5, colQoQoY: 7, colUnit: -1, "Chi So Gia Tieu Dung");
            InsertThongKeOnlyRecord(EKeyTongCucThongKe.CPI_GiaVang, dt, sheet, colContent: 1, colVal: -1, colQoQ: 5, colQoQoY: 7, colUnit: -1, "Chi So Gia Vang");
            InsertThongKeOnlyRecord(EKeyTongCucThongKe.CPI_DoLa, dt, sheet, colContent: 1, colVal: -1, colQoQ: 5, colQoQoY: 7, colUnit: -1, "Do La");
            InsertThongKeOnlyRecord(EKeyTongCucThongKe.CPI_LamPhat, dt, sheet, colContent: 1, colVal: -1, colQoQ: 5, colQoQoY: 7, colUnit: -1, "Lam Phat");
        }

        private List<string> _lVanTaiHanhKhach = new List<string>
        {
            "VT HK",
            "Hanh Khach",
            "VanTai HK",
            "Van Tai HK"
        };
        private void VanTaiHanhKhach(ExcelWorksheet sheet, DateTime dt)
        {
            var resHangKhong = InsertThongKeOnlyRecord(EKeyTongCucThongKe.HanhKhach_HangKhong, dt, sheet, colContent: 1, colVal: 2, colQoQ: 5, colQoQoY: 4, colUnit: -1, "Hang Khong");
            if (!resHangKhong)
            {
                InsertThongKeOnlyRecord(EKeyTongCucThongKe.HanhKhach_HangKhong, dt, sheet, colContent: 2, colVal: 3, colQoQ: 6, colQoQoY: 5, colUnit: -1, "Hang Khong");
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
            var resTrongNuoc = InsertThongKeOnlyRecord(EKeyTongCucThongKe.VanTai_TrongNuoc, dt, sheet, colContent: 1, colVal: 2, colQoQ: 5, colQoQoY: 4, colUnit: -1, "Trong Nuoc");
            if (!resTrongNuoc)
            {
                InsertThongKeOnlyRecord(EKeyTongCucThongKe.VanTai_TrongNuoc, dt, sheet, colContent: 2, colVal: 3, colQoQ: 6, colQoQoY: 5, colUnit: -1, "Trong Nuoc");
            }

            var resNgoaiNuoc = InsertThongKeOnlyRecord(EKeyTongCucThongKe.VanTai_NuocNgoai, dt, sheet, colContent: 1, colVal: 2, colQoQ: 5, colQoQoY: 4, colUnit: -1, "Ngoai Nuoc");
            if (!resNgoaiNuoc)
            {
                InsertThongKeOnlyRecord(EKeyTongCucThongKe.VanTai_NuocNgoai, dt, sheet, colContent: 2, colVal: 3, colQoQ: 6, colQoQoY: 5, colUnit: -1, "Ngoai Nuoc");
            }

            var resDuongBien = InsertThongKeOnlyRecord(EKeyTongCucThongKe.VanTai_DuongBien, dt, sheet, colContent: 1, colVal: 2, colQoQ: 5, colQoQoY: 4, colUnit: -1, "Duong Bien");
            if (!resDuongBien)
            {
                InsertThongKeOnlyRecord(EKeyTongCucThongKe.VanTai_DuongBien, dt, sheet, colContent: 2, colVal: 3, colQoQ: 6, colQoQoY: 5, colUnit: -1, "Duong Bien");
            }

            var resDuongBo = InsertThongKeOnlyRecord(EKeyTongCucThongKe.VanTai_DuongBo, dt, sheet, colContent: 1, colVal: 2, colQoQ: 5, colQoQoY: 4, colUnit: -1, "Duong Bo");
            if (!resDuongBo)
            {
                InsertThongKeOnlyRecord(EKeyTongCucThongKe.VanTai_DuongBo, dt, sheet, colContent: 2, colVal: 3, colQoQ: 6, colQoQoY: 5, colUnit: -1, "Duong Bo");
            }

            var resHangKhong = InsertThongKeOnlyRecord(EKeyTongCucThongKe.VanTai_HangKhong, dt, sheet, colContent: 1, colVal: 2, colQoQ: 5, colQoQoY: 4, colUnit: -1, "Hang Khong");
            if (!resHangKhong)
            {
                InsertThongKeOnlyRecord(EKeyTongCucThongKe.VanTai_HangKhong, dt, sheet, colContent: 2, colVal: 3, colQoQ: 6, colQoQoY: 5, colUnit: -1, "Hang Khong");
            }
        }

        private bool InsertThongKeOnlyRecord(EKeyTongCucThongKe eThongKe, DateTime dt, ExcelWorksheet sheet, int colContent, int colVal, int colQoQ, int colQoQoY, int colUnit, string textCompare, string textIgnore = "")
        {
            try
            {
                if (colContent <= 0)
                    return false;

                var unitStr = string.Empty;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    var valContent = sheet.Cells[i, colContent].Value?.ToString().Trim() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(valContent))
                        continue;

                    if (!valContent.RemoveSpace().RemoveSignVietnamese().ToUpper().Replace(",","").Contains(textCompare.RemoveSpace().RemoveSignVietnamese().ToUpper()))
                        continue;

                    if (!string.IsNullOrWhiteSpace(textIgnore) && valContent.RemoveSpace().RemoveSignVietnamese().ToUpper().Replace(",", "").Contains(textIgnore.RemoveSpace().RemoveSignVietnamese().ToUpper()))
                        continue;

                    var model = new ThongKe
                    {
                        d = int.Parse($"{dt.Year}{dt.Month.To2Digit()}"),
                        key = (int)eThongKe
                    };
                    model.content = valContent;

                    if (colVal > 0)
                    {
                        var valStr = sheet.Cells[i, colVal].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        model.va = isDouble ? Math.Round(val, 1) : 0;
                    }

                    if (colQoQ > 0)
                    {
                        var valStr = sheet.Cells[i, colQoQ].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        model.qoq = isDouble ? Math.Round(val, 1) : 0;
                    }

                    if (colQoQoY > 0)
                    {
                        var valStr = sheet.Cells[i, colQoQoY].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        model.qoqoy = isDouble ? Math.Round(val, 1) : 0;
                    }

                    if (colUnit > 0)
                    {
                        var valStr = sheet.Cells[i, colUnit].Value?.ToString().Trim() ?? string.Empty;//
                        if (string.IsNullOrWhiteSpace(valStr.Replace("'", "").Replace("\"", "")))
                        {
                            valStr = unitStr;
                        }
                        model.unit = valStr;
                        unitStr = valStr;
                    }

                    if (model.va <= 0 && model.qoq <= 0 && model.qoqoy <= 0)
                        continue;

                    _thongkeRepo.InsertOne(model);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.InsertThongKeOnlyRecord|EXCEPTION| {ex.Message}");
            }
            return false;
        }

        private bool InsertThongKeSomeRecord(EKeyTongCucThongKe eThongKe, DateTime dt, ExcelWorksheet sheet, int colContent, int colVal, int colQoQ, int colQoQoY, int colUnit, string keyStart, int colKeyStart, string keyEnd, int colKeyEnd)
        {
            try
            {
                var unitStr = string.Empty;
                var isStart = false;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    if(!isStart)
                    {
                        var valKeyStart = sheet.Cells[i, colKeyStart].Value?.ToString().Trim() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(valKeyStart))
                            continue;

                        if (valKeyStart.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(keyStart.RemoveSpace().ToUpper()))
                        {
                            isStart = true;
                        }
                        continue;
                    }

                    if(!string.IsNullOrWhiteSpace(keyEnd))
                    {
                        var valKeyEnd = sheet.Cells[i, colKeyEnd].Value?.ToString().Trim() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(valKeyEnd))
                        {
                            if (valKeyEnd.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(keyEnd.RemoveSpace().ToUpper()))
                            {
                                return true;
                            }
                        }
                    }

                    var model = new ThongKe
                    {
                        d = int.Parse($"{dt.Year}{dt.Month.To2Digit()}"),
                        key = (int)eThongKe
                    };

                    if (colContent > 0)
                    {
                        var valStr = sheet.Cells[i, colContent].Value?.ToString().Trim() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(valStr))
                            continue;

                        model.content = valStr;
                    }

                    if (colVal > 0)
                    {
                        var valStr = sheet.Cells[i, colVal].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        model.va = isDouble ? Math.Round(val, 1) : 0;
                    }

                    if (colQoQ > 0)
                    {
                        var valStr = sheet.Cells[i, colQoQ].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        model.qoq = isDouble ? Math.Round(val, 1) : 0;
                    }

                    if (colQoQoY > 0)
                    {
                        var valStr = sheet.Cells[i, colQoQoY].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        model.qoqoy = isDouble ? Math.Round(val, 1) : 0;
                    }

                    if (colUnit > 0)
                    {
                        var valStr = sheet.Cells[i, colUnit].Value?.ToString().Trim() ?? string.Empty;//
                        if (string.IsNullOrWhiteSpace(valStr.Replace("'", "").Replace("\"", "")))
                        {
                            valStr = unitStr;
                        }
                        model.unit = valStr;
                        unitStr = valStr;
                    }

                    if (model.va <= 0 && model.qoq <= 0 && model.qoqoy <= 0)
                        continue;

                    _thongkeRepo.InsertOne(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.InsertThongKeSomeRecord|EXCEPTION| {ex.Message}");
            }
            return true;
        }
    }
}
