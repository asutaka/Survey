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

            var isXayDung = StaticVal._lXayDung.Any(x => x == stock.s);
            if (isXayDung) 
            {
                return await Chart_XayDung(input);
            }
            var isKCN = StaticVal._lKCN.Any(x => x == stock.s);
            if(isKCN) 
            {
                return await Chart_KCN(input);
            }
            var isVin = StaticVal._lVin.Any(x => x == stock.s);
            if(isVin) 
            {
                return await Chart_BatDongSan(input);
            }
            var isBDS = !isXayDung && !isKCN && !isVin && stock.h24.Any(y => y.code == "2357" || y.code == "8600");
            if(isBDS) 
            {
                return await Chart_BatDongSan(input);
            }
            var isNganHang = stock.h24.Any(y => y.code == "8300");
            if (isNganHang)
            {
                return await Chart_NganHang(input);
            }
            var isChungKhoan = stock.h24.Any(y => y.code == "8777");
            if (isChungKhoan)
            {
                return await Chart_ChungKhoan(input);
            }
            var isThep = stock.h24.Any(y => y.code == "1757");
            if (isThep)
            {
                return await Chart_Thep(input);
            }
            var isBanLe = stock.h24.Any(y => y.code == "5379"
                                        || y.code == "3530"
                                        || y.code == "3577");
            if (isBanLe)
            {
                return await Chart_BanLe(input);
            }

            var isDien = stock.h24.Any(y => y.code == "7535");
            if (isDien)
            {
                return await Chart_Dien(input);
            }

            var isCangBien = stock.h24.Any(y => y.code == "2777");
            if (isCangBien)
            {
                return await Chart_CangBien(input);
            }

            var isLogistic = stock.h24.Any(y => y.code == "2773" || y.code == "2779");
            if (isLogistic)
            {
                return await Chart_Logistic(input);
            }

            var isHangKhong = stock.h24.Any(y => y.code == "5751");
            if (isHangKhong)
            {
                return await Chart_HangKhong(input);
            }

            var isCaoSu = StaticVal._lCaoSu.Any(x => x == stock.s);
            if (isCaoSu)
            {
                return await Chart_CaoSu(input);
            }

            var isNhua = StaticVal._lNhua.Any(x => x == stock.s);
            if (isNhua)
            {
                return await Chart_Nhua(input);
            }

            var isOto = stock.h24.Any(y => y.code == "3353");
            if (isOto)
            {
                return await Chart_Oto(input);
            }

            var isPhanBon = StaticVal._lPhanBon.Any(x => x == stock.s);
            if (isPhanBon)
            {
                return await Chart_PhanBon(input);
            }

            var isThan = stock.h24.Any(y => y.code == "1771");
            if (isThan)
            {
                return await Chart_Than(input);
            }

            var isThuySan = StaticVal._lThuySan.Any(x => x == stock.s);
            if (isThuySan)
            {
                return await Chart_ThuySan(input);
            }

            var isXimang = StaticVal._lXimang.Any(x => x == stock.s);
            if (isXimang)
            {
                return await Chart_Ximang(input);
            }

            var isDetmay = stock.h24.Any(y => y.code == "3763");
            if (isDetmay)
            {
                return await Chart_DetMay(input);
            }

            var isGo = stock.h24.Any(y => y.code == "1733");
            if (isGo)
            {
                return await Chart_Go(input);
            }

            var isDauKhi = stock.h24.Any(y => y.code == "7573" || y.code == "0500");
            if (isDauKhi)
            {
                return await Chart_DauKhi(input);
            }
            return null;
        }
    }
}
