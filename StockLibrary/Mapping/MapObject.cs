using StockLibrary.Model;
using StockLibrary.Model.APIModel;
using StockLibrary.Util;
using System;
using System.Collections.Generic;

namespace StockLibrary.Mapping
{
    public static class MapObject
    {
        public static Foreign ToForeign(this ForeignDataModel item)
        {
            return new Foreign
            {
                d = new DateTimeOffset(item.tradingDate.ToDateTime("dd/MM/yyyy HH:mm:ss"), TimeSpan.FromHours(7)).ToUnixTimeSeconds(),
                pc = item.priceChange,
                ppc = item.perPriceChange,
                cp = item.ceilingPrice,
                fp = item.floorPrice,
                rp = item.refPrice,
                o = item.openPrice,
                h = item.highestPrice,
                l = item.lowestPrice,
                c = item.closePrice,
                ap = item.averagePrice,
                cpa = item.closePriceAdjusted,
                tmvo = item.totalMatchVol,
                tmva = item.totalMatchVal,
                tdva = item.totalDealVal,
                tdvo = item.totalDealVol,
                fbvot = item.foreignBuyVolTotal,
                fcr = item.foreignCurrentRoom,
                fsvot = item.foreignSellVolTotal,
                fbvat = item.foreignBuyValTotal,
                fsvat = item.foreignSellValTotal,
                tbt = item.totalBuyTrade,
                tbtvo = item.totalBuyTradeVol,
                tst = item.totalSellTrade,
                tstvo = item.totalSellTradeVol,
                nbsvo = item.netBuySellVol,
                nbsva = item.netBuySellVal,
                ex = item.exchange,
                s = item.symbol,
                fbvom = item.foreignBuyVolMatched,
                fbvod = item.foreignBuyVolDeal,
                t = DateTimeOffset.Now.ToUnixTimeSeconds()
            };
        }
        public static List<Foreign> ToForeign(this ForeignModel model)
        {
            if(model is null
                || model.data is null)
                return null;

            var lForeign = new List<Foreign>();
            foreach (var item in model.data)
            {
                lForeign.Add(new Foreign
                {
                    d = new DateTimeOffset(item.tradingDate.ToDateTime("dd/MM/yyyy HH:mm:ss"), TimeSpan.FromHours(7)).ToUnixTimeSeconds(),
                    pc = item.priceChange,
                    ppc = item.perPriceChange,
                    cp = item.ceilingPrice,
                    fp = item.floorPrice,
                    rp = item.refPrice,
                    o = item.openPrice,
                    h = item.highestPrice,
                    l = item.lowestPrice,
                    c = item.closePrice,
                    ap = item.averagePrice,
                    cpa = item.closePriceAdjusted,
                    tmvo = item.totalMatchVol,
                    tmva = item.totalMatchVal,
                    tdva = item.totalDealVal,
                    tdvo = item.totalDealVol,
                    fbvot = item.foreignBuyVolTotal,
                    fcr = item.foreignCurrentRoom,
                    fsvot = item.foreignSellVolTotal,
                    fbvat = item.foreignBuyValTotal,
                    fsvat = item.foreignSellValTotal,
                    tbt = item.totalBuyTrade,
                    tbtvo = item.totalBuyTradeVol,
                    tst = item.totalSellTrade,
                    tstvo = item.totalSellTradeVol,
                    nbsvo = item.netBuySellVol,
                    nbsva = item.netBuySellVal,
                    ex = item.exchange,
                    s = item.symbol,
                    fbvom = item.foreignBuyVolMatched,
                    fbvod = item.foreignBuyVolDeal,
                    t = DateTimeOffset.Now.ToUnixTimeSeconds()
                });
            }

            return lForeign;
        }
    }
}
