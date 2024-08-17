using StockLib.Service;

namespace StockLib.PublicService
{
    public interface IFinancialDataService
    {
        Task SyncBCTC_BatDongSan();
        Task SyncBCTC_NganHang();
        Task SyncBCTC_ChungKhoan();
        Task SyncBCTC_Thep();
        Task SyncBCTC_BanLe();
        Task SyncBCTC_Dien();
        Task SyncBCTC_Khac();
    }
    public class FinancialDataService : IFinancialDataService
    {
        private readonly IBllService _bllService;
        public FinancialDataService(IBllService bllService)
        {
            _bllService = bllService;
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

        public async Task SyncBCTC_Thep()
        {
            await _bllService.SyncBCTC_Thep();
        }

        public async Task SyncBCTC_BanLe()
        {
            await _bllService.SyncBCTC_BanLe();
        }

        public async Task SyncBCTC_Dien()
        {
            await _bllService.SyncBCTC_Dien();
        }

        public async Task SyncBCTC_Khac()
        {

        }
    }
}
