using StockLib.Service;

namespace StockLib.PublicService
{
    public interface IFinancialDataService
    {
        Task SyncBCTC_BatDongSan();
        Task SyncBCTC_NganHang();
        Task SyncBCTC_ChungKhoan();
        Task SyncBCTC_Thep();
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

        }

        public async Task SyncBCTC_ChungKhoan() 
        { 
        
        }

        public async Task SyncBCTC_Thep()
        {

        }

        public async Task SyncBCTC_Khac()
        {

        }
    }
}
