using StockLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if(isVin) { }
            var isBDS = !isXayDung && !isKCN && !isVin && stock.h24.Any(y => y.code == "2357" || y.code == "8600");
            if(isBDS) 
            {
                return await Chart_BatDongSan(input);
            }
            return null;
        }
    }
}
