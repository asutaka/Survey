using iTextSharp.text.pdf.qrcode;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        private async Task<Stream> Chart_XNK(Stock stock)
        {
            var eHaiquan = EHaiQuan.None;
            var eThongKe = EKeyTongCucThongKe.None;
            var strTitle = string.Empty;
            if(stock.cat.Any(x => x.ty == (int)EStockType.Thep))
            {
                eHaiquan = EHaiQuan.SatThep;
                eThongKe = EKeyTongCucThongKe.XK_SatThep;
                strTitle = "sắt thép";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.Than))
            {
                eHaiquan = EHaiQuan.Than;
                strTitle = "than";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.PhanBon))
            {
                eHaiquan = EHaiQuan.PhanBon;
                strTitle = "phân bón";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.HoaChat))
            {
                eHaiquan = EHaiQuan.HoaChat;
                eThongKe = EKeyTongCucThongKe.XK_HoaChat;
                strTitle = "hóa chất";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.Go))
            {
                eHaiquan = EHaiQuan.Go;
                eThongKe = EKeyTongCucThongKe.XK_Go;
                strTitle = "gỗ";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.Gao))
            {
                eHaiquan = EHaiQuan.Gao;
                eThongKe = EKeyTongCucThongKe.XK_Gao;
                strTitle = "gạo";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.XiMang))
            {
                eHaiquan = EHaiQuan.Ximang;
                eThongKe = EKeyTongCucThongKe.XK_Ximang;
                strTitle = "xi măng";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.CaPhe))
            {
                eThongKe = EKeyTongCucThongKe.XK_CaPhe;
                strTitle = "cà phê";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.CaoSu))
            {
                eHaiquan = EHaiQuan.CaoSu;
                eThongKe = EKeyTongCucThongKe.XK_CaoSu;
                strTitle = "cao su";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.Oto))
            {
                eHaiquan = EHaiQuan.Oto9Cho_NK;
                strTitle = "ô tô con";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.OtoTai))
            {
                eHaiquan = EHaiQuan.OtoVanTai_NK;
                strTitle = "ô tô tải";
            }

            var lHaiQuan = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)eHaiquan)).OrderBy(x => x.d);
            var lThongKe = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)eThongKe)).OrderBy(x => x.d);
            var lThongKeQuy = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)eThongKe)).OrderBy(x => x.d);

            var lastHaiQuan = lHaiQuan?.LastOrDefault() ?? new ThongKeHaiQuan();//2024081
            var lastThongKe = lThongKe?.LastOrDefault() ?? new ThongKe();//202301
            var lastThongKeQuy = lThongKeQuy?.LastOrDefault() ?? new ThongKeQuy();//20231

            var yearHaiQuan = lastHaiQuan.d / 1000;
            var yearThongKe = lastThongKe.d / 100;
            var yearThongKeQuy = lastThongKeQuy.d / 10;

            var monthHaiQuan = (lastHaiQuan.d - yearHaiQuan * 1000) / 10;
            var monthThongKe = lastThongKe.d - yearThongKe * 100;
            var monthThongKeQuy = lastThongKeQuy.d - yearThongKeQuy * 10;

            var dayHaiQuan = lastHaiQuan.d % 10 == 1 ? 15 : 26;
            var dayThongKe = 27;
            var dayThongKeQuy = 28;

            var dHaiQuan = new DateTime(yearHaiQuan < 1 ? 1 : yearHaiQuan, monthHaiQuan < 1 ? 1 : monthHaiQuan, dayHaiQuan < 1 ? 1 : dayHaiQuan);
            var dThongKe = new DateTime(yearThongKe < 1 ? 1 : yearThongKe, monthThongKe < 1 ? 1 : monthThongKe, dayThongKe < 1 ? 1 : dayThongKe);
            var dThongKeQuy = new DateTime(yearThongKeQuy < 1 ? 1 : yearThongKeQuy, monthThongKeQuy < 1 ? 1 : monthThongKeQuy, dayThongKeQuy < 1 ? 1 : dayThongKeQuy);

            var isNK = eHaiquan.ToString().Contains("NK") || eThongKe.ToString().Contains("NK");
            var lData = new List<(double, double, string)>();

            if(dHaiQuan > dThongKe && dHaiQuan > dThongKeQuy)
            {
                lData = lHaiQuan.Select(x => (x.va, x.price, x.d.GetNameHaiQuan())).ToList();
            }
            else if(dThongKe >  dHaiQuan && dThongKe > dThongKeQuy)
            {
                lData = lThongKe.Select(x => (x.va, x.price, x.d.GetNameMonth())).ToList();
            }
            else
            {
                lData = lThongKeQuy.Select(x => (x.va, x.price, x.d.GetNameQuarter())).ToList();
            }
            return await Chart_XNK(lData, !isNK, strTitle, string.Empty, string.Empty);
        }

        public async Task<List<Stream>> Chart_MaCK(string input)
        {
            try
            {
                var stock = StaticVal._lStock.FirstOrDefault(x => x.s == input);
                if (stock == null)
                    return null;

                var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, input));
                if (!lFinancial.Any())
                    return null;

                var lStream = new List<Stream>();
                var streamDoanhThu = await Chart_DoanhThu_LoiNhuan(lFinancial.Select(x => new BaseFinancialDTO { d = x.d, rv = x.rv, pf = x.pf }).ToList(), input);
                lStream.Add(streamDoanhThu);

                var streamNoTaiChinh = await Chart_NoTaiChinh(lFinancial, input);
                lStream.Add(streamNoTaiChinh);

                if (stock.IsTonKho())
                {
                    var stream = await Chart_TonKho(lFinancial, input);
                    lStream.Add(stream);
                }

                if (stock.IsFDI())
                {
                    var stream = await Chart_FDI();
                    lStream.Add(stream);
                }

                if (stock.IsNguoiMua())
                {
                    var stream = await Chart_NguoiMua(lFinancial, input);
                    lStream.Add(stream);
                }

                if (stock.IsXNK())
                {
                    var stream = await Chart_XNK(stock);
                    lStream.Add(stream);
                }

                if(stock.IsBanLe())
                {
                    var stream = await Chart_ThongKe_BanLe();
                    lStream.Add(stream);
                }


                //var isCangBien = stock.cat.First().ty == (int)EStockType.CangBien;
                //if (isCangBien)
                //{
                //    return await Chart_CangBien(input);
                //}

                //var isLogistic = stock.cat.First().ty == (int)EStockType.Logistic 
                //    || stock.cat.First().ty == (int)EStockType.VanTaiBien;
                //if (isLogistic)
                //{
                //    return await Chart_Logistic(input);
                //}

                //var isHangKhong = stock.cat.First().ty == (int)EStockType.HangKhong;
                //if (isHangKhong)
                //{
                //    return await Chart_HangKhong(input);
                //}

                //var isCaoSu = stock.cat.First().ty == (int)EStockType.CaoSu;
                //if (isCaoSu)
                //{
                //    return await Chart_CaoSu(input);
                //}

                //var isNhua = stock.cat.First().ty == (int)EStockType.Nhua;
                //if (isNhua)
                //{
                //    return await Chart_Nhua(input);
                //}

                //var isOto = stock.cat.First().ty == (int)EStockType.Oto 
                //    || stock.cat.First().ty == (int)EStockType.OtoTai;
                //if (isOto)
                //{
                //    return await Chart_Oto(input);
                //}

                //var isPhanBon = stock.cat.First().ty == (int)EStockType.PhanBon;
                //if (isPhanBon)
                //{
                //    return await Chart_PhanBon(input);
                //}

                //var isThan = stock.cat.First().ty == (int)EStockType.Than;
                //if (isThan)
                //{
                //    return await Chart_Than(input);
                //}

                //var isThuySan = stock.cat.First().ty == (int)EStockType.ThuySan;
                //if (isThuySan)
                //{
                //    return await Chart_ThuySan(input);
                //}

                //var isXimang = stock.cat.First().ty == (int)EStockType.XiMang;
                //if (isXimang)
                //{
                //    return await Chart_Ximang(input);
                //}

                //var isDetmay = stock.cat.First().ty == (int)EStockType.DetMay;
                //if (isDetmay)
                //{
                //    return await Chart_DetMay(input);
                //}

                //var isGo = stock.cat.First().ty == (int)EStockType.Go;
                //if (isGo)
                //{
                //    return await Chart_Go(input);
                //}

                //var isDauKhi = stock.cat.First().ty == (int)EStockType.DauKhi;
                //if (isDauKhi)
                //{
                //    return await Chart_DauKhi(input);
                //}

                //var isNganHang = stock.cat.First().ty == (int)EStockType.NganHang;
                //if (isNganHang)
                //{
                //    return await Chart_NganHang(input);
                //}
                //var isChungKhoan = stock.cat.First().ty == (int)EStockType.ChungKhoan;
                //if (isChungKhoan)
                //{
                //    return await Chart_ChungKhoan(input);
                //}
                return lStream;
            }
            catch(Exception ex)
            {
                _logger.LogError($"BllService.Chart_MaCK|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}
