using StockLib.Service;

namespace StockLib.PublicService
{
    public interface IFinancialDataService
    {
        Task SyncTest();
        Task SyncBCTC_BatDongSan();
        Task SyncBCTC_NganHang();
        Task SyncBCTC_ChungKhoan();
        Task SyncBCTC();
        Task SyncPE();
        Task SyncKeHoach();
        Task SyncShare();
    }
    public class FinancialDataService : IFinancialDataService
    {
        private readonly IBllService _bllService;
        public FinancialDataService(IBllService bllService)
        {
            _bllService = bllService;
        }

        public async Task SyncTest()
        {
            await _bllService.SyncTest();
        }

        public async Task SyncBCTC_BatDongSan()
        {
            await _bllService.SyncBCTC_BatDongSan();
        }

        public async Task SyncBCTC_NganHang()
        {
            await _bllService.SyncBCTC_NganHang();
        }

        public async Task SyncBCTC_ChungKhoan() 
        {
            await _bllService.SyncBCTC_ChungKhoan();
        }

        public async Task SyncBCTC()
        {
            await _bllService.SyncBCTC();
        }

        public async Task SyncPE()
        {
            await _bllService.SyncPE();
        }

        public async Task SyncKeHoach()
        {
            await _bllService.SyncKeHoach();
        }

        public async Task SyncShare()
        {
            await _bllService.SyncShare();
        }
    }
}
