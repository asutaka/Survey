using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Skender.Stock.Indicators;
using StockLib.DAL;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;
using static iTextSharp.text.pdf.AcroFields;

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
        private async Task<(EPoint, EStockType)> DinhGiaNganh(string code, int nganh)
        {
            var eNganh = (EStockType)nganh;
            if(eNganh == EStockType.BanLe)
            {
                return (await DG_BanLe(code), eNganh);
            }

            if (eNganh == EStockType.BDS)
            {
                return (await DG_BDS(code), eNganh);
            }

            if (eNganh == EStockType.CangBien)
            {
                return (await DG_CangBien(code), eNganh);
            }

            if (eNganh == EStockType.CaoSu)
            {
                return (await DG_CaoSu(code), eNganh);
            }

            if (eNganh == EStockType.ChungKhoan)
            {
                return (await DG_ChungKhoan(code), eNganh);
            }

            if (eNganh == EStockType.DauKhi)
            {
                return (await DG_DauKhi(code), eNganh);
            }

            if (eNganh == EStockType.DetMay)
            {
                return (await DG_DetMay(code), eNganh);
            }

            if (eNganh == EStockType.DienGio)
            {
                return (await DG_DienGio(code), eNganh);
            }

            if (eNganh == EStockType.DienKhi)
            {
                return (await DG_DienKhi(code), eNganh);
            }

            if (eNganh == EStockType.DienMatTroi)
            {
                return (await DG_DienMatTroi(code), eNganh);
            }

            if (eNganh == EStockType.DienThan)
            {
                return (await DG_DienThan(code), eNganh);
            }

            if (eNganh == EStockType.ThuyDien)
            {
                return (await DG_ThuyDien(code), eNganh);
            }

            if (eNganh == EStockType.Forex)
            {
                return (Swap(await DinhGia_Forex(EForex.DXU1, 2, 5)), eNganh);
            }

            if (eNganh == EStockType.Go)
            {
                return (await DG_Go(code), eNganh);
            }

            if (eNganh == EStockType.HangKhong)
            {
                return (await DG_HangKhong(code), eNganh);
            }

            if (eNganh == EStockType.KCN)
            {
                return (await DG_KCN(code), eNganh);
            }

            if (eNganh == EStockType.Logistic)
            {
                return (await DG_Logistic(code), eNganh);
            }

            if (eNganh == EStockType.NganHang)
            {
                return (await DG_NganHang(code), eNganh);
            }

            if (eNganh == EStockType.Nhua)
            {
                return (await DG_Nhua(code), eNganh);
            }

            if (eNganh == EStockType.Oto)
            {
                return (await DG_Oto(code), eNganh);
            }

            if (eNganh == EStockType.PhanBon)
            {
                return (await DG_PhanBon(code), eNganh);
            }

            if (eNganh == EStockType.Than)
            {
                return (await DG_Than(code), eNganh);
            }

            if (eNganh == EStockType.Thep)
            {
                return (await DG_Thep(code), eNganh);
            }

            if (eNganh == EStockType.ThuySan)
            {
                return (await DG_ThuySan(code), eNganh);
            }

            if (eNganh == EStockType.Vin)
            {
                return (await DG_Vin(code), eNganh);
            }

            if (eNganh == EStockType.XayDung)
            {
                return (await DG_XayDung(code), eNganh);
            }

            if (eNganh == EStockType.XiMang)
            {
                return (await DG_XiMang(code), eNganh);
            }

            if (eNganh == EStockType.VanTaiBien)
            {
                return (await DG_VanTaiBien(code), eNganh);
            }

            if (eNganh == EStockType.ChanNuoi)
            {
                return (await DG_ChanNuoi(code), eNganh);
            }

            if (eNganh == EStockType.NongNghiep)
            {
                return (await DG_NongNghiep(code), eNganh);
            }

            if (eNganh == EStockType.HoaChat)
            {
                return (await DG_HoaChat(code), eNganh);
            }

            if (eNganh == EStockType.CaPhe)
            {
                return (await DG_CaPhe(code), eNganh);
            }

            if (eNganh == EStockType.Gao)
            {
                return (await DG_Gao(code), eNganh);
            }

            if (eNganh == EStockType.Duoc)
            {
                return (await DG_Duoc(code), eNganh);
            }

            if (eNganh == EStockType.DichVuYTe)
            {
                return (await DG_DichVuYTe(code), eNganh);
            }

            if (eNganh == EStockType.BaoHiem)
            {
                return (await DG_BaoHiem(code), eNganh);
            }

            if (eNganh == EStockType.CNTT)
            {
                return (await DG_CNTT(code), eNganh);
            }

            if (eNganh == EStockType.DauTuCong)
            {
                return (await DG_DauTuCong(code), eNganh);
            }

            if (eNganh == EStockType.ThietBiDien)
            {
                return (await DG_ThietBiDien(code), eNganh);
            }

            if (eNganh == EStockType.Duong)
            {
                return (await DG_Duong(code), eNganh);
            }

            if (eNganh == EStockType.Bia)
            {
                return (await DG_Bia(code), eNganh);
            }

            if (eNganh == EStockType.SPNongNghiepKhac)
            {
                return (await DG_SPNongNghiepKhac(code), eNganh);
            }

            if (eNganh == EStockType.NuocNgot)
            {
                return (await DG_NuocNgot(code), eNganh);
            }

            if (eNganh == EStockType.Sua)
            {
                return (await DG_Sua(code), eNganh);
            }

            if (eNganh == EStockType.XuatKhau)
            {
                return (await DG_XuatKhau(code, eNganh), eNganh);
            }

            if (eNganh == EStockType.NangLuong)
            {
                return (await DG_NangLuong(code), eNganh);
            }

            if (eNganh == EStockType.Khac)
            {
                return (await DG_Khac(code), eNganh);
            }

            return (EPoint.Unknown, eNganh);
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
                    strRes.AppendLine($"+ {dg.Item2.GetDisplayName()}: {dg.Item1.GetDisplayName()}");
                    lDinhGia.Add((double)dg.Item1 * stock.ty1_r / 100);
                }
            }
            if (stock.ty2 > -1)
            {
                var dg = await DinhGiaNganh(stock.s, stock.ty2);
                if (dg.Item1 != EPoint.Unknown)
                {
                    strRes.AppendLine($"+ {dg.Item2.GetDisplayName()}: {dg.Item1.GetDisplayName()}");
                    lDinhGia.Add((double)dg.Item1 * stock.ty2_r / 100);
                }
            }
            if (stock.ty3 > -1)
            {
                var dg = await DinhGiaNganh(stock.s, stock.ty3);
                if (dg.Item1 != EPoint.Unknown)
                {
                    strRes.AppendLine($"+  {dg.Item2.GetDisplayName()}: {dg.Item1.GetDisplayName()}");
                    lDinhGia.Add((double)dg.Item1 * stock.ty3_r / 100);
                }
            }

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
        private async Task<EPoint> DinhGiaXNK(EHaiQuan haiquan, double step1, double step2)
        {
            try
            {
                var lXK = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)haiquan)).OrderByDescending(x => x.d);
                if (lXK is null || !lXK.Any())
                    return EPoint.VeryNegative;

                var cur = lXK.First();
                var near = lXK.Skip(1).FirstOrDefault();
                var prev = lXK.FirstOrDefault(x => x.d == cur.d - 1000);
                double va = 0;
                if (prev != null && prev.va > 0)
                {
                    var qoq_va = Math.Round(100 * (-1 + cur.va / prev.va), 1);
                    var total_va = 0;
                    if (qoq_va > step2)
                    {
                        total_va = (int)EPoint.VeryPositive;
                    }
                    else if (qoq_va <= step2 && qoq_va > step1)
                    {
                        total_va = (int)EPoint.Positive;
                    }
                    else if (qoq_va <= step1 && qoq_va >= -step1)
                    {
                        total_va = (int)EPoint.Normal;
                    }
                    else if (qoq_va < -step1 && qoq_va >= -step2)
                    {
                        total_va = (int)EPoint.Negative;
                    }
                    else
                    {
                        total_va = (int)EPoint.VeryNegative;
                    }
                    va = total_va;
                }
                if (near != null && near.va > 0)
                {
                    var qoqoy_va = Math.Round(100 * (-1 + cur.va / near.va), 1);
                    var total_va = 0;
                    if (qoqoy_va > step2)
                    {
                        total_va = (int)EPoint.VeryPositive;
                    }
                    else if (qoqoy_va <= step2 && qoqoy_va > step1)
                    {
                        total_va = (int)EPoint.Positive;
                    }
                    else if (qoqoy_va <= step1 && qoqoy_va >= -step1)
                    {
                        total_va = (int)EPoint.Normal;
                    }
                    else if (qoqoy_va < -step1 && qoqoy_va >= -step2)
                    {
                        total_va = (int)EPoint.Negative;
                    }
                    else
                    {
                        total_va = (int)EPoint.VeryNegative;
                    }
                    va = va * 0.6 + total_va * 0.4;
                }

                if (va < (int)EPoint.Negative)
                {
                    return EPoint.VeryNegative;
                }
                if (va < (int)EPoint.Normal)
                {
                    return EPoint.Negative;
                }
                if (va < (int)EPoint.Positive)
                {
                    return EPoint.Normal;
                }
                if (va < (int)EPoint.VeryPositive)
                {
                    return EPoint.Positive;
                }
                return EPoint.VeryPositive;
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DinhGiaXNK|EXCEPTION| {ex.Message}");
            }
            return EPoint.VeryNegative;
        }
        private async Task<EPoint> DinhGiaXNK_Gia(EHaiQuan haiquan, double step1, double step2)
        {
            try
            {
                var lXK = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)haiquan)).OrderByDescending(x => x.d);
                if (lXK is null || !lXK.Any())
                    return EPoint.VeryNegative;

                var cur = lXK.First();
                var near = lXK.Skip(1).FirstOrDefault();
                var prev = lXK.FirstOrDefault(x => x.d == cur.d - 1000);
                double va = 0, price = 0;
                if (prev != null && prev.price > 0)
                {
                    var qoq_price = Math.Round(100 * (-1 + cur.price / prev.price), 1);
                    var total_price = 0;
                    if (qoq_price > step2)
                    {
                        total_price = (int)EPoint.VeryPositive;
                    }
                    else if (qoq_price <= step2 && qoq_price > step1)
                    {
                        total_price = (int)EPoint.Positive;
                    }
                    else if (qoq_price <= step1 && qoq_price >= -step1)
                    {
                        total_price = (int)EPoint.Normal;
                    }
                    else if (qoq_price < -step1 && qoq_price >= -step2)
                    {
                        total_price = (int)EPoint.Negative;
                    }
                    else
                    {
                        total_price = (int)EPoint.VeryNegative;
                    }
                    price = total_price;
                }
                if (near != null && near.price > 0)
                {
                    var qoqoy_price = Math.Round(100 * (-1 + cur.price / near.price), 1);
                    var total_price = 0;
                    if (qoqoy_price > step2)
                    {
                        total_price = (int)EPoint.VeryPositive;
                    }
                    else if (qoqoy_price <= step2 && qoqoy_price > step1)
                    {
                        total_price = (int)EPoint.Positive;
                    }
                    else if (qoqoy_price <= step1 && qoqoy_price >= -step1)
                    {
                        total_price = (int)EPoint.Normal;
                    }
                    else if (qoqoy_price < -step1 && qoqoy_price >= -step2)
                    {
                        total_price = (int)EPoint.Negative;
                    }
                    else
                    {
                        total_price = (int)EPoint.VeryNegative;
                    }
                    price = price * 0.6 + total_price * 0.4;
                }
                if (price <= 0)
                    return EPoint.Unknown;
                if (price < (int)EPoint.Negative)
                {
                    return EPoint.VeryNegative;
                }
                if (price < (int)EPoint.Normal)
                {
                    return EPoint.Negative;
                }
                if (price < (int)EPoint.Positive)
                {
                    return EPoint.Normal;
                }
                if (price < (int)EPoint.VeryPositive)
                {
                    return EPoint.Positive;
                }
                return EPoint.VeryPositive;
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DinhGiaXNK_Gia|EXCEPTION| {ex.Message}");
            }
            return EPoint.Unknown;
        }


        private async Task<EPoint> XNK(EStockType eType, double step1, double step2)
        {
            try
            {
                if(eType == EStockType.CaoSu)
                {
                    var eHaiQuan = EHaiQuan.CaoSu;

                    var eThongKe = EKeyTongCucThongKe.XK_CaoSu;
                }
                else if (eType == EStockType.DetMay)
                {
                    var eHaiQuan = EHaiQuan.DetMay;

                    var eThongKe = EKeyTongCucThongKe.XK_DetMay;
                }
                else if (eType == EStockType.Go)
                {
                    var eHaiQuan = EHaiQuan.Go;
                    var eThongKe = EKeyTongCucThongKe.XK_Go;

                    var lHaiQuan = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)eHaiQuan));
                    var lThongKe = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)eThongKe));
                    var lThongKeQuy = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)eThongKe));

                    var mode = 0;
                    var haiquan = lHaiQuan.MaxBy(x => x.d);
                    var thongke = lThongKe.MaxBy(x => x.d);
                    var thongkequy = lThongKeQuy.MaxBy(x => x.d);
                    var quarter = int.Parse(thongkequy.d.ToString().Last().ToString());
                    var timeQuy = 0;
                    if(quarter == 1)
                    {
                        timeQuy = 3;
                    }
                    else if(quarter == 2)
                    {
                        timeQuy = 6;
                    }
                    else if(quarter == 3)
                    {
                        timeQuy = 9;
                    }
                    else
                    {
                        timeQuy = 12;
                    }
                    var timeMonth = thongke.d - (thongke.d / 100) * 100;
                    var timeHaiQuan = 
                }
                else if (eType == EStockType.Nhua)
                {
                    var eHaiQuan = EHaiQuan.ChatDeo;
                    var eHaiQuanSP = EHaiQuan.SPChatDeo;

                    var eThongKe1 = EKeyTongCucThongKe.XK_ChatDeo;
                    var eThongKe2 = EKeyTongCucThongKe.XK_SPChatDeo;
                }
                else if (eType == EStockType.Oto)
                {
                    var eHaiQuan = EHaiQuan.Oto9Cho_NK;
                    var eHaiQuanVT = EHaiQuan.OtoVanTai_NK;

                    var eThongKe = EKeyTongCucThongKe.NK_Oto;
                }
                else if (eType == EStockType.PhanBon)
                {
                    var eHaiQuan = EHaiQuan.PhanBon;

                    var eThongKe = EKeyTongCucThongKe.QUY_GiaXK_PhanBon;
                    var eThongKe1 = EKeyTongCucThongKe.NK_PhanBon;
                }
                else if (eType == EStockType.Than)
                {
                    var eHaiQuan = EHaiQuan.Than;

                    var eThongKe1 = EKeyTongCucThongKe.QUY_GiaXK_Than;
                }
                else if (eType == EStockType.Thep)
                {
                    var eHaiQuan = EHaiQuan.SatThep;
                    var eHaiQuanSP = EHaiQuan.SPSatThep;
                    var eHaiQuanNK = EHaiQuan.SPSatThep_NK;

                    var eThongKe = EKeyTongCucThongKe.NK_SatThep;
                    var eThongKe1 = EKeyTongCucThongKe.NK_SPSatThep;
                    var eThongKe5 = EKeyTongCucThongKe.XK_SatThep;
                    var eThongKe6 = EKeyTongCucThongKe.XK_SPSatThep;
                }
                else if (eType == EStockType.ThuySan)
                {
                    var eHaiQuan = EHaiQuan.ThuySan;

                    var eThongKe = EKeyTongCucThongKe.QUY_GiaXK_ThuySan;
                    var eThongKe2 = EKeyTongCucThongKe.XK_ThuySan;
                }
                else if (eType == EStockType.XiMang)
                {
                    var eHaiQuan = EHaiQuan.Ximang;

                    var eThongKe1 = EKeyTongCucThongKe.XK_Ximang;
                }
                else if (eType == EStockType.HoaChat)
                {
                    var eHaiQuan = EHaiQuan.HoaChat;
                    var eHaiQuanSP = EHaiQuan.SPHoaChat;

                    var eThongKe2 = EKeyTongCucThongKe.XK_HoaChat;
                    var eThongKe3 = EKeyTongCucThongKe.XK_SPHoaChat;
                }
                else if (eType == EStockType.CaPhe)
                {
                    var eThongKe1 = EKeyTongCucThongKe.XK_CaPhe;

                }
                else if (eType == EStockType.Gao)
                {
                    var eHaiQuan = EHaiQuan.Gao;

                    var eThongKe1 = EKeyTongCucThongKe.XK_Gao;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.HaiQuanXNK|EXCEPTION| {ex.Message}");
            }
            return EPoint.Unknown;
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

                var rate = Math.Round(cur.va * 100 / prev.va, 1) - 100;
                if (rate >= 20)
                {
                    return (EPoint.VeryPositive, $"QoQ: {rate}%");
                }
                else if (rate >= 10)
                {
                    return (EPoint.Positive, $"QoQ: {rate}%");
                }
                else if (rate <= -5)
                {
                    return (EPoint.Negative, $"QoQ: {rate}%");
                }
                else if (rate <= -20)
                {
                    return (EPoint.VeryNegative, $"QoQ: {rate}%");
                }
                return (EPoint.Normal, $"QoQ: {rate}%");
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
                var rate = 100 - cur.qoq;
                if(rate >= 20)
                {
                    return (EPoint.VeryPositive, $"QoQ: {rate}%");
                }
                else if(rate >= 10)
                {
                    return (EPoint.Positive, $"QoQ: {rate}%");
                }
                else if (rate <= -5)
                {
                    return (EPoint.Negative, $"QoQ: {rate}%");
                }
                else if (rate <= -20)
                {
                    return (EPoint.VeryNegative, $"QoQ: {rate}%");
                }
                return (EPoint.Normal, $"QoQ: {rate}%");
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
                var rate = 100 - cur.qoq;
                if (rate >= 20)
                {
                    return (EPoint.VeryPositive, $"QoQ: {rate}%");
                }
                else if (rate >= 10)
                {
                    return (EPoint.Positive, $"QoQ: {rate}%");
                }
                else if (rate <= -5)
                {
                    return (EPoint.Negative, $"QoQ: {rate}%");
                }
                else if (rate <= -20)
                {
                    return (EPoint.VeryNegative, $"QoQ: {rate}%");
                }
                return (EPoint.Normal, $"QoQ: {rate}%");
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
