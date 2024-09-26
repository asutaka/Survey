using Microsoft.Extensions.Logging;
using StockLib.PublicService;
using StockLib.Service;
using StockLib.Utils;

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
            //await _service.SurveyGoldFish("DC4");
            //await _service.SurveySuperTrend("VNINDEX");
            //await _service.SurveyVCP("NTL");
            //await _service.SurveyW("DPG");
            await _service.SurveyDanZagerCustom("DPG");
            //await _service.Survey3C("DPG");

            ////#region Test All Ma Chung Khoan
            //foreach (var stock in StaticVal._lStock.Where(x => x.rank == 1 || x.rank == 2))
            //{
            //    await _service.SurveyVCP(stock.s);
            //    Thread.Sleep(1000);
            //}

            //_service.RankChungKhoan();
            ////#endregion
        }
    }
}
