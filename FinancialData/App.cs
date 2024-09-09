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
            //await _service.SyncBCTC_CangBien();
            //await _service.SyncBCTC_CaoSu();
            //await _service.SyncBCTC_DetMay();
            //await _service.SyncBCTC_Go();
            //await _service.SyncBCTC_HangKhong();
            //await _service.SyncBCTC_Logistic();
            //await _service.SyncBCTC_Nhua();
            //await _service.SyncBCTC_Oto();
            //await _service.SyncBCTC_PhanBon();
            //await _service.SyncBCTC_Than();
            //await _service.SyncBCTC_ThuySan();
            //await _service.SyncBCTC_Ximang();
            //await _service.SyncBCTC_DauKhi();

            await _service.SyncPE();
            //await _service.SyncKeHoach();
        }
    }
}