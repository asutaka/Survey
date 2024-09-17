using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Skender.Stock.Indicators;
using StockLib.DAL;
using StockLib.DAL.Entity;
using StockLib.Utils;
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
        private readonly IShareRepo _shareRepo;
        private readonly IKeHoachRepo _kehoachRepo;
        private readonly IThongKeRepo _thongkeRepo;
        private readonly IThongKeQuyRepo _thongkequyRepo;
        private readonly IThongKeHaiQuanRepo _haiquanRepo;
        private readonly IStockTypeRepo _stockTypeRepo;


        private readonly IAPIService _apiService;
        public DinhGiaService(ILogger<BllService> logger,
                            IChiSoPERepo peRepo,
                            IShareRepo shareRepo,
                            IKeHoachRepo kehoachRepo,
                            IThongKeRepo thongkeRepo,
                            IThongKeQuyRepo thongkequyRepo,
                            IThongKeHaiQuanRepo haiquanRepo,
                            IStockTypeRepo stockTypeRepo,
                            IAPIService apiService)
        {
            _logger = logger;
            _peRepo = peRepo;
            _shareRepo = shareRepo;
            _kehoachRepo = kehoachRepo;
            _haiquanRepo = haiquanRepo;
            _thongkeRepo = thongkeRepo;
            _thongkequyRepo = thongkequyRepo;
            _stockTypeRepo = stockTypeRepo;
            _apiService = apiService;
        }
        private async Task<(EPoint, string, EStockType)> DinhGiaNganh(string code, int nganh)
        {
            var eNganh = (EStockType)nganh;
            if(eNganh == EStockType.BanLe)
            {
                return (await DG_BanLe(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.BDS)
            {
                return (await DG_BDS(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.CangBien)
            {
                return (await DG_CangBien(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.CaoSu)
            {
                return (await DG_CaoSu(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.ChungKhoan)
            {
                return (await DG_ChungKhoan(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.DauKhi)
            {
                return (await DG_DauKhi(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.DetMay)
            {
                return (await DG_DetMay(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.DienGio)
            {
                return (await DG_DienGio(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.DienKhi)
            {
                return (await DG_DienKhi(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.DienMatTroi)
            {
                return (await DG_DienMatTroi(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.DienThan)
            {
                return (await DG_DienThan(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.ThuyDien)
            {
                return (await DG_ThuyDien(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Forex)
            {
                return (Swap(await DinhGia_Forex(EForex.DXU1, 2, 5)), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Go)
            {
                var go = DG_Go(code);
                return (go.Item1, go.Item2, eNganh);
            }

            if (eNganh == EStockType.HangKhong)
            {
                return (await DG_HangKhong(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.KCN)
            {
                return (await DG_KCN(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Logistic)
            {
                return (await DG_Logistic(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.NganHang)
            {
                return (await DG_NganHang(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Nhua)
            {
                return (await DG_Nhua(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Oto)
            {
                return (await DG_Oto(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.PhanBon)
            {
                return (await DG_PhanBon(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Than)
            {
                return (await DG_Than(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Thep)
            {
                return (await DG_Thep(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.ThuySan)
            {
                return (await DG_ThuySan(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Vin)
            {
                return (await DG_Vin(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.XayDung)
            {
                return (await DG_XayDung(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.XiMang)
            {
                return (await DG_XiMang(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.VanTaiBien)
            {
                return (await DG_VanTaiBien(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.ChanNuoi)
            {
                return (await DG_ChanNuoi(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.NongNghiep)
            {
                return (await DG_NongNghiep(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.HoaChat)
            {
                return (await DG_HoaChat(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.CaPhe)
            {
                return (await DG_CaPhe(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Gao)
            {
                return (await DG_Gao(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Duoc)
            {
                return (await DG_Duoc(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.DichVuYTe)
            {
                return (await DG_DichVuYTe(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.BaoHiem)
            {
                return (await DG_BaoHiem(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.CNTT)
            {
                return (await DG_CNTT(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.DauTuCong)
            {
                return (await DG_DauTuCong(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.ThietBiDien)
            {
                return (await DG_ThietBiDien(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Duong)
            {
                return (await DG_Duong(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Bia)
            {
                return (await DG_Bia(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.SPNongNghiepKhac)
            {
                return (await DG_SPNongNghiepKhac(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.NuocNgot)
            {
                return (await DG_NuocNgot(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Sua)
            {
                return (await DG_Sua(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.XuatKhau)
            {
                //return (await DG_XuatKhau(code, string.Empty, eNganh), string.Empty, eNganh);
            }

            if (eNganh == EStockType.NangLuong)
            {
                return (await DG_NangLuong(code), string.Empty, eNganh);
            }

            if (eNganh == EStockType.Khac)
            {
                return (await DG_Khac(code), string.Empty, eNganh);
            }

            return (EPoint.Unknown, string.Empty, eNganh);
        }

        public async Task<string> Mes_DinhGia(string input)
        {
            var stock = _stockTypeRepo.GetEntityByFilter(Builders<StockType>.Filter.Eq(x => x.s, input));
            if (stock == null)
                return null;

            var strRes = new StringBuilder();
            strRes.AppendLine($"Định giá cổ phiếu {stock.s}");

            var lQuote = await _apiService.SSI_GetDataStock(stock.s);
            var pe = DinhGiaPE(input, lQuote);
            strRes.AppendLine($"+ P/E: {pe.Item1.GetDisplayName()}");
            if(!string.IsNullOrWhiteSpace(pe.Item2))
            {
                strRes.AppendLine(pe.Item2);
            }

            var lDinhGia = new List<double>();
            if(stock.ty1 > -1)
            {
                var dg = await DinhGiaNganh(stock.s, stock.ty1);
                if(dg.Item1 != EPoint.Unknown)
                {
                    strRes.AppendLine($"+ {dg.Item3.GetDisplayName()}: {dg.Item1.GetDisplayName()}");
                    if (!string.IsNullOrWhiteSpace(dg.Item2))
                    {
                        strRes.AppendLine(dg.Item2);
                    }
                    lDinhGia.Add((double)dg.Item1 * stock.ty1_r / 100);
                }
            }
            if (stock.ty2 > -1)
            {
                var dg = await DinhGiaNganh(stock.s, stock.ty2);
                if (dg.Item1 != EPoint.Unknown)
                {
                    strRes.AppendLine($"+ {dg.Item3.GetDisplayName()}: {dg.Item1.GetDisplayName()}");
                    if (!string.IsNullOrWhiteSpace(dg.Item2))
                    {
                        strRes.AppendLine(dg.Item2);
                    }
                    lDinhGia.Add((double)dg.Item1 * stock.ty2_r / 100);
                }
            }
            if (stock.ty3 > -1)
            {
                var dg = await DinhGiaNganh(stock.s, stock.ty3);
                if (dg.Item1 != EPoint.Unknown)
                {
                    strRes.AppendLine($"+  {dg.Item3.GetDisplayName()}: {dg.Item1.GetDisplayName()}");
                    if (!string.IsNullOrWhiteSpace(dg.Item2))
                    {
                        strRes.AppendLine(dg.Item2);
                    }
                    lDinhGia.Add((double)dg.Item1 * stock.ty3_r / 100);
                }
            }

            strRes.AppendLine();
            if (!lDinhGia.Any())
            {
                strRes.AppendLine($"=> Kết Luận: {pe.Item1.GetDisplayName()}");
                return strRes.ToString();
            }    
                
            var avgPoint = Math.Round(lDinhGia.Sum() / lDinhGia.Count(), 1);
            var total = Math.Round(((double)pe.Item1 + avgPoint) / 2, 1);
            if(total > (double)EPoint.Positive)
            {
                strRes.AppendLine($"=> Kết Luận: {EPoint.VeryPositive.GetDisplayName()}");
            }
            else if (total > (double)EPoint.Normal)
            {
                strRes.AppendLine($"=> Kết Luận: {EPoint.Positive.GetDisplayName()}");
            }
            else if (total > (double)EPoint.Negative)
            {
                strRes.AppendLine($"=> Kết Luận: {EPoint.Normal.GetDisplayName()}");
            }
            else if (total > (double)EPoint.VeryNegative)
            {
                strRes.AppendLine($"=> Kết Luận: {EPoint.Negative.GetDisplayName()}");
            }
            else
            {
                strRes.AppendLine($"=> Kết Luận: {EPoint.VeryNegative.GetDisplayName()}");
            }
            return strRes.ToString();

            //var isGo = stock.h24.Any(y => y.code == "1733");
            //if (isGo)
            //{
            //    var xk = await DinhGiaXNK(EHaiQuan.Go, 5, 15);
            //    strRes.AppendLine($"  + P/E: {pe.GetDisplayName()}");
            //    strRes.AppendLine($"  + Giá trị xuất khẩu: {xk.GetDisplayName()}");

            //    var xk_gia = await DinhGiaXNK_Gia(EHaiQuan.Go, 5, 15);
            //    if (xk_gia != EPoint.Unknown)
            //    {
            //        strRes.AppendLine($"  + Giá xuất khẩu: {xk.GetDisplayName()}");
            //    }

            //    //return await Chart_Go(input);
            //}
        }

        //private async Task<EPoint> DinhGiaThongKe(EKeyTongCucThongKe thongke)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.DinhGiaThongKe|EXCEPTION| {ex.Message}");
        //    }
        //}
        //private async Task<EPoint> DinhGiaThongKeQuy(EKeyTongCucThongKe thongke)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.DinhGiaThongKeQuy|EXCEPTION| {ex.Message}");
        //    }
        //}
        //private async Task<EPoint> DinhGiaXNK(EHaiQuan haiquan, double step1, double step2)
        //{
        //    try
        //    {
        //        var lXK = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)haiquan)).OrderByDescending(x => x.d);
        //        if (lXK is null || !lXK.Any())
        //            return EPoint.VeryNegative;

        //        var cur = lXK.First();
        //        var near = lXK.Skip(1).FirstOrDefault();
        //        var prev = lXK.FirstOrDefault(x => x.d == cur.d - 1000);
        //        double va = 0;
        //        if (prev != null && prev.va > 0)
        //        {
        //            var qoq_va = Math.Round(100 * (-1 + cur.va / prev.va), 1);
        //            var total_va = 0;
        //            if (qoq_va > step2)
        //            {
        //                total_va = (int)EPoint.VeryPositive;
        //            }
        //            else if (qoq_va <= step2 && qoq_va > step1)
        //            {
        //                total_va = (int)EPoint.Positive;
        //            }
        //            else if (qoq_va <= step1 && qoq_va >= -step1)
        //            {
        //                total_va = (int)EPoint.Normal;
        //            }
        //            else if (qoq_va < -step1 && qoq_va >= -step2)
        //            {
        //                total_va = (int)EPoint.Negative;
        //            }
        //            else
        //            {
        //                total_va = (int)EPoint.VeryNegative;
        //            }
        //            va = total_va;
        //        }
        //        if (near != null && near.va > 0)
        //        {
        //            var qoqoy_va = Math.Round(100 * (-1 + cur.va / near.va), 1);
        //            var total_va = 0;
        //            if (qoqoy_va > step2)
        //            {
        //                total_va = (int)EPoint.VeryPositive;
        //            }
        //            else if (qoqoy_va <= step2 && qoqoy_va > step1)
        //            {
        //                total_va = (int)EPoint.Positive;
        //            }
        //            else if (qoqoy_va <= step1 && qoqoy_va >= -step1)
        //            {
        //                total_va = (int)EPoint.Normal;
        //            }
        //            else if (qoqoy_va < -step1 && qoqoy_va >= -step2)
        //            {
        //                total_va = (int)EPoint.Negative;
        //            }
        //            else
        //            {
        //                total_va = (int)EPoint.VeryNegative;
        //            }
        //            va = va * 0.6 + total_va * 0.4;
        //        }

        //        if (va < (int)EPoint.Negative)
        //        {
        //            return EPoint.VeryNegative;
        //        }
        //        if (va < (int)EPoint.Normal)
        //        {
        //            return EPoint.Negative;
        //        }
        //        if (va < (int)EPoint.Positive)
        //        {
        //            return EPoint.Normal;
        //        }
        //        if (va < (int)EPoint.VeryPositive)
        //        {
        //            return EPoint.Positive;
        //        }
        //        return EPoint.VeryPositive;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"DinhGiaService.DinhGiaXNK|EXCEPTION| {ex.Message}");
        //    }
        //    return EPoint.VeryNegative;
        //}
        //private async Task<EPoint> DinhGiaXNK_Gia(EHaiQuan haiquan, double step1, double step2)
        //{
        //    try
        //    {
        //        var lXK = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)haiquan)).OrderByDescending(x => x.d);
        //        if (lXK is null || !lXK.Any())
        //            return EPoint.VeryNegative;

        //        var cur = lXK.First();
        //        var near = lXK.Skip(1).FirstOrDefault();
        //        var prev = lXK.FirstOrDefault(x => x.d == cur.d - 1000);
        //        double va = 0, price = 0;
        //        if (prev != null && prev.price > 0)
        //        {
        //            var qoq_price = Math.Round(100 * (-1 + cur.price / prev.price), 1);
        //            var total_price = 0;
        //            if (qoq_price > step2)
        //            {
        //                total_price = (int)EPoint.VeryPositive;
        //            }
        //            else if (qoq_price <= step2 && qoq_price > step1)
        //            {
        //                total_price = (int)EPoint.Positive;
        //            }
        //            else if (qoq_price <= step1 && qoq_price >= -step1)
        //            {
        //                total_price = (int)EPoint.Normal;
        //            }
        //            else if (qoq_price < -step1 && qoq_price >= -step2)
        //            {
        //                total_price = (int)EPoint.Negative;
        //            }
        //            else
        //            {
        //                total_price = (int)EPoint.VeryNegative;
        //            }
        //            price = total_price;
        //        }
        //        if (near != null && near.price > 0)
        //        {
        //            var qoqoy_price = Math.Round(100 * (-1 + cur.price / near.price), 1);
        //            var total_price = 0;
        //            if (qoqoy_price > step2)
        //            {
        //                total_price = (int)EPoint.VeryPositive;
        //            }
        //            else if (qoqoy_price <= step2 && qoqoy_price > step1)
        //            {
        //                total_price = (int)EPoint.Positive;
        //            }
        //            else if (qoqoy_price <= step1 && qoqoy_price >= -step1)
        //            {
        //                total_price = (int)EPoint.Normal;
        //            }
        //            else if (qoqoy_price < -step1 && qoqoy_price >= -step2)
        //            {
        //                total_price = (int)EPoint.Negative;
        //            }
        //            else
        //            {
        //                total_price = (int)EPoint.VeryNegative;
        //            }
        //            price = price * 0.6 + total_price * 0.4;
        //        }
        //        if (price <= 0)
        //            return EPoint.Unknown;
        //        if (price < (int)EPoint.Negative)
        //        {
        //            return EPoint.VeryNegative;
        //        }
        //        if (price < (int)EPoint.Normal)
        //        {
        //            return EPoint.Negative;
        //        }
        //        if (price < (int)EPoint.Positive)
        //        {
        //            return EPoint.Normal;
        //        }
        //        if (price < (int)EPoint.VeryPositive)
        //        {
        //            return EPoint.Positive;
        //        }
        //        return EPoint.VeryPositive;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"DinhGiaService.DinhGiaXNK_Gia|EXCEPTION| {ex.Message}");
        //    }
        //    return EPoint.Unknown;
        //}


        private (EPoint, string) XNK(EStockType eType, double step1, double step2)
        {
            try
            {
                if(eType == EStockType.CaoSu)
                {
                    var eHaiQuan = EHaiQuan.CaoSu;
                    var eThongKe = EKeyTongCucThongKe.XK_CaoSu;
                    return ModeXNK(eThongKe, eHaiQuan, step1, step2);
                }
                else if (eType == EStockType.DetMay)
                {
                    var eHaiQuan = EHaiQuan.DetMay;
                    var eThongKe = EKeyTongCucThongKe.XK_DetMay;
                    return ModeXNK(eThongKe, eHaiQuan, step1, step2);
                }
                else if (eType == EStockType.Go)
                {
                    var eHaiQuan = EHaiQuan.Go;
                    var eThongKe = EKeyTongCucThongKe.XK_Go;
                    return ModeXNK(eThongKe, eHaiQuan, step1, step2);
                }
                else if (eType == EStockType.Nhua)
                {
                    var eHaiQuan = EHaiQuan.ChatDeo;
                    var eThongKe = EKeyTongCucThongKe.XK_ChatDeo;
                    var mode = ModeXNK(eThongKe, eHaiQuan, step1, step2);

                    var eHaiQuanSP = EHaiQuan.SPChatDeo;
                    var eThongKeSP = EKeyTongCucThongKe.XK_SPChatDeo;
                    var modeSP = ModeXNK(eThongKeSP, eHaiQuanSP, step1, step2);

                    var sBuilder = new StringBuilder();
                    sBuilder.AppendLine(mode.Item2);
                    sBuilder.AppendLine($"   - SP {modeSP.Item2}");
                    return (MergeEpoint(mode.Item1, modeSP.Item1), sBuilder.ToString());
                }
                else if (eType == EStockType.Oto)
                {
                    var eHaiQuan = EHaiQuan.Oto9Cho_NK;
                    var eThongKe = EKeyTongCucThongKe.None;
                    var mode = ModeXNK(eThongKe, eHaiQuan, step1, step2);
                }
                else if (eType == EStockType.PhanBon)
                {
                    var eHaiQuan = EHaiQuan.PhanBon;
                    var eThongKe = EKeyTongCucThongKe.None;
                    var mode = ModeXNK(eThongKe, eHaiQuan, step1, step2);
                }
                else if (eType == EStockType.Than)
                {
                    var eHaiQuan = EHaiQuan.Than;
                    var eThongKe = EKeyTongCucThongKe.None;
                    return ModeXNK(eThongKe, eHaiQuan, step1, step2);
                }
                else if (eType == EStockType.Thep)
                {
                    var eHaiQuan = EHaiQuan.SatThep;
                    var eThongKe = EKeyTongCucThongKe.XK_SatThep;
                    var mode = ModeXNK(eThongKe, eHaiQuan, step1, step2);

                    var eHaiQuanSP = EHaiQuan.SPSatThep;
                    var eThongKeSP = EKeyTongCucThongKe.XK_SPSatThep;
                    var modeSP = ModeXNK(eThongKeSP, eHaiQuanSP, step1, step2);

                    var eHaiQuanNK = EHaiQuan.SPSatThep_NK;
                    var eThongKeNK = EKeyTongCucThongKe.NK_SPSatThep;
                    var modeNK = ModeXNK(eThongKeNK, eHaiQuanNK, step1, step2);

                    var sBuilder = new StringBuilder();
                    sBuilder.AppendLine(mode.Item2);
                    sBuilder.AppendLine($"   - SP {modeSP.Item2}");
                    sBuilder.AppendLine($"   - NK {modeNK.Item2}");

                    var mergeXK = MergeEpoint(mode.Item1, modeSP.Item1);
                    return (MergeEpoint(mergeXK, Swap(modeNK.Item1)), sBuilder.ToString());

                }
                else if (eType == EStockType.ThuySan)
                {
                    var eHaiQuan = EHaiQuan.ThuySan;
                    var eThongKe = EKeyTongCucThongKe.XK_ThuySan;
                    return ModeXNK(eThongKe, eHaiQuan, step1, step2);
                }
                else if (eType == EStockType.XiMang)
                {
                    var eHaiQuan = EHaiQuan.Ximang;
                    var eThongKe = EKeyTongCucThongKe.XK_Ximang;
                    return ModeXNK(eThongKe, eHaiQuan, step1, step2);
                }
                else if (eType == EStockType.HoaChat)
                {
                    var eHaiQuan = EHaiQuan.HoaChat;
                    var eThongKe = EKeyTongCucThongKe.XK_HoaChat;
                    var mode = ModeXNK(eThongKe, eHaiQuan, step1, step2);

                    var eHaiQuanSP = EHaiQuan.SPHoaChat;
                    var eThongKeSP = EKeyTongCucThongKe.XK_SPHoaChat;
                    var modeSP = ModeXNK(eThongKeSP, eHaiQuanSP, step1, step2);
                    
                    var sBuilder = new StringBuilder();
                    sBuilder.AppendLine(mode.Item2);
                    sBuilder.AppendLine($"   - SP {modeSP.Item2}");
                    return (MergeEpoint(mode.Item1, modeSP.Item1), sBuilder.ToString());

                }
                else if (eType == EStockType.CaPhe)
                {
                    var eHaiQuan = EHaiQuan.None;
                    var eThongKe = EKeyTongCucThongKe.XK_CaPhe;
                    return ModeXNK(eThongKe, eHaiQuan, step1, step2);
                }
                else if (eType == EStockType.Gao)
                {
                    var eHaiQuan = EHaiQuan.Gao;
                    var eThongKe = EKeyTongCucThongKe.XK_Gao;
                    return ModeXNK(eThongKe, eHaiQuan, step1, step2);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.HaiQuanXNK|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
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

        private EPoint MergeEpoint(EPoint e1, EPoint e2)
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

        private (EPoint, string) ModeXNK(EKeyTongCucThongKe eThongKe, EHaiQuan eHaiQuan, double step1, double step2)
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
                    return HaiQuanXNK(eHaiQuan, step1, step2);
                }
                else if(mode == 2)
                {
                    return ThongKeXNK(eThongKe, step1, step2);
                }
                else
                {
                    return ThongKeQuyXNK(eThongKe, step1, step2);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.ModeXNK|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }

        private (EPoint, string) HaiQuanXNK(EHaiQuan eHaiQuan, double step1, double step2)
        {
            try
            {
                var lXK = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)eHaiQuan)).OrderByDescending(x => x.d);
                if (lXK is null || !lXK.Any())
                {
                    return (EPoint.Unknown, string.Empty);
                }
                var cur = lXK.FirstOrDefault();
                var prev = lXK.FirstOrDefault(x => x.d == cur.d - 1000);
                if (prev is null)
                {
                    return (EPoint.Unknown, string.Empty);
                }

                var rate = Math.Round(Math.Round(cur.va * 100 / prev.va, 1) - 100, 1);
                if (rate >= 20)
                {
                    return (EPoint.VeryPositive, $"   - qoq( Hải Quan ): {rate}%");
                }
                else if (rate >= 10)
                {
                    return (EPoint.Positive, $"   - qoq( Hải Quan ): {rate}%");
                }
                else if (rate <= -5)
                {
                    return (EPoint.Negative, $"   - qoq( Hải Quan ): {rate}%");
                }
                else if (rate <= -20)
                {
                    return (EPoint.VeryNegative, $"   - qoq( Hải Quan ): {rate}%");
                }
                return (EPoint.Normal, $"   - qoq( Hải Quan ): {rate}%");
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.HaiQuanXNK|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty); 
        }

        private (EPoint, string) ThongKeXNK(EKeyTongCucThongKe eThongKe, double step1, double step2)
        {
            try
            {
                var lThongKe = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)eThongKe)).OrderByDescending(x => x.d);
                if(lThongKe is null || !lThongKe.Any())
                {
                    return (EPoint.Unknown, string.Empty);
                }    

                var cur = lThongKe.FirstOrDefault();
                var rate = Math.Round(cur.qoq - 100, 1);
                if(rate >= 20)
                {
                    return (EPoint.VeryPositive, $"   - qoq( Tháng ): {rate}%");
                }
                else if(rate >= 10)
                {
                    return (EPoint.Positive, $"   - qoq( Tháng ): {rate}%");
                }
                else if (rate <= -5)
                {
                    return (EPoint.Negative, $"   - qoq( Tháng ): {rate}%");
                }
                else if (rate <= -20)
                {
                    return (EPoint.VeryNegative, $"   - qoq( Tháng ): {rate}%");
                }
                return (EPoint.Normal, $"   - qoq( Tháng ): {rate}%");
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.ThongKeXNK|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }

        private (EPoint, string) ThongKeQuyXNK(EKeyTongCucThongKe eThongKe, double step1, double step2)
        {
            try
            {
                var lThongKe = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)eThongKe)).OrderByDescending(x => x.d);
                if (!lThongKe.Any())
                {
                    return (EPoint.Unknown, string.Empty);
                }

                var cur = lThongKe.FirstOrDefault();
                var rate = Math.Round(cur.qoq - 100, 1);
                if (rate >= 20)
                {
                    return (EPoint.VeryPositive, $"   - qoq( Quý ): {rate}%");
                }
                else if (rate >= 10)
                {
                    return (EPoint.Positive, $"   - qoq( Quý ): {rate}%");
                }
                else if (rate <= -5)
                {
                    return (EPoint.Negative, $"   - qoq( Quý ): {rate}%");
                }
                else if (rate <= -20)
                {
                    return (EPoint.VeryNegative, $"   - qoq( Quý ): {rate}%");
                }
                return (EPoint.Normal, $"   - qoq( Quý ): {rate}%");
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.ThongKeQuyXNK|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }

        private (EPoint, string) DinhGiaPE(string code, List<Quote> lQuote)
        {
            try
            {
                var dt = DateTime.Now;
                if (lQuote is null || !lQuote.Any())
                {
                    return (EPoint.VeryNegative, string.Empty);
                }
                //pe
                var lpe = _peRepo.GetByFilter(Builders<ChiSoPE>.Filter.Eq(x => x.s, code));
                if (lpe is null || !lpe.Any())
                {
                    return (EPoint.Negative, string.Empty);
                }

                var quote = lQuote.MaxBy(x => x.Date);//Giá mới nhất
                var lKehoach = _kehoachRepo.GetByFilter(Builders<KeHoach>.Filter.Eq(x => x.s, code));
                var curPlan = lKehoach?.FirstOrDefault(x => x.d == dt.Year);
                if (lKehoach is null || !lKehoach.Any() || curPlan is null)
                {
                    return DinhGiaPEkoKeHoach(lpe, quote, dt);
                }

                double pf_truth = 0;
                var avgRate = lKehoach.Where(x => x.pf_real > 0).Average(x => x.pf_real_r);//Tỉ lệ hoàn thành lợi nhuận trung bình
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

                if (pf_truth < 0)
                {
                    return (EPoint.VeryNegative, string.Empty);
                }

                if (pf_truth == 0)
                {
                    return (EPoint.Negative, string.Empty);
                }

                //True Path 
                var share = _shareRepo.GetEntityByFilter(Builders<Share>.Filter.Eq(x => x.s, code));
                if (share is null || share.share <= 0)
                {
                    return (EPoint.VeryNegative, string.Empty);
                }

                var eps_truth = Math.Round(pf_truth * 1000000000 / share.share, 1);
                if (eps_truth == 0)
                {
                    return (EPoint.VeryNegative, string.Empty);
                }
                
                var pe_truth = Math.Round((double)quote.Close * 1000 / eps_truth, 1);
                if (pe_truth <= 0)
                {
                    return (EPoint.VeryNegative, string.Empty);
                }
                var sBuilder = new StringBuilder();
                sBuilder.AppendLine($"   - PE dự phóng: {Math.Round(pe_truth, 1)}");

                var lastPE = lpe.MaxBy(x => x.d);
                if (lastPE.eps <= 0)
                {
                    return (EPoint.VeryNegative, sBuilder.ToString());
                }

                var pe_cur = Math.Round((double)quote.Close * 1000 / lastPE.eps, 1);
                var pe_avg = lpe.Where(x => x.d.ToString().EndsWith(dt.GetQuarter().ToString())).Average(x => x.pe);
                sBuilder.AppendLine($"   - PE trung bình: {Math.Round(pe_avg, 1)}");
                sBuilder.AppendLine($"   - PE hiện tại: {Math.Round(pe_cur, 1)}");
                if (pe_cur >= pe_avg
                    || pe_cur >= pe_truth)
                {
                    return (EPoint.Normal, sBuilder.ToString());
                }

                if (pe_cur * 1.05 < pe_truth)
                {
                    return (EPoint.VeryPositive, sBuilder.ToString());
                }

                return (EPoint.Positive, sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DinhGiaPE|EXCEPTION| {ex.Message}");
            }
            return (EPoint.VeryNegative, string.Empty);
        }

        private (EPoint, string) DinhGiaPEkoKeHoach(List<ChiSoPE> lpe, Quote quote, DateTime dt)
        {
            var pe_avg = lpe.Where(x => x.d.ToString().EndsWith(dt.GetQuarter().ToString())).Average(x => x.pe);
            if (pe_avg <= 0)
            {
                return (EPoint.VeryNegative, string.Empty);
            }
            var sBuilder = new StringBuilder();
            sBuilder.AppendLine($"   - PE trung bình: {Math.Round(pe_avg, 1)}");


            var lastPE = lpe.MaxBy(x => x.d);
            if (lastPE.eps <= 0)
            {
                return (EPoint.VeryNegative, sBuilder.ToString());
            }

            var pe_cur = Math.Round((double)quote.Close * 1000 / lastPE.eps, 1);
            sBuilder.AppendLine($"   - PE hiện tại: {Math.Round(pe_cur, 1)}");
            if (pe_cur >= pe_avg)
            {
                return (EPoint.Normal, sBuilder.ToString());
            }

            if (pe_cur * 1.05 < pe_avg)
            {
                return (EPoint.VeryPositive, sBuilder.ToString());
            }

            return (EPoint.Positive, sBuilder.ToString());
        }
    }
}
