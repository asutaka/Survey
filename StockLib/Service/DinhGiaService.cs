using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Skender.Stock.Indicators;
using StockLib.DAL;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace StockLib.Service
{
    public interface IDinhGiaService
    {
        Task<string> Mes_DinhGia(string input);
    }
    public partial class DinhGiaService : IDinhGiaService
    {
        private readonly ILogger<BllService> _logger;

        private readonly IChiSoPERepo _peRepo;
        private readonly IKeHoachRepo _kehoachRepo;
        private readonly IThongKeRepo _thongkeRepo;
        private readonly IThongKeQuyRepo _thongkequyRepo;
        private readonly IThongKeHaiQuanRepo _haiquanRepo;
        
        private readonly ISpecialInfoRepo _specRepo;
        private readonly IFinancialRepo _financialRepo;

        private readonly IAPIService _apiService;
        public DinhGiaService(ILogger<BllService> logger,
                            IChiSoPERepo peRepo,
                            IKeHoachRepo kehoachRepo,
                            IThongKeRepo thongkeRepo,
                            IThongKeQuyRepo thongkequyRepo,
                            IThongKeHaiQuanRepo haiquanRepo,
                            ISpecialInfoRepo specRepo,
                            IFinancialRepo financialRepo,
                            IAPIService apiService)
        {
            _logger = logger;
            _peRepo = peRepo;
            _kehoachRepo = kehoachRepo;
            _haiquanRepo = haiquanRepo;
            _thongkeRepo = thongkeRepo;
            _thongkequyRepo = thongkequyRepo;
            _specRepo = specRepo;
            _apiService = apiService;
            _financialRepo = financialRepo;
        }

        public async Task<string> Mes_DinhGia(string input)
        {
            var stock = StaticVal._lStock.FirstOrDefault(x => x.s == input);
            if (stock == null)
                return null;

            var strRes = new StringBuilder();
            strRes.AppendLine($"Mã cổ phiếu {stock.s}: {stock.p.n}");

            var mesThongTin = Mes_ThongTinCoPhieu(input);
            if(!string.IsNullOrWhiteSpace(mesThongTin))
            {
                strRes.AppendLine($"+ Lĩnh vực: {mesThongTin}");
            }

            var lQuote = await _apiService.SSI_GetDataStock(stock.s);
            var pe = PECoPhieu(input, lQuote);
            if(!string.IsNullOrWhiteSpace(pe))
            {
                strRes.AppendLine($"+ P/E");
                strRes.AppendLine(pe);
                strRes.AppendLine();
            }

            if (stock.f?.Any() ?? false)
            {
                foreach (var item in stock.f)
                {
                    var leconomic = await _apiService.Tradingeconimic_Commodities();
                    if (stock.IsCrude_Oil())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.Crude_Oil.ToString());
                        if(first != null)
                        {
                            strRes.AppendLine($"+ Giá dầu thô: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsNatural_gas())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.Natural_gas.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá khí tự nhiên: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsCoal())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.Coal.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá Than: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsRubber())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.Rubber.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá Cao su: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsSteel())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.Steel.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá Thép: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsHRC_Steel())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.HRC_Steel.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá Thép HRC: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsGold())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.Gold.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá Vàng: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsCoffee())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.Coffee.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá Cà phê: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsRice())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.Rice.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá Gạo: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsSugar())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.Sugar.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá Đường: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsUrea())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.Urea.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá U-rê: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsPolyvinyl())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.polyvinyl.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá nhựa PVC: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsNickel())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.Nickel.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá Niken: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }
                    if (stock.IsMilk())
                    {
                        var first = leconomic.FirstOrDefault(x => x.Code == EPrice.milk.ToString());
                        if (first != null)
                        {
                            strRes.AppendLine($"+ Giá sữa: Weekly({first.Weekly}%)|YoY({first.YoY})");
                        }
                    }

                    if (stock.IsWCI())
                    {
                        var wci = await _apiService.Drewry_WCI();
                        strRes.AppendLine($"+ Giá cước Container(weekly): {wci.Item1}%| YoY: {wci.Item2}%");
                    }
                    if (stock.IsYellowPhotpho())
                    {
                        var lPhotpho = await _apiService.Metal_GetYellowPhotpho();
                        if (lPhotpho?.Any() ?? false)
                        {
                            var cur = lPhotpho.First();
                            var time = cur.metalsPrice.renewDate.ToDateTime("yyyy-MM-dd");
                            var nearTime = time.AddDays(-6);

                            var near = lPhotpho.FirstOrDefault(x => x.metalsPrice.renewDate == $"{nearTime.Year}-{nearTime.Month.To2Digit()}-{nearTime.Day.To2Digit()}"
                                                                || x.metalsPrice.renewDate == $"{nearTime.Year}-{nearTime.Month.To2Digit()}-{(nearTime.Day - 1).To2Digit()}"
                                                                || x.metalsPrice.renewDate == $"{nearTime.Year}-{nearTime.Month.To2Digit()}-{(nearTime.Day - 2).To2Digit()}"
                                                                || x.metalsPrice.renewDate == $"{nearTime.Year}-{nearTime.Month.To2Digit()}-{(nearTime.Day - 3).To2Digit()}");
                            if (near != null)
                            {
                                var prev = lPhotpho.FirstOrDefault(x => x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{time.Day.To2Digit()}");
                                if (prev is null)
                                {
                                    prev = lPhotpho.FirstOrDefault(x => x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{time.Day.To2Digit()}"
                                                                    || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day + 1).To2Digit()}"
                                                                    || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day + 2).To2Digit()}"
                                                                    || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day + 3).To2Digit()}"
                                                                    || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day + 4).To2Digit()}"
                                                                    || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day + 5).To2Digit()}"
                                                                    || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day + 6).To2Digit()}"
                                                                    || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day - 1).To2Digit()}"
                                                                    || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day - 2).To2Digit()}"
                                                                    || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day - 3).To2Digit()}"
                                                                    || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day - 4).To2Digit()}");
                                }

                                var rate = Math.Round(100 * (-1 + cur.metalsPrice.average / near.metalsPrice.average), 1);
                                var strMes = $"- Giá phốt pho vàng(weekly): {rate}%";
                                if (prev != null)
                                {
                                    var ratePrev = Math.Round(100 * (-1 + cur.metalsPrice.average / prev.metalsPrice.average), 1);
                                    strMes += $" |YoY: {Math.Round(ratePrev, 1)}%";
                                }
                                strRes.AppendLine(strMes);
                            }
                        }
                    }
                    if (stock.IsBDTI())
                    {
                        var bdti = await _apiService.Macrovar_Commodities(); //BDTI: cước vận tải dầu
                        if (bdti != null)
                        {
                            strRes.AppendLine($"+ Cước vận tải dầu thô(weekly): {bdti.ow}%| YoY: {bdti.oy}%");
                        }
                    }
                    if (stock.IsDXY())
                    {
                        var lVal = await _apiService.VietStock_GetForex(EForex.DXU1.ToString());
                        if (lVal is null || !lVal.t.Any())
                            return string.Empty;

                        var c_last = lVal.c.Last();
                        var c_near = lVal.c.SkipLast(6).Last();

                        var t_prev = ((long)lVal.t.Last()).UnixTimeStampToDateTime().AddYears(-1).AddMonths(1);
                        var dtPrev = new DateTime(t_prev.Year, t_prev.Month, 1, 0, 0, 0);
                        var timestamp = new DateTimeOffset(dtPrev).ToUnixTimeSeconds();
                        var t_index = lVal.t.Where(x => x < timestamp).Max(x => x);
                        var index = lVal.t.IndexOf(t_index);
                        var c_prev = lVal.c.ElementAt(index);

                        var weekly = Math.Round(100 * (-1 + c_last / c_near), 1);
                        var qoq = Math.Round(100 * (-1 + c_last / c_prev), 1);
                        return $"+ Chỉ số USD QoQ(weekly): {weekly}%| YoY: {qoq}%";
                    }
                }
            }

            if(stock.IsBanLe())
            {
                strRes.AppendLine(ModeThongKe(EKeyTongCucThongKe.BanLe));
            }

            if(stock.IsBDS())
            {
                strRes.AppendLine(DG_BDS(stock.s));
            }    

            if (stock.IsCaPhe())
            {
                var eHaiQuan = EHaiQuan.None;
                var eThongKe = EKeyTongCucThongKe.XK_CaPhe;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            if(stock.IsCangBien())
            {
                var vt = ModeThongKe(EKeyTongCucThongKe.VanTai_DuongBien);
                var gia = ModeThongKe(EKeyTongCucThongKe.QUY_GiaVT_KhoBai);
                strRes.AppendLine(vt);
                strRes.AppendLine(gia);
            }

            if (stock.IsCaoSu())
            {
                var eHaiQuan = EHaiQuan.CaoSu;
                var eThongKe = EKeyTongCucThongKe.XK_CaoSu;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            if(stock.IsChungKhoan())
            {
                strRes.AppendLine(await DG_ChungKhoan(stock.s));
            }    

            if (stock.IsDauTuCong())
            {
                strRes.AppendLine(ModeThongKe(EKeyTongCucThongKe.DauTuCong));
            }

            if(stock.IsDetMay())
            {
                var eHaiQuan = EHaiQuan.DetMay;
                var eThongKe = EKeyTongCucThongKe.XK_DetMay;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            if(stock.IsGao())
            {
                var eHaiQuan = EHaiQuan.Gao;
                var eThongKe = EKeyTongCucThongKe.XK_Gao;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            if(stock.IsGo())
            {
                var eHaiQuan = EHaiQuan.Go;
                var eThongKe = EKeyTongCucThongKe.XK_Go;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            if(stock.IsHangKhong())
            {
                var vt = ModeThongKe(EKeyTongCucThongKe.VanTai_HangKhong);
                var hk = ModeThongKe(EKeyTongCucThongKe.HanhKhach_HangKhong);
                var giave = ModeThongKe(EKeyTongCucThongKe.QUY_GiaVT_HangKhong);
                strRes.AppendLine(vt);
                strRes.AppendLine(hk);
                strRes.AppendLine(giave);
            }

            if (stock.IsHoaChat())
            {
                var eHaiQuan = EHaiQuan.HoaChat;
                var eThongKe = EKeyTongCucThongKe.XK_HoaChat;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            if (stock.IsNhua())
            {
                var eHaiQuan = EHaiQuan.SPChatDeo;
                var eThongKe = EKeyTongCucThongKe.XK_SPChatDeo;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            if(stock.IsOto())
            {
                var eHaiQuan = EHaiQuan.Oto9Cho_NK;
                var eThongKe = EKeyTongCucThongKe.None;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            if (stock.IsOtoTai())
            {
                var eHaiQuan = EHaiQuan.OtoVanTai_NK;
                var eThongKe = EKeyTongCucThongKe.None;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            if(stock.IsPhanBon())
            {
                var eHaiQuan = EHaiQuan.PhanBon;
                var eThongKe = EKeyTongCucThongKe.None;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            if(stock.IsThan())
            {
                var eHaiQuan = EHaiQuan.Than;
                var eThongKe = EKeyTongCucThongKe.None;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            if (stock.IsThep())
            {
                var eHaiQuan = EHaiQuan.SatThep;
                var eThongKe = EKeyTongCucThongKe.XK_SatThep;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            if(stock.IsThuySan())
            {
                var eHaiQuan = EHaiQuan.ThuySan;
                var eThongKe = EKeyTongCucThongKe.XK_ThuySan;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }    

            if(stock.IsXimang())
            {
                var eHaiQuan = EHaiQuan.Ximang;
                var eThongKe = EKeyTongCucThongKe.XK_Ximang;
                strRes.AppendLine(ModeXNK(eThongKe, eHaiQuan));
            }

            

            return strRes.ToString();
        }

        public string Mes_ThongTinCoPhieu(string code)
        {
            if (code.Equals("PVD"))
            {
                return $"Cho thuê giàn khoan";
            }
            else if (code.Equals("PVS"))
            {
                return $"Thăm dò và khai thác dầu khí";
            }
            else if (code.Equals("PVT") || code.Equals("PVP"))
            {
                return $"Vận tải dầu khí";
            }
            else if (code.Equals("GAS"))
            {
                return $"Chế biến và phân phối khí(mua trong nước + nhập khẩu)";
            }
            else if (code.Equals("BSR"))
            {
                return $"Chế biến dầu mỏ(100% nhập khẩu)";
            }
            else if (code.Equals("POW"))
            {
                return $"Điện khí(khí là nguyên liệu đầu vào)";
            }
            else if (code.Equals("PLX"))
            {
                return $"Phân phối dầu khí(chiếm 50% thị phần)";
            }
            else if (code.Equals("OIL"))
            {
                return $"Phân phối dầu khí(chiếm 20% thị phần)";
            }
            return string.Empty;
        }

        private EPoint Swap(EPoint point)
        {
            if (point == EPoint.VeryPositive)
                return EPoint.VeryNegative;
            if (point == EPoint.VeryNegative)
                return EPoint.VeryPositive;
            if (point == EPoint.Positive)
                return EPoint.Negative;
            if (point == EPoint.Negative)
                return EPoint.Positive;
            return point;
        }

        private EPoint MergeEnpoint(EPoint e1, EPoint e2)
        {
            var res = 0.5 * ((double)e1 + (double)e2);
            if (res > (double)EPoint.Positive)
                return EPoint.VeryPositive;
            if (res > (double)EPoint.Normal)
                return EPoint.Positive;
            if (res > (double)EPoint.Negative)
                return EPoint.Normal;
            if (res > (double)EPoint.VeryNegative)
                return EPoint.Negative;
            return EPoint.VeryNegative;
        }

        private EPoint MergeEnpoint(EPoint e1, EPoint e2, EPoint e3)
        {
            var res = ((double)e1 + (double)e2 + (double)e3) / 3;
            if (res > (double)EPoint.Positive)
                return EPoint.VeryPositive;
            if (res > (double)EPoint.Normal)
                return EPoint.Positive;
            if (res > (double)EPoint.Negative)
                return EPoint.Normal;
            if (res > (double)EPoint.VeryNegative)
                return EPoint.Negative;
            return EPoint.VeryNegative;
        }

        private string ModeXNK(EKeyTongCucThongKe eThongKe, EHaiQuan eHaiQuan)
        {
            try
            {
                var lHaiQuan = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)eHaiQuan));
                ThongKeHaiQuan haiquan = null;
                ThongKe thongke = null;
                ThongKeQuy thongkequy = null;
                if (lHaiQuan != null && lHaiQuan.Any()) 
                {
                    haiquan = lHaiQuan.MaxBy(x => x.d);
                }
                var lThongKe = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)eThongKe));
                if (lThongKe != null && lThongKe.Any())
                {
                    thongke = lThongKe.MaxBy(x => x.d);
                }
                var lThongKeQuy = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)eThongKe));
                if (lThongKeQuy != null && lThongKeQuy.Any())
                {
                    thongkequy = lThongKeQuy.MaxBy(x => x.d);
                }
                var dHaiQuan = 0;
                var dThongKe = 0;
                var dThongKeQuy = 0;
                if(haiquan != null)
                {
                    var year = haiquan.d / 1000;
                    var month = (haiquan.d - 1000 * year) / 10;
                    var div = haiquan.d - (1000 * year + month * 10);
                    var day = (div <= 1) ? 1 : 28;
                    dHaiQuan = int.Parse($"{year}{month.To2Digit()}{day.To2Digit()}");
                }
                if(thongke != null)
                {
                    var year = thongke.d / 100;
                    var month = thongke.d - 100 * year;
                    var day = 15;
                    dThongKe = int.Parse($"{year}{month.To2Digit()}{day.To2Digit()}");
                }
                if(thongkequy != null)
                {
                    var year = thongkequy.d / 10;
                    var quarter = thongkequy.d - 10 * year;
                    var day = 30;
                    var month = 0;
                    if (quarter == 1)
                    {
                        month = 3;
                    }
                    else if (quarter == 2)
                    {
                        month = 6;
                    }
                    else if (quarter == 3)
                    {
                        month = 9;
                    }
                    else
                    {
                        month = 12;
                    }
                    dThongKeQuy = int.Parse($"{year}{month.To2Digit()}{day.To2Digit()}");
                }

                var mode = 1;
                var max = dHaiQuan;
                if(dThongKe > max)
                {
                    max = dThongKe;
                    mode = 2;
                }
                if(dThongKeQuy > max)
                {
                    max = dThongKeQuy;
                    mode = 3;
                }

                if (mode == 1)
                {
                    return HaiQuanXNK(eHaiQuan);
                }
                else if(mode == 2)
                {
                    return ThongKeXNK(eThongKe);
                }
                else
                {
                    return ThongKeQuyXNK(eThongKe);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.ModeXNK|EXCEPTION| {ex.Message}");
            }
            return string.Empty;
        }

        private string HaiQuanXNK(EHaiQuan eHaiQuan)
        {
            try
            {
                var lXK = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)eHaiQuan)).OrderByDescending(x => x.d);
                if (lXK is null || !lXK.Any())
                {
                    return string.Empty;
                }
                var cur = lXK.First();
                var prev = lXK.FirstOrDefault(x => x.d == cur.d - 1000);
                if (prev is null)
                {
                    return string.Empty;
                }

                var rate = Math.Round(Math.Round(cur.va * 100 / prev.va, 1) - 100, 1);
                return $"   - Xuất khẩu Hải Quan nửa Tháng cùng kỳ: {rate}%";
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.HaiQuanXNK|EXCEPTION| {ex.Message}");
            }
            return string.Empty; 
        }

        private string ThongKeXNK(EKeyTongCucThongKe eThongKe)
        {
            try
            {
                var lThongKe = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)eThongKe)).OrderByDescending(x => x.d);
                if(lThongKe is null || !lThongKe.Any())
                {
                    return string.Empty;
                }    

                var cur = lThongKe.FirstOrDefault();
                var rate = Math.Round(cur.qoq - 100, 1);
                return $"   - Xuất khẩu Tháng cùng kỳ: {rate}%";
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.ThongKeXNK|EXCEPTION| {ex.Message}");
            }
            return string.Empty;
        }

        private string ThongKeQuyXNK(EKeyTongCucThongKe eThongKe)
        {
            try
            {
                var lThongKe = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)eThongKe)).OrderByDescending(x => x.d);
                if (!lThongKe.Any())
                {
                    return string.Empty;
                }

                var cur = lThongKe.FirstOrDefault();
                var rate = Math.Round(cur.qoq - 100, 1);
                return $"   - Xuất khẩu Quý cùng kỳ: {rate}%";
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.ThongKeQuyXNK|EXCEPTION| {ex.Message}");
            }
            return string.Empty;
        }

        private string ModeThongKe(EKeyTongCucThongKe eThongKe)
        {
            try
            {
                ThongKe thongke = null;
                ThongKeQuy thongkequy = null;
                var lThongKe = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)eThongKe));
                if (lThongKe != null && lThongKe.Any())
                {
                    thongke = lThongKe.MaxBy(x => x.d);
                }
                var lThongKeQuy = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)eThongKe));
                if (lThongKeQuy != null && lThongKeQuy.Any())
                {
                    thongkequy = lThongKeQuy.MaxBy(x => x.d);
                }
                var dThongKe = 0;
                var dThongKeQuy = 0;
                if (thongke != null)
                {
                    var year = thongke.d / 100;
                    var month = thongke.d - 100 * year;
                    var day = 15;
                    dThongKe = int.Parse($"{year}{month.To2Digit()}{day.To2Digit()}");
                }
                if (thongkequy != null)
                {
                    var year = thongkequy.d / 10;
                    var quarter = thongkequy.d - 10 * year;
                    var day = 30;
                    var month = 0;
                    if (quarter == 1)
                    {
                        month = 3;
                    }
                    else if (quarter == 2)
                    {
                        month = 6;
                    }
                    else if (quarter == 3)
                    {
                        month = 9;
                    }
                    else
                    {
                        month = 12;
                    }
                    dThongKeQuy = int.Parse($"{year}{month.To2Digit()}{day.To2Digit()}");
                }

                var mode = 1;
                var max = dThongKeQuy;
                if (dThongKe > max)
                {
                    max = dThongKe;
                    mode = 2;
                }

                if (mode == 2)
                {
                    return ThongKeOther(eThongKe);
                }
                else
                {
                    return ThongKeQuyOther(eThongKe);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.ModeThongKe|EXCEPTION| {ex.Message}");
            }
            return string.Empty;
        }

        private string ThongKeOther(EKeyTongCucThongKe eThongKe)
        {
            try
            {
                var lThongKe = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)eThongKe)).OrderByDescending(x => x.d);
                if (lThongKe is null || !lThongKe.Any())
                {
                    return string.Empty;
                }

                var strTitle = eThongKe.GetDisplayName();
                var cur = lThongKe.First();
                double rate = 0;
                if(cur.qoq == 0)
                {
                    var prev = lThongKe.FirstOrDefault(x => x.d == cur.d - 100);
                    if (prev is null || prev.va <= 0)
                    {
                        return string.Empty;
                    }
                    rate = Math.Round(100 * (-1 + cur.va / prev.va), 1);
                }
                else
                {
                    rate = Math.Round(cur.qoq - 100, 1);
                }

                return $"   - {strTitle} Tháng cùng kỳ: {rate}%";
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.ThongKeOther|EXCEPTION| {ex.Message}");
            }
            return string.Empty;
        }

        private string ThongKeQuyOther(EKeyTongCucThongKe eThongKe)
        {
            try
            {
                var lThongKe = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)eThongKe)).OrderByDescending(x => x.d);
                if (!lThongKe.Any())
                {
                    return string.Empty;
                }

                var strTitle = eThongKe.GetDisplayName();
                var cur = lThongKe.First();
                double rate = 0;
                if (cur.qoq == 0)
                {
                    var prev = lThongKe.FirstOrDefault(x => x.d == cur.d - 100);
                    if (prev is null || prev.va <= 0)
                    {
                        return string.Empty;
                    }
                    rate = Math.Round(100 * (-1 + cur.va / prev.va), 1);
                }
                else
                {
                    rate = Math.Round(cur.qoq - 100, 1);
                }

                return $"   - {strTitle} Quý cùng kỳ: {rate}%";
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.ThongKeQuyXNK|EXCEPTION| {ex.Message}");
            }
            return string.Empty;
        }

        private string PECoPhieu(string code, List<Quote> lQuote)
        {
            try
            {
                var dt = DateTime.Now;
                if (lQuote is null || !lQuote.Any())
                {
                    return string.Empty;
                }
                //pe
                var lpe = _peRepo.GetByFilter(Builders<ChiSoPE>.Filter.Eq(x => x.s, code));
                if (lpe is null || !lpe.Any())
                {
                    return string.Empty;
                }

                var quote = lQuote.MaxBy(x => x.Date);//Giá mới nhất
                var lKehoach = _kehoachRepo.GetByFilter(Builders<KeHoach>.Filter.Eq(x => x.s, code));
                var curPlan = lKehoach?.FirstOrDefault(x => x.d == dt.Year);
                if (lKehoach is null || !lKehoach.Any() || curPlan is null)
                {
                    return DinhGiaPEkoKeHoach(lpe, quote, dt);
                }

                double pf_truth = 0;
                var lpf_real = lKehoach.Where(x => x.pf_real > 0);
                double avgRate = 0;
                if (lpf_real.Any())
                {
                    avgRate = lpf_real.Average(x => x.pf_real_r);
                }

                var cum = curPlan.pf_cum;
                if (cum > 0)
                {
                    var quarter = dt.GetQuarter() - 1;
                    var flag = curPlan.pf_cum_r >= quarter * 25;
                    if (flag)
                    {
                        pf_truth = curPlan.pf_plan;
                    }
                }

                if (pf_truth == 0)
                {
                    pf_truth = Math.Round(curPlan.pf_plan * avgRate / 100, 1);
                }

                if (pf_truth <= 0)
                {
                    return string.Empty;
                }

                //True Path 
                var stock = StaticVal._lStock.FirstOrDefault(x => x.s == code);
                if (stock.p.q <= 0)
                {
                    return string.Empty;
                }

                var eps_truth = Math.Round(pf_truth * 1000000000 / stock.p.q, 1);
                if (eps_truth == 0)
                {
                    return string.Empty;
                }
                
                var pe_truth = Math.Round((double)quote.Close * 1000 / eps_truth, 1);
                if (pe_truth <= 0)
                {
                    return string.Empty;
                }
                var sBuilder = new StringBuilder();
                sBuilder.AppendLine($"   - PE dự phóng: {Math.Round(pe_truth, 1)}");

                var lastPE = lpe.MaxBy(x => x.d);
                if (lastPE.eps <= 0)
                {
                    return sBuilder.ToString();
                }

                var pe_cur = Math.Round((double)quote.Close * 1000 / lastPE.eps, 1);
                var pe_avg = lpe.Where(x => x.d.ToString().EndsWith(dt.GetQuarter().ToString())).Average(x => x.pe);
                sBuilder.AppendLine($"   - PE trung bình quý {dt.GetQuarterStr()}: {Math.Round(pe_avg, 1)}");
                sBuilder.AppendLine($"   - PE hiện tại: {Math.Round(pe_cur, 1)}");

                return sBuilder.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DinhGiaPE|EXCEPTION| {ex.Message}");
            }
            return string.Empty;
        }

        private string DinhGiaPEkoKeHoach(List<ChiSoPE> lpe, Quote quote, DateTime dt)
        {
            var sBuilder = new StringBuilder();
            
            var pe_avg = lpe.Where(x => x.d.ToString().EndsWith(dt.GetQuarter().ToString())).Average(x => x.pe);
            sBuilder.AppendLine($"   - PE trung bình: {Math.Round(pe_avg, 1)}");

            var lastPE = lpe.MaxBy(x => x.d);
            if (lastPE.eps == 0)
            {
                return sBuilder.ToString();
            }

            var pe_cur = Math.Round((double)quote.Close * 1000 / lastPE.eps, 1);
            sBuilder.AppendLine($"   - PE hiện tại: {Math.Round(pe_cur, 1)}");
            
            return sBuilder.ToString();
        }

        private (EPoint, string) EPointResponse(double val, double step1, double step2, string mes)
        {
            (EPoint, string) output;
            output.Item2 = $"   - {mes}: {val}%";
            if (val >= step2)
            {
                output.Item1 = EPoint.VeryPositive;
            }
            else if (val >= step1)
            {
                output.Item1 = EPoint.Positive;
            }
            else if (val >= -step1)
            {
                output.Item1 = EPoint.Normal;
            }
            else if (val >= -step2)
            {
                output.Item1 = EPoint.Negative;
            }
            else
            {
                output.Item1 = EPoint.VeryNegative;
            }
            return output;
        }
    }
}
