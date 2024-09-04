﻿using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OfficeOpenXml;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        //28 -> 5
        public async Task<(int, string)> TongCucThongKeQuy(DateTime dt)
        {
            if((dt.Day <= 5 && dt.Month % 3 == 0)
                || (dt.Day >= 28 && dt.Month % 3 == 1))
            {
                return (0, null);
            }

            if (dt.Day <= 5 && dt.Month % 3 == 1)
            {
                dt = dt.AddMonths(-1);
            }
            var t = long.Parse($"{dt.Year}{dt.GetQuarter()}"); 
                
            try
            {
                var mode = EConfigDataType.TongCucThongKeQuy;
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)mode);
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
                bool isVTHK = false, isVTHH = false, isDauTuCong = false, isBanLe = false, isDuLich = false, isSPCN = false, isIIP = false;
                foreach (var sheet in lSheet)
                {
                    if (false) { }
                    else if (_lGiaVT.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        GiaVanTai(sheet, dt);
                    }
                    else if (_lGiaNVL.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        GiaNguyenVatLieu(sheet, dt);
                    }
                    else if (_lIIP_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isIIP = true;
                        IIP_Quy(sheet, dt);
                    }
                    else if (_lVonDauTu_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isDauTuCong = true;
                        VonDauTuNhaNuoc_Quy(sheet, dt);
                    }
                    else if (_lBanLe_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isBanLe = true;
                        BanLe_Quy(sheet, dt);
                    }
                    else if (_lVanTaiHanhKhach_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isVTHK = true;
                        VanTaiHanhKhach_Quy(sheet, dt);
                    }
                    else if (_lVanTaiHangHoa_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isVTHH = true;
                        VanTaiHangHoa_Quy(sheet, dt);
                    }
                }
                foreach (var sheet in lSheet)
                {
                    if (false) { }
                    else if (!isVTHK && _lVanTaiHanhKhach_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        VanTaiHanhKhach_Quy(sheet, dt);
                    }
                    else if (!isVTHH && _lVanTaiHangHoa.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        VanTaiHangHoa_Quy(sheet, dt);
                    }
                    else if (!isDauTuCong && _lVonDauTu.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        VonDauTuNhaNuoc_Quy(sheet, dt);
                    }
                    else if (!isBanLe && _lBanLe.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        BanLe_Quy_Custom(sheet, dt);
                    }
                    else if (!isIIP && _lIIP.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        IIP_Quy_Custom(sheet, dt);
                    }
                }

                var mes = TongCucThongKeQuyPrint(dt);
                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)mode,
                    t = t
                });
                return (1, mes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucThongKeQuy|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        private List<string> _lGiaVT = new List<string>
        {
            "Gia Van Tai",
            "Gia VT"
        };
        private void GiaVanTai(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaVT_Bien, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Bien");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaVT_HangKhong, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Hang Khong");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaVT_KhoBai, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Kho Bai");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaVT_BuuChinh, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Buu Chinh");
        }

        private List<string> _lGiaNVL = new List<string>
        {
            "Gia Nguyen Vat Lieu",
            "Gia NVL"
        };
        private void GiaNguyenVatLieu(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaNVL_Dien, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Dien");
        }

        private List<string> _lIIP_Quy = new List<string>
        {
            "IIPQuy"
        };
        private void IIP_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.IIP_Dien, dt, sheet, colContent: 1, colVal: -1, colQoQ: 4, colQoQoY: 5, colUnit: -1, textCompare: "Phan Phoi Dien");
        }

        private void IIP_Quy_Custom(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.IIP_Dien, dt, sheet, colContent: 1, colVal: -1, colQoQ: 5, colQoQoY: -1, colUnit: -1, textCompare: "Phan Phoi Dien");
        }

        private List<string> _lVonDauTu_Quy = new List<string>
        {
            "VDT Quy",
            "Von Dau Tu Quy",
            "NSNN Quy",
            "Von DT Quy",
            "Thuc Hien Quy"
        };
        private void VonDauTuNhaNuoc_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyDTC_Quy(EKeyTongCucThongKe.DauTuCong, dt, sheet, colContent: 1, "Tong So");
        }
        private List<string> _lBanLe_Quy = new List<string>
        {
            "Tongmuc Quy",
            "TM_Quy",
            "TM Quy"
        };

        private void BanLe_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            var res = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.BanLe, dt, sheet, colContent: 1, colVal: 3, colQoQ: 5, colQoQoY: -1, colUnit: -1, "Ban Le");
            if (!res)
            {
                InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.BanLe, dt, sheet, colContent: 2, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Ban Le");
            }
        }

        private void BanLe_Quy_Custom(ExcelWorksheet sheet, DateTime dt)
        {
            var res = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.BanLe, dt, sheet, colContent: 1, colVal: 4, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Ban Le");
        }

        private List<string> _lVanTaiHanhKhach_Quy = new List<string>
        {
            "VT HK Quy",
            "Hanh Khach Quy",
            "VanTai HK Quy",
            "Van Tai HK Quy"
        };
        private void VanTaiHanhKhach_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            var resHangKhong = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.HanhKhach_HangKhong, dt, sheet, colContent: 1, colVal: 3, colQoQ: 5, colQoQoY: -1, colUnit: -1, "Hang Khong");
            if (!resHangKhong)
            {
                InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.HanhKhach_HangKhong, dt, sheet, colContent: 2, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Hang Khong");
            }
        }

        private List<string> _lVanTaiHangHoa_Quy = new List<string>
        {
            "VT HH Quy",
            "Hang Hoa Quy",
            "VanTai HH Quy",
            "Van Tai HH Quy"
        };
        private void VanTaiHangHoa_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            var resTrongNuoc = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_TrongNuoc, dt, sheet, colContent: 1, colVal: 3, colQoQ: 5, colQoQoY: -1, colUnit: -1, "Trong Nuoc");
            if (!resTrongNuoc)
            {
                InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_TrongNuoc, dt, sheet, colContent: 2, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Trong Nuoc");
            }

            var resNgoaiNuoc = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_NuocNgoai, dt, sheet, colContent: 1, colVal: 3, colQoQ: 5, colQoQoY: -1, colUnit: -1, "Ngoai Nuoc");
            if (!resNgoaiNuoc)
            {
                InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_NuocNgoai, dt, sheet, colContent: 2, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Ngoai Nuoc");
            }

            var resDuongBien = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_DuongBien, dt, sheet, colContent: 1, colVal: 3, colQoQ: 5, colQoQoY: -1, colUnit: -1, "Duong Bien");
            if (!resDuongBien)
            {
                InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_DuongBien, dt, sheet, colContent: 2, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Duong Bien");
            }

            var resDuongBo = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_DuongBo, dt, sheet, colContent: 1, colVal: 3, colQoQ: 5, colQoQoY: -1, colUnit: -1, "Duong Bo");
            if (!resDuongBo)
            {
                InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_DuongBo, dt, sheet, colContent: 2, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Duong Bo");
            }

            var resHangKhong = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_HangKhong, dt, sheet, colContent: 1, colVal: 3, colQoQ: 5, colQoQoY: -1, colUnit: -1, "Hang Khong");
            if (!resHangKhong)
            {
                InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_HangKhong, dt, sheet, colContent: 2, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Hang Khong");
            }
        }

        private bool InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe eThongKe, DateTime dt, ExcelWorksheet sheet, int colContent, int colVal, int colQoQ, int colQoQoY, int colUnit, string textCompare, string textIgnore = "", int colQoQTry = -1)
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

                    if (!valContent.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(textCompare.RemoveSpace().RemoveSignVietnamese().ToUpper()))
                        continue;

                    if (!string.IsNullOrWhiteSpace(textIgnore) && valContent.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(textIgnore.RemoveSpace().RemoveSignVietnamese().ToUpper()))
                        continue;

                    var model = new ThongKeQuy
                    {
                        d = int.Parse($"{dt.Year}{dt.GetQuarter()}"),
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

                        if(model.qoq == 0 && colQoQTry > 0)
                        {
                            valStr = sheet.Cells[i, colQoQTry].Value?.ToString().Trim() ?? string.Empty;
                            isDouble = double.TryParse(valStr.Replace(",", ""), out var valTry);
                            model.qoq = isDouble ? Math.Round(valTry, 1) : 0;
                        }
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

                    _thongkeQuyRepo.InsertOne(model);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.InsertThongKeOnlyRecord_Quy|EXCEPTION| {ex.Message}");
            }
            return false;
        }

        private bool InsertThongKeOnlyDTC_Quy(EKeyTongCucThongKe eThongKe, DateTime dt, ExcelWorksheet sheet, int colContent, string textCompare, string textIgnore = "")
        {
            try
            {
                if (colContent <= 0)
                    return false;

                var unitStr = string.Empty;
                var quarterStr = dt.GetQuarterStr();
                var colVal = -1;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    
                    for (int j = colContent + 1; j < sheet.Dimension.Columns; j++)
                    {
                        var valCheck = (sheet.Cells[i, j].Value?.ToString().Trim() ?? string.Empty).RemoveSpace().RemoveSignVietnamese().ToUpper();
                        if (string.IsNullOrWhiteSpace(valCheck))
                            continue;

                        if(valCheck.Contains("Uoc Tinh".RemoveSpace().RemoveSignVietnamese().ToUpper()))
                        {
                            if(valCheck.Contains($"Quy {quarterStr}".RemoveSpace().RemoveSignVietnamese().ToUpper())
                                || valCheck.Contains($"QUÝ{quarterStr}"))
                            {
                                colVal = j;
                                break;
                            }
                            else
                            {
                                var valCheckNext = (sheet.Cells[i + 1, j].Value?.ToString().Trim() ?? string.Empty).RemoveSpace().RemoveSignVietnamese().ToUpper();
                                if (valCheckNext.Contains($"Quy {quarterStr}".RemoveSpace().RemoveSignVietnamese().ToUpper())
                                    || valCheckNext.Contains($"QUÝ{quarterStr}"))
                                {
                                    colVal = j;
                                    break;
                                }
                            }
                        }
                    }

                    if (colVal <= 0)
                        continue;


                    var valContent = sheet.Cells[i, colContent].Value?.ToString().Trim() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(valContent))
                        continue;

                    if (!valContent.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(textCompare.RemoveSpace().RemoveSignVietnamese().ToUpper()))
                        continue;

                    if (!string.IsNullOrWhiteSpace(textIgnore) && valContent.RemoveSpace().RemoveSignVietnamese().ToUpper().Contains(textIgnore.RemoveSpace().RemoveSignVietnamese().ToUpper()))
                        continue;

                    var model = new ThongKeQuy
                    {
                        d = int.Parse($"{dt.Year}{dt.GetQuarter()}"),
                        key = (int)eThongKe
                    };
                    model.content = valContent;

                    if (colVal > 0)
                    {
                        var valStr = sheet.Cells[i, colVal].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        model.va = isDouble ? Math.Round(val, 1) : 0;
                    }

                    if (model.va <= 0)
                        continue;

                    _thongkeQuyRepo.InsertOne(model);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.InsertThongKeOnlyDTC_Quy|EXCEPTION| {ex.Message}");
            }
            return false;
        }

        private bool InsertThongKe_Quy(EKeyTongCucThongKe eThongKe, DateTime dt, ExcelWorksheet sheet, int colContent, int colVal, int colQoQ, int colQoQoY, int colUnit)
        {
            try
            {
                var unitStr = string.Empty;
                //loop all rows in the sheet
                for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                {
                    var model = new ThongKeQuy
                    {
                        d = int.Parse($"{dt.Year}{dt.GetQuarter()}"),
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

                    _thongkeQuyRepo.InsertOne(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.InsertThongKe_Quy|EXCEPTION| {ex.Message}");
            }
            return true;
        }
    }
}
