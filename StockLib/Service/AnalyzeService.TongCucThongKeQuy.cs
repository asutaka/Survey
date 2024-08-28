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
        public async Task<(int, string)> TongCucThongKeQuy(DateTime dt)
        {
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                if (dt.Month % 3 != 0)
                    return (0, null);

                //var builder = Builders<ConfigData>.Filter;
                //FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)EConfigDataType.TongCucThongKeQuy);
                //var lConfig = _configRepo.GetByFilter(filter);
                //if (lConfig.Any())
                //{
                //    if (lConfig.Any(x => x.t == t))
                //        return (0, null);

                //    _configRepo.DeleteMany(filter);
                //}

                //var strOutput = new StringBuilder();
                //var stream = await _apiService.TongCucThongKe(dt);
                //if (stream is null
                //    || stream.Length < 1000)
                //    return (0, null);

                //var dic = new Dictionary<int, string>();
                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //var package = new ExcelPackage(stream);
                //var lSheet = package.Workbook.Worksheets;
                //foreach (var sheet in lSheet)
                //{
                //    if (false) { }
                //    else if (_lGiaVT.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        GiaVanTai(sheet, dt);
                //    }
                //    else if (_lGiaNVL.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        GiaNguyenVatLieu(sheet, dt);
                //    }
                //    else if (_lGiaXK.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        GiaXuatKhau(sheet, dt);
                //    }
                //    else if (_lGiaNK.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        GiaNhapKhau(sheet, dt);
                //    }
                //    else if (_lGDP_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        GDP_Quy(sheet, dt);
                //    }
                //    else if (_lChanNuoi_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        ChanNuoi_Quy(sheet, dt);
                //    }
                //    else if (_lThuySan_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        ThuySan_Quy(sheet, dt);
                //    }
                //    else if (_lIIP_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        IIP_Quy(sheet, dt);
                //    }
                //    else if (_lSPCongNghiep_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        SanPhamCongNghiep_Quy(sheet, dt);
                //    }
                //    else if (_lVonDauTu_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        VonDauTuNhaNuoc_Quy(sheet, dt);
                //    }
                //    else if (_lBanLe_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        BanLe_Quy(sheet, dt);
                //    }
                //    else if (_lXK_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        XuatKhau_Quy(sheet, dt);
                //    }
                //    else if (_lNK_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        NhapKhau_Quy(sheet, dt);
                //    }
                //    else if (_lVanTaiHangHoa_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        VanTaiHangHoa_Quy(sheet, dt);
                //    }
                //    else if (_lKhachQuocTe_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                //    {
                //        KhachQuocTe_Quy(sheet, dt);
                //    }
                //}
                var mes = TongCucThongKeQuyPrint(dt);
                return (1, mes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucThongKeQuy|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        public async Task<(int, string)> TongCucThongKeQuyTest(DateTime dt)
        {
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                if (dt.Month % 3 != 0)
                    return (0, null);

                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)EConfigDataType.TongCucThongKeQuy);
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
                    else if (_lGiaVT.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        GiaVanTai(sheet, dt);
                    }
                    else if (_lGiaNVL.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        GiaNguyenVatLieu(sheet, dt);
                    }
                    else if (_lGiaXK.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        GiaXuatKhau(sheet, dt);
                    }
                    else if (_lGiaNK.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        GiaNhapKhau(sheet, dt);
                    }
                    else if (_lGDP_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        GDP_Quy(sheet, dt);
                    }
                    else if (_lChanNuoi_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        ChanNuoi_Quy(sheet, dt);
                    }
                    else if (_lThuySan_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        ThuySan_Quy(sheet, dt);
                    }
                    else if (_lIIP_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        IIP_Quy(sheet, dt);
                    }
                    else if (_lSPCongNghiep_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        SanPhamCongNghiep_Quy(sheet, dt);
                    }
                    else if (_lVonDauTu_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        VonDauTuNhaNuoc_Quy(sheet, dt);
                    }
                    else if (_lBanLe_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        BanLe_Quy(sheet, dt);
                    }
                    else if (_lXK_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        XuatKhau_Quy(sheet, dt);
                    }
                    else if (_lNK_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        NhapKhau_Quy(sheet, dt);
                    }
                    else if (_lVanTaiHangHoa_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        VanTaiHangHoa_Quy(sheet, dt);
                    }
                    else if (_lKhachQuocTe_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        KhachQuocTe_Quy(sheet, dt);
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

        private List<string> _lGiaVT = new List<string>
        {
            "Gia Van Tai",
            "Gia VT"
        };
        private void GiaVanTai(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaVT_DuongSat, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Duong Sat", textIgnore: "Duong Bo");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaVT_DuongBo, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Duong Bo", textIgnore: "Duong Sat");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaVT_Bien, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Bien");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaVT_DuongThuy, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Thuy Noi Dia");
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

        private List<string> _lGiaXK = new List<string>
        {
            "Gia Xuat Khau",
            "Gia XK"
        };
        private void GiaXuatKhau(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaXK_ThuySan, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Thuy San");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaXK_Gao, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Gao");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaXK_CaoSu, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Cao su");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaXK_PhanBon, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Phan Bon");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaXK_SPChatDeo, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Chat Deo");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaXK_Go, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Go");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaXK_DetMay, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Det May");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaXK_SatThep, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Sat Thep");
        }

        private List<string> _lGiaNK = new List<string>
        {
            "Gia Nhap Khau",
            "Gia NK"
        };
        private void GiaNhapKhau(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaNK_SatThep, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "Sat Thep");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GiaNK_Oto, dt, sheet, colContent: 1, colVal: -1, colQoQ: 2, colQoQoY: 3, colUnit: -1, "O to");
        }

        private List<string> _lGDP_Quy = new List<string>
        {
            "GDP HH"
        };
        private void GDP_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GDP_NongNghiep, dt, sheet, colContent: 2, colVal: 4, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Nong Nghiep");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GDP_LamNghiep, dt, sheet, colContent: 2, colVal: 4, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Lam Nghiep", textIgnore: "Thuy San");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GDP_ThuySan, dt, sheet, colContent: 2, colVal: 4, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Thuy San", textIgnore: "Lam Nghiep");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GDP_XayDung, dt, sheet, colContent: 2, colVal: 4, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Xay Dung");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GDP_Oto, dt, sheet, colContent: 2, colVal: 4, colQoQ: 7, colQoQoY: -1, colUnit: -1, "O to");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GDP_VanTaiKhoBai, dt, sheet, colContent: 2, colVal: 4, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Van Tai");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GDP_NganHangBaoHiem, dt, sheet, colContent: 2, colVal: 4, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Ngan Hang");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.GDP_BatDongSan, dt, sheet, colContent: 2, colVal: 4, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Bat Dong San");
        }

        private List<string> _lChanNuoi_Quy = new List<string>
        {
            "Chan Nuoi"
        };

        private void ChanNuoi_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.ChanNuoi_Lon, dt, sheet, colContent: 1, colVal: 3, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Lon");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.ChanNuoi_Sua, dt, sheet, colContent: 1, colVal: 3, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Sua");
        }

        private List<string> _lThuySan_Quy = new List<string>
        {
            "Thuy San"
        };

        private void ThuySan_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.ThuySan_Ca, dt, sheet, colContent: 1, colVal: 3, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Ca");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.ThuySan_Tom, dt, sheet, colContent: 1, colVal: 3, colQoQ: 7, colQoQoY: -1, colUnit: -1, "Tom");
        }

        private List<string> _lIIP_Quy = new List<string>
        {
            "IIPQuy"
        };
        private void IIP_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKe_Quy(EKeyTongCucThongKe.IIP, dt, sheet, colContent: 1, colVal: -1, colQoQ: 3, colQoQoY: -1, colUnit: -1);
        }

        private List<string> _lSPCongNghiep_Quy = new List<string>
        {
            "SPCNQuy"
        };
        private void SanPhamCongNghiep_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKe_Quy(EKeyTongCucThongKe.SP_CongNghiep, dt, sheet, colContent: 1, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: 2);
        }

        private List<string> _lVonDauTu_Quy = new List<string>
        {
            "VDT Quy",
            "Von Dau Tu Quy",
            "NSNN Quy",
            "NSNN Thang Quy",
            "Von DT Quy"
        };
        private void VonDauTuNhaNuoc_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.DauTuCong, dt, sheet, colContent: 1, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Tong So");
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

        private List<string> _lXK_Quy = new List<string>
        {
            "XK Quy",
            "XK HH Quy",
            "Xuat Khau Quy"
        };
        private void XuatKhau_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_TrongNuoc, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Trong Nuoc");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_FDI, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "NN");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_ThuySan, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Thuy San");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_Gao, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Gao");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_Ximang, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Xi Mang");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_HoaChat, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Hoa Chat");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_SPHoaChat, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "San Pham Hoa Chat");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_SPChatDeo, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "San Pham Tu Chat Deo");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_CaoSu, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Cao Su");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_Go, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Go");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_DetMay, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Det May");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_SatThep, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Sat Thep");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_SPSatThep, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "San Pham Tu Sat Thep");
        }

        private List<string> _lNK_Quy = new List<string>
        {
            "NK Quy",
            "NK HH Quy",
            "Nhap Khau Quy"
        };
        private void NhapKhau_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.NK_TrongNuoc, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Trong Nuoc");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.NK_FDI, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "NN");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.NK_PhanBon, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Phan Bon");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.NK_Vai, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Vai");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.NK_SatThep, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "Sat Thep", textIgnore: "Phe Lieu");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.NK_SPSatThep, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "San Pham Tu Sat Thep");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.NK_Oto, dt, sheet, colContent: 2, colVal: 7, colQoQ: 13, colQoQoY: -1, colUnit: -1, "O to");
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

            var resDuongSat = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_DuongSat, dt, sheet, colContent: 1, colVal: 3, colQoQ: 5, colQoQoY: -1, colUnit: -1, "Duong Sat");
            if (!resDuongSat)
            {
                InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_DuongSat, dt, sheet, colContent: 2, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Duong Sat");
            }

            var resDuongBien = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_DuongBien, dt, sheet, colContent: 1, colVal: 3, colQoQ: 5, colQoQoY: -1, colUnit: -1, "Duong Bien");
            if (!resDuongBien)
            {
                InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_DuongBien, dt, sheet, colContent: 2, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Duong Bien");
            }

            var resDuongThuy = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_DuongThuy, dt, sheet, colContent: 1, colVal: 3, colQoQ: 5, colQoQoY: -1, colUnit: -1, "Duong Thuy");
            if (!resDuongThuy)
            {
                InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_DuongThuy, dt, sheet, colContent: 2, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Duong Thuy");
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

        private List<string> _lKhachQuocTe_Quy = new List<string>
        {
            "KQT Quy",
            "Du Lich Quy",
            "Khach QT Quy",
            "Khach Quoc Te Quy"
        };
        private void KhachQuocTe_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.DuLich, dt, sheet, colContent: 1, colVal: 4, colQoQ: 6, colQoQoY: -1, colUnit: -1, "Tong So");
        }

        private bool InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe eThongKe, DateTime dt, ExcelWorksheet sheet, int colContent, int colVal, int colQoQ, int colQoQoY, int colUnit, string textCompare, string textIgnore = "")
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

                    if (!valContent.RemoveSpace().RemoveSignVietnamese().ToUpper().Replace(",", "").Contains(textCompare.RemoveSpace().RemoveSignVietnamese().ToUpper()))
                        continue;

                    if (!string.IsNullOrWhiteSpace(textIgnore) && valContent.RemoveSpace().RemoveSignVietnamese().ToUpper().Replace(",", "").Contains(textIgnore.RemoveSpace().RemoveSignVietnamese().ToUpper()))
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
