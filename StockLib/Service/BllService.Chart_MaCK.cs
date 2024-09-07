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
            if (isXayDung) { }
            var isKCN = StaticVal._lKCN.Any(x => x == stock.s);
            if(isKCN) { }
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

            //var isCangBien = StaticVal._lCangBien.Any(x => x == stock.s);
            //if (isCangBien)
            //{
            //    return await Chart_CangBien(input);
            //}

            //var isLogistic = StaticVal._lLogistic.Any(x => x == stock.s);
            //if (isLogistic)
            //{
            //    return await Chart_Logistic(input);
            //}

            var isHangKhong = stock.h24.Any(y => y.code == "5751");
            if (isHangKhong)
            {
                return await Chart_HangKhong(input);
            }
            return null;
        }
    }
}
