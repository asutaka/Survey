using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OfficeOpenXml;
using StockLib.DAL.Entity;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        private async Task<bool> TongCucThongKeQuyHandle(string url)
        {
            try
            {
                var mode = EConfigDataType.TongCucThongKeQuy;
                var dtNow = DateTime.Now;
                var year = dtNow.Year;
                var urlCheck = url.Replace("-", ".");
                var index = urlCheck.IndexOf($".{year}");
                if (index == -1)
                {
                    year = dtNow.Year - 1;
                    index = url.IndexOf($".{year}");
                }

                if (index == -1)
                    return false;
                var isInt = int.TryParse(urlCheck.Substring(index - 2, 2).Replace("T", ""), out var month);
                if (!isInt)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        if (urlCheck.IndexOf($"{i.To2Digit()}/") > -1)
                        {
                            month = i;
                            break;
                        }
                    }
                }
                var dt = new DateTime(year, month, 28);

                if (dt.Month % 3 != 0)
                    return false;

                var stream = await _apiService.StreamTongCucThongKe(url);
                if (stream is null
                   || stream.Length < 1000)
                {
                    return false;
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage(stream);
                var lSheet = package.Workbook.Worksheets;
                bool isIIP = false, isVonDauTu = false, isBanLe = false, isVantaiHK = false, isVanTaiHH = false, isSPCN = false, isXK = false, isNK = false, isGiaVT = false, isGiaNVL = false, isGiaXK = false, isGDP = false;
                foreach (var sheet in lSheet)
                {
                    if (false) { }
                    else if (!isIIP && _lIIP_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isIIP = true;
                        IIP_Quy(sheet, dt);
                    }
                    else if (!isSPCN && _lSPCN_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isSPCN = true;
                        SPCN_Quy(sheet, dt);
                    }
                    else if (!isVonDauTu && _lVonDauTu_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isVonDauTu = true;
                        VonDauTuNhaNuoc_Quy(sheet, dt);
                    }
                    else if (!isBanLe && _lBanLe_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isBanLe = true;
                        BanLe_Quy(sheet, dt);
                    }
                    else if (!isXK && _lXK_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isXK = true;
                        XuatKhau_Quy(sheet, dt);
                    }
                    else if (!isNK && _lNK_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isNK = true;
                        NhapKhau_Quy(sheet, dt);
                    }
                    else if (!isVantaiHK && _lVanTaiHanhKhach_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isVantaiHK = true;
                        VanTaiHanhKhach_Quy(sheet, dt);
                    }
                    else if (!isVanTaiHH && _lVanTaiHangHoa_Quy.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isVanTaiHH = true;
                        VanTaiHangHoa_Quy(sheet, dt);
                    }
                    else if (!isGiaVT && _lGiaVT.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isGiaVT = true;
                        GiaVanTai(sheet, dt);
                    }
                    else if (!isGiaNVL && _lGiaNVL.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isGiaNVL = true;
                        GiaNguyenVatLieu(sheet, dt);
                    }
                    else if (!isGiaXK && _lGiaXK.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isGiaXK = true;
                        GiaXuatKhau(sheet, dt);
                    }
                    else if (!isGDP && _lGDP.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isGDP = true;
                        GDP(sheet, dt);
                    }
                }
                foreach (var sheet in lSheet)
                {
                    if (false) { }
                    else if (!isIIP && _lIIP.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isIIP = true;
                        IIP_Quy(sheet, dt);
                    }
                    else if (!isSPCN && _lSPCN.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isSPCN = true;
                        SPCN_Quy(sheet, dt);
                    }
                    else if (!isVonDauTu && _lVonDauTu.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isVonDauTu = true;
                        VonDauTuNhaNuoc_Quy(sheet, dt);
                    }
                    else if (!isBanLe && _lBanLe.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isBanLe = true;
                        BanLe_Quy(sheet, dt);
                    }
                    else if (!isXK && _lXK.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isXK = true;
                        XuatKhau_Quy(sheet, dt);
                    }
                    else if (!isNK && _lNK.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isNK = true;
                        NhapKhau_Quy(sheet, dt);
                    }
                    else if (!isVantaiHK && _lVanTaiHanhKhach.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isVantaiHK = true;
                        VanTaiHanhKhach_Quy(sheet, dt);
                    }
                    else if (!isVanTaiHH && _lVanTaiHangHoa.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isVanTaiHH = true;
                        VanTaiHangHoa_Quy(sheet, dt);
                    }
                    else if (!isGiaVT && _lGiaVT.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isGiaVT = true;
                        GiaVanTai(sheet, dt);
                    }
                    else if (!isGiaNVL && _lGiaNVL.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isGiaNVL = true;
                        GiaNguyenVatLieu(sheet, dt);
                    }
                    else if (!isGiaXK && _lGiaXK.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isGiaXK = true;
                        GiaXuatKhau(sheet, dt);
                    }
                    else if (!isGDP && _lGDP.Any(x => sheet.Name.RemoveSpace().RemoveSignVietnamese().ToUpper().EndsWith(x.RemoveSpace().ToUpper())))
                    {
                        isGDP = true;
                        GDP(sheet, dt);
                    }
                }

                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)mode);
                var t = long.Parse($"{dt.Year}{dt.GetQuarter()}");
                var lConfig = _configRepo.GetByFilter(filter);
                var last = lConfig.LastOrDefault();
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
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucThongKeHandle|EXCEPTION| {ex.Message}");
            }
            return false;
        }
        public async Task TongCucThongKeQuyHis()
        {
            try
            {

                var lUrl = await _apiService.DSTongCucThongKe();
                lUrl.Reverse();

                foreach (var item in lUrl)
                {
                    await TongCucThongKeQuyHandle(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucThongKeQuyHis|EXCEPTION| {ex.Message}");
            }
        }

        public async Task<(int, string)> TongCucThongKeQuy(DateTime dtNow)
        {
            try
            {
                var url = await _apiService.TongCucThongKe();
                var urlCheck = url.Replace("-", ".");
                var year = dtNow.Year;
                var index = urlCheck.IndexOf($".{year}");
                if (index == -1)
                {
                    year = dtNow.Year - 1;
                    index = urlCheck.IndexOf($".{year}");
                }
                if (index == -1)
                    return (0, null);

                var isInt = int.TryParse(urlCheck.Substring(index - 2, 2).Replace("T", ""), out var month);
                if (!isInt)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        if (urlCheck.IndexOf($"{i.To2Digit()}/") > -1)
                        {
                            month = i;
                            break;
                        }
                    }
                }
                var t = long.Parse($"{dtNow.Year}{dtNow.GetQuarter()}");
                var dtLocal = new DateTime(year, month, 28);
                if (dtLocal.Month % 3 != 0)
                    return (0, null);

                var mode = EConfigDataType.TongCucThongKeQuy;
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)mode);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);
                }

                var res = await TongCucThongKeQuyHandle(url);
                if(res)
                {
                    var mes = TongCucThongKeQuyPrint(dtLocal);
                    return (1, mes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TongCucThongKeQuy|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        private List<string> _lIIP_Quy = new List<string>
        {
            "IIPQuy"
        };
        private void IIP_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            var cQoQPrev = -1;
            var cQoQ = -1;
            var quarter = dt.GetQuarter();
            if(quarter == 1)
            {
                cQoQ = 5;
            }
            else if(quarter == 2)
            {
                cQoQPrev = 2;
                cQoQ = 3;
            }
            else if(quarter == 3)
            {
                cQoQPrev = 3;
                cQoQ = 4;
            }
            else
            {
                cQoQPrev = 4;
                cQoQ = 5;
            }

            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.IIP_Dien, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: cQoQPrev, textCompare: "Phan Phoi Dien");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.IIP_Than, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: cQoQPrev, textCompare: "Khai Thac Than");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.IIP_DauTho, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: cQoQPrev, textCompare: "Dau Tho");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.IIP_TrangPhuc, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: cQoQPrev, textCompare: "Trang Phuc");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.IIP_CheBienGo, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: cQoQPrev, textCompare: "Che Bien Go");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.IIP_GiuongTuBanGhe, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: cQoQPrev, textCompare: "Giuong");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.IIP_HoaChat, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: cQoQPrev, textCompare: "Hoa Chat");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.IIP_Thuoc, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: cQoQPrev, textCompare: "San Xuat Thuoc");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.IIP_CaoSuNhua, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: cQoQPrev, textCompare: "Cao Su");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.IIP_ThietBiDien, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: cQoQPrev, textCompare: "Thiet Bi Dien");
        }

        private List<string> _lSPCN_Quy = new List<string>
        {
            "SP CN Quy"
        };

        private void SPCN_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            var cQoQPrev = -1;
            var cQoQ = -1;
            var cValPrev = -1;
            var cVal = -1;
            var quarter = dt.GetQuarter();
            if (quarter == 1)
            {
                cVal = 5;
                cQoQ = 7;
            }
            else if (quarter == 2)
            {
                cValPrev = 3;
                cVal = 4;
                cQoQPrev = 5;
                cQoQ = 6;
            }
            else if (quarter == 3)
            {
                cValPrev = 4;
                cVal = 5;
                cQoQPrev = 7;
                cQoQ = 8;
            }
            else
            {
                cValPrev = 3;
                cVal = 4;
                cQoQPrev = 5;
                cQoQ = 6;
            }

            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.SPCN_ThuySan, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, textCompare: "Thuy San");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.SPCN_Duong, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, textCompare: "Duong");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.SPCN_Bia, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, textCompare: "Bia");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.SPCN_Ure, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, textCompare: "Ure");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.SPCN_NPK, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, textCompare: "NPK");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.SPCN_Ximang, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, textCompare: "Xi mang");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.SPCN_ThepCan, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, textCompare: "Thep Can");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.SPCN_ThepThanh, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, textCompare: "Thep Thanh");
        }

        private List<string> _lVonDauTu_Quy = new List<string>
        {
            "VDT Quy",
            "Von Dau Tu Quy",
            "NSNN Quy",
            "Von DT Quy"
        };
        private void VonDauTuNhaNuoc_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            var cQoQPrev = -1;
            var cQoQ = -1;
            var cValPrev = -1;
            var cVal = -1;
            var quarter = dt.GetQuarter();
            if (quarter == 1)
            {
                cVal = 5;
                cQoQ = 7;
            }
            else if (quarter == 2)
            {
                cValPrev = 3;
                cVal = 4;
                cQoQPrev = 5;
                cQoQ = 6;
            }
            else if (quarter == 3)
            {
                cValPrev = 4;
                cVal = 5;
                cQoQPrev = 7;
                cQoQ = 8;
            }
            else
            {
                cValPrev = 3;
                cVal = 4;
                cQoQPrev = 5;
                cQoQ = 6;
            }

            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.DauTuCong, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Tong So");
        }

        private List<string> _lBanLe_Quy = new List<string>
        {
            "Tongmuc Quy",
            "TM Quy",
        };

        private void BanLe_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            var cQoQPrev = -1;
            var cQoQ = -1;
            var cValPrev = -1;
            var cVal = -1;
            var quarter = dt.GetQuarter();
            if (quarter == 1)
            {
                cVal = 4;
                cQoQ = 7;
            }
            else if (quarter == 2)
            {
                cValPrev = 3;
                cVal = 4;
                cQoQPrev = 5;
                cQoQ = 6;
            }
            else if (quarter == 3)
            {
                cValPrev = 4;
                cVal = 5;
                cQoQPrev = 6;
                cQoQ = 7;
            }
            else
            {
                cValPrev = 3;
                cVal = 4;
                cQoQPrev = 5;
                cQoQ = 6;
            }

            var res = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.BanLe, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Ban Le");
        }

        private List<string> _lXK_Quy = new List<string>
        {
            "XK Quy",
            "Xuat Khau Quy"
        };

        private void XuatKhau_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            var cVal = 7;
            var cQoQ = 13;
            var cQoQPrev = -1;
            var cValPrev = -1;
            var quarter = dt.GetQuarter();
            if (quarter == 1)
            {
            }
            else 
            {
                cValPrev = 4;
                cQoQPrev = 10;
            }

            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_ThuySan, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Thuy San");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_CaPhe, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Ca Phe");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_Gao, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Gao");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_Ximang, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Xi mang");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_HoaChat, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Hoa chat", textIgnore: "san pham");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_SPHoaChat, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "San pham hoa chat");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_ChatDeo, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Chat deo", textIgnore: "san pham");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_SPChatDeo, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Tu chat deo");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_CaoSu, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Cao su");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_Go, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Go");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_DetMay, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Det may");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_SatThep, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Sat thep", textIgnore: "san pham");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_SPSatThep, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "San pham tu sat thep");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.XK_DayDien, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Day dien");
        }

        private List<string> _lNK_Quy = new List<string>
        {
            "NK Quy",
            "Nhap Khau Quy"
        };

        private void NhapKhau_Quy(ExcelWorksheet sheet, DateTime dt)
        {
            var cVal = 7;
            var cQoQ = 13;
            var cQoQPrev = -1;
            var cValPrev = -1;
            var quarter = dt.GetQuarter();
            if (quarter == 1)
            {
            }
            else
            {
                cValPrev = 4;
                cQoQPrev = 10;
            }

            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.NK_PhanBon, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Phan bon");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.NK_SatThep, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Sat thep", textIgnore: "san pham");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.NK_SPSatThep, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "San pham tu sat thep");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.NK_Oto, dt, sheet, colContent: 2, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: 3, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "nguyen chiec");
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
            var cQoQPrev = -1;
            var cQoQ = -1;
            var cValPrev = -1;
            var cVal = -1;
            var quarter = dt.GetQuarter();
            if (quarter == 1)
            {
                cVal = 3;
                cQoQ = 6;
            }
            else 
            {
                cValPrev = 3;
                cVal = 4;
                cQoQPrev = 5;
                cQoQ = 6;
            }

            var resHangKhong = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.HanhKhach_HangKhong, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Hang Khong");
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
            var cQoQPrev = -1;
            var cQoQ = -1;
            var cValPrev = -1;
            var cVal = -1;
            var quarter = dt.GetQuarter();
            if (quarter == 1)
            {
                cVal = 3;
                cQoQ = 6;
            }
            else
            {
                cValPrev = 3;
                cVal = 4;
                cQoQPrev = 5;
                cQoQ = 6;
            }

            var resTrongNuoc = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_TrongNuoc, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Trong Nuoc");
            var resNgoaiNuoc = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_NuocNgoai, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Ngoai Nuoc");
            var resDuongBien = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_DuongBien, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Duong Bien");
            var resDuongBo = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_DuongBo, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Duong Bo");
            var resHangKhong = InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.VanTai_HangKhong, dt, sheet, colContent: 1, colVal: cVal, colQoQ: cQoQ, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: cQoQPrev, "Hang Khong");
        }

        private List<string> _lGiaVT = new List<string>
        {
            "Gia Van Tai",
            "Gia VT"
        };
        private void GiaVanTai(ExcelWorksheet sheet, DateTime dt)
        {
            var cQoQ = 2;
            var cQoQoY = 3;
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaVT_Bien, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Bien");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaVT_HangKhong, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Hang Khong");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaVT_KhoBai, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Kho Bai");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaVT_BuuChinh, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Buu Chinh");
        }

        private List<string> _lGiaNVL = new List<string>
        {
            "Gia Nguyen Vat Lieu",
            "Gia NVL"
        };
        private void GiaNguyenVatLieu(ExcelWorksheet sheet, DateTime dt)
        {
            var cQoQ = 2;
            var cQoQoY = 3;
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaNVL_Dien, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Dien");
        }

        private List<string> _lGiaXK = new List<string>
        {
            "Gia XK",
            "Gia Xuat Khau"
        };
        private void GiaXuatKhau(ExcelWorksheet sheet, DateTime dt)
        {
            var cQoQ = 2;
            var cQoQoY = 3;
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_ThuySan, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Thuy San");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_CaPhe, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Ca Phe");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_Gao, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Gao");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_CaoSu, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Cao Su");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_Than, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Than");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_DauTho, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Dau Tho");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_SPHoaChat, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Hoa Chat");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_PhanBon, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Phan Bon");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_SPChatDeo, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Chat Deo");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_Go, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Go");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_DetMay, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Det May");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_SatThep, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Sat Thep");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GiaXK_CapDien, dt, sheet, colContent: 1, colVal: -1, colQoQ: cQoQ, colQoQoY: cQoQoY, colUnit: -1, colPrice: -1, colValPrev: -1, colQoQPrev: -1, "Cap Dien");
        }

        private List<string> _lGDP = new List<string>
        {
            "GDP",
            "GDP HH"
        };
        private void GDP(ExcelWorksheet sheet, DateTime dt)
        {
            var cValPrev = -1;
            var cVal = -1;
            var quarter = dt.GetQuarter();
            if (quarter == 1)
            {
                cVal = 3;
            }
            else
            {
                cValPrev = 3;
                cVal = 4;
            }

            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GDP_YTE, dt, sheet, colContent: 2, colVal: cVal, colQoQ: -1, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: -1, "Y Te");
            InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe.QUY_GDP_NganHangBaoHiem, dt, sheet, colContent: 2, colVal: cVal, colQoQ: -1, colQoQoY: -1, colUnit: -1, colPrice: -1, colValPrev: cValPrev, colQoQPrev: -1, "Bao Hiem");
        }

        private bool InsertThongKeOnlyRecord_Quy(EKeyTongCucThongKe eThongKe, DateTime dt, ExcelWorksheet sheet, int colContent, int colVal, int colQoQ, int colQoQoY, int colUnit, int colPrice, int colValPrev, int colQoQPrev, string textCompare, string textIgnore = "", int colQoQTry = -1)
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

                    if (colPrice > 0)
                    {
                        var valStr = sheet.Cells[i, colPrice].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        if (val > 0)
                        {
                            model.price = Math.Round(model.va * 1000 / val, 1);
                        }
                    }

                    if (colValPrev > 0)
                    {
                        var valStr = sheet.Cells[i, colValPrev].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        var va = isDouble ? Math.Round(val, 1) : 0;
                        if (va > 0)
                        {
                            var dtPrev = dt.AddMonths(-3);
                            FilterDefinition<ThongKeQuy> filter = null;
                            var builder = Builders<ThongKeQuy>.Filter;
                            var lFilter = new List<FilterDefinition<ThongKeQuy>>()
                            {
                                builder.Eq(x => x.d, int.Parse($"{dtPrev.Year}{dtPrev.GetQuarter()}")),
                                builder.Eq(x => x.key, (int)eThongKe)
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }

                            var entityPrev = _thongkeQuyRepo.GetEntityByFilter(filter);
                            if (entityPrev != null)
                            {
                                entityPrev.va = va;
                                _thongkeQuyRepo.Update(entityPrev);
                            }
                        }
                    }

                    if (colQoQPrev > 0)
                    {
                        var valStr = sheet.Cells[i, colQoQPrev].Value?.ToString().Trim() ?? string.Empty;
                        var isDouble = double.TryParse(valStr.Replace(",", ""), out var val);
                        var va = isDouble ? Math.Round(val, 1) : 0;
                        if (va > 0)
                        {
                            var dtPrev = dt.AddMonths(-3);
                            FilterDefinition<ThongKeQuy> filter = null;
                            var builder = Builders<ThongKeQuy>.Filter;
                            var lFilter = new List<FilterDefinition<ThongKeQuy>>()
                            {
                                builder.Eq(x => x.d, int.Parse($"{dtPrev.Year}{dtPrev.GetQuarter()}")),
                                builder.Eq(x => x.key, (int)eThongKe)
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }

                            var entityPrev = _thongkeQuyRepo.GetEntityByFilter(filter);
                            if (entityPrev != null)
                            {
                                entityPrev.qoq = va;
                                _thongkeQuyRepo.Update(entityPrev);
                            }
                        }
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
    }
}

