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
                    else if (_lIIP.Any(x => sheet.Name.RemoveSignVietnamese().ToUpper().Replace(" ","").EndsWith(x.ToUpper().Replace(" ", ""))))
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucThongKe|EXCEPTION| {ex.Message}");
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
                            if(isSatThep)
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
                            break;
                        }

                        if (col < 0)
                        {
                            continue;
                        }

                        if (!isChiSoGiaTieuDung)
                        {
                            isChiSoGiaTieuDung = InsertThongKe(EKeyTongCucThongKe.CPI_GiaTieuDung, "Chi So Gia Tieu Dung", cellValueCur, i, col, dt, sheet);
                        }

                        if (!isGiaVang)
                        {
                            isGiaVang = InsertThongKe(EKeyTongCucThongKe.CPI_GiaVang, "Gia Vang", cellValueCur, i, col, dt, sheet);
                        }

                        if (!isUSD)
                        {
                            isUSD = InsertThongKe(EKeyTongCucThongKe.CPI_DoLa, "Do La", cellValueCur, i, col, dt, sheet);
                        }

                        if (!isLamPhat)
                        {
                            isLamPhat = InsertThongKe(EKeyTongCucThongKe.CPI_LamPhat, "Lam Phat", cellValueCur, i, col, dt, sheet);
                            if(isLamPhat)
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

        private bool InsertThongKe(EKeyTongCucThongKe eThongKe, string textCompare, string text, int i, int col, DateTime dt, ExcelWorksheet sheet, string textCompare2 = null, int offset = 0)
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
                _thongkeRepo.InsertOne(new ThongKe
                {
                    d = int.Parse($"{dt.Year}{dt.Month.To2Digit()}"),
                    key = (int)eThongKe,
                    va = isDouble ? Math.Round(val, 1) : 0
                });
            }
            return true;
        }
    }
}
