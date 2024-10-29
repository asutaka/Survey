using Microsoft.Extensions.Logging;
using StockLib.DAL.Entity;
using StockLib.PublicService;
using StockLib.Service;
using StockLib.Utils;
using System.Data;

namespace StockTestCase
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly IStockTestCaseService _service;
        private readonly IBllService _bllService;

        public App(ILogger<App> logger,
                        IStockTestCaseService service,
                        IBllService bllService)
        {
            _logger = logger;
            _service = service;
            _bllService = bllService;
        }

        public async Task Run(string[] args)
        {
            //await _service.SurveyCoinSuperTrendPhrase2("BTCUSDT");

            //await _service.SurveyGoldFish("DC4");
            //await _service.SurveySuperTrend("L18");
            //await _service.SurveySuperTrendPhrase2("L18");
            //await _service.SurveyT3("L18");
            //await _service.SurveyVCP("NTL");
            //await _service.SurveyMa20("TCH");
            //await _service.SurveyDanZagerCustom("TCH");
            //await _service.Survey3C("DPG");

            //////#region Test All Ma Chung Khoan
            //foreach (var stock in StaticVal._lStock.Where(x => x.rank != 4))
            //{
            //    await _service.SurveySuperTrendPhrase2(stock.s);
            //    Thread.Sleep(1000);
            //}

            //_service.RankChungKhoan();
            //_service.TotalDays();
            //////#endregion
            ///

            ////#region Test All Coin
            await _service.SurveyCoinEliot("");
            //var lSymbol = await StaticVal.ByBitInstance().SpotApiV3.ExchangeData.GetSymbolsAsync();


            //foreach (var coin in lSymbol.Data)
            //{
            //    if (coin.QuoteAsset != "USDT")
            //        continue;

            //    //if (coin.BaseAsset != "BTC" && coin.BaseAsset != "ETH" && coin.BaseAsset != "BNB")
            //    //    continue;

            //    if (coin.BaseAsset != "DOGE")
            //        continue;

            //    await _service.SurveyCoinEliot(coin.Alias);
            //    Thread.Sleep(1000);
            //}
            //_service.PrintAll();

            //_service.RankChungKhoan(EIndicator.DanZangerVolumne, 7);
            //_service.TotalDays();
            ////#endregion

            //var lCoin = await GlobalVal.BinanceInstance().SpotApi.ExchangeData.GetProductsAsync();
            //var lData = lCoin.Data.Where(x => x.QuoteAsset == "USDT");

        }
    }
}
