using Microsoft.Extensions.Logging;
using StockLib.PublicService;

namespace FinancialData
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly IFinancialDataService _service;

        public App(ILogger<App> logger,
                        IFinancialDataService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task Run(string[] args)
        {
            //await _service.SyncBCTC_BatDongSan();
            //await _service.SyncBCTC_NganHang();
            //await _service.SyncBCTC_ChungKhoan();
            //await _service.SyncBCTC_Thep();
            //await _service.SyncBCTC_BanLe();
            //await _service.SyncBCTC_Dien();
            //await _service.SyncBCTC_Khac();
        }
    }
}
