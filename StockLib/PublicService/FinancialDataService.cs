using StockLib.Service;

namespace StockLib.PublicService
{
    public interface IFinancialDataService
    {
        Task SyncTest();
        Task SyncBCTC_BatDongSan();
        Task SyncBCTC_NganHang();
        Task SyncBCTC_ChungKhoan();
        Task SyncBCTC_Thep();
        Task SyncBCTC_BanLe();
        Task SyncBCTC_Dien();
        Task SyncBCTC_CangBien();
        Task SyncBCTC_CaoSu();
        Task SyncBCTC_DetMay();
        Task SyncBCTC_Go();
        Task SyncBCTC_HangKhong();
        Task SyncBCTC_Logistic();
        Task SyncBCTC_Nhua();
        Task SyncBCTC_Oto();
        Task SyncBCTC_PhanBon();
        Task SyncBCTC_Than();
        Task SyncBCTC_ThuySan();
        Task SyncBCTC_Ximang();
        Task SyncBCTC_DauKhi();

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

        public async Task SyncBCTC_CangBien()
        {
            await _bllService.SyncBCTC_CangBien();
        }

        public async Task SyncBCTC_CaoSu()
        {
            await _bllService.SyncBCTC_CaoSu();
        }

        public async Task SyncBCTC_DetMay()
        {
            await _bllService.SyncBCTC_DetMay();
        }

        public async Task SyncBCTC_Go()
        {
            await _bllService.SyncBCTC_Go();
        }

        public async Task SyncBCTC_HangKhong()
        {
            await _bllService.SyncBCTC_HangKhong();
        }

        public async Task SyncBCTC_Logistic()
        {
            await _bllService.SyncBCTC_Logistic();
        }

        public async Task SyncBCTC_Nhua()
        {
            await _bllService.SyncBCTC_Nhua();
        }

        public async Task SyncBCTC_Oto()
        {
            await _bllService.SyncBCTC_Oto();
        }

        public async Task SyncBCTC_PhanBon()
        {
            await _bllService.SyncBCTC_PhanBon();
        }

        public async Task SyncBCTC_Than()
        {
            await _bllService.SyncBCTC_Than();
        }

        public async Task SyncBCTC_ThuySan()
        {
            await _bllService.SyncBCTC_ThuySan();
        }

        public async Task SyncBCTC_Ximang()
        {
            await _bllService.SyncBCTC_Ximang();
        }

        public async Task SyncBCTC_DauKhi()
        {
            await _bllService.SyncBCTC_DauKhi();
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
