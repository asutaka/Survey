using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<List<Stream>> Chart_MaCK(string input)
        {
            var stock = StaticVal._lStock.FirstOrDefault(x => x.s == input);
            if (stock == null)
                return null;

            var isXayDung = stock.cat.Any(y => y.ty == (int)EStockType.XayDung);
            if (isXayDung) 
            {
                return await Chart_XayDung(input);
            }
            var isKCN = stock.cat.Any(y => y.ty == (int)EStockType.KCN);
            if (isKCN) 
            {
                return await Chart_KCN(input);
            }
            var isVin = StaticVal._lVin.Any(x => x == stock.s);
            if(isVin) 
            {
                return await Chart_BatDongSan(input);
            }
            var isBDS = stock.cat.Any(y => y.ty == (int)EStockType.BDS);
            if (isBDS) 
            {
                return await Chart_BatDongSan(input);
            }
            var isNganHang = stock.cat.Any(y => y.ty == (int)EStockType.NganHang);
            if (isNganHang)
            {
                return await Chart_NganHang(input);
            }
            var isChungKhoan = stock.cat.Any(y => y.ty == (int)EStockType.ChungKhoan);
            if (isChungKhoan)
            {
                return await Chart_ChungKhoan(input);
            }
            var isThep = stock.cat.Any(y => y.ty == (int)EStockType.Thep);
            if (isThep)
            {
                return await Chart_Thep(input);
            }
            var isBanLe = stock.cat.Any(y => y.ty == (int)EStockType.BanLe);
            if (isBanLe)
            {
                return await Chart_BanLe(input);
            }

            var isDien = stock.cat.Any(y => y.ty == (int)EStockType.DienGio
                                            || y.ty == (int)EStockType.DienKhi
                                            || y.ty == (int)EStockType.DienMatTroi
                                            || y.ty == (int)EStockType.DienThan
                                            || y.ty == (int)EStockType.ThuyDien
                                            || y.ty == (int)EStockType.NangLuong);
            if (isDien)
            {
                return await Chart_Dien(input);
            }

            var isCangBien = stock.cat.Any(y => y.ty == (int)EStockType.CangBien);
            if (isCangBien)
            {
                return await Chart_CangBien(input);
            }

            var isLogistic = stock.cat.Any(y => y.ty == (int)EStockType.Logistic || y.ty == (int)EStockType.VanTaiBien);
            if (isLogistic)
            {
                return await Chart_Logistic(input);
            }

            var isHangKhong = stock.cat.Any(y => y.ty == (int)EStockType.HangKhong);
            if (isHangKhong)
            {
                return await Chart_HangKhong(input);
            }

            var isCaoSu = stock.cat.Any(y => y.ty == (int)EStockType.CaoSu);
            if (isCaoSu)
            {
                return await Chart_CaoSu(input);
            }

            var isNhua = stock.cat.Any(y => y.ty == (int)EStockType.Nhua);
            if (isNhua)
            {
                return await Chart_Nhua(input);
            }

            var isOto = stock.cat.Any(y => y.ty == (int)EStockType.Oto || y.ty == (int)EStockType.OtoTai);
            if (isOto)
            {
                return await Chart_Oto(input);
            }

            var isPhanBon = stock.cat.Any(y => y.ty == (int)EStockType.PhanBon);
            if (isPhanBon)
            {
                return await Chart_PhanBon(input);
            }

            var isThan = stock.cat.Any(y => y.ty == (int)EStockType.Than);
            if (isThan)
            {
                return await Chart_Than(input);
            }

            var isThuySan = stock.cat.Any(y => y.ty == (int)EStockType.ThuySan);
            if (isThuySan)
            {
                return await Chart_ThuySan(input);
            }

            var isXimang = stock.cat.Any(y => y.ty == (int)EStockType.XiMang);
            if (isXimang)
            {
                return await Chart_Ximang(input);
            }

            var isDetmay = stock.cat.Any(y => y.ty == (int)EStockType.DetMay);
            if (isDetmay)
            {
                return await Chart_DetMay(input);
            }

            var isGo = stock.cat.Any(y => y.ty == (int)EStockType.Go);
            if (isGo)
            {
                return await Chart_Go(input);
            }

            var isDauKhi = stock.cat.Any(y => y.ty == (int)EStockType.DauKhi);
            if (isDauKhi)
            {
                return await Chart_DauKhi(input);
            }
            return null;
        }
    }
}
