namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        public async Task SyncBCTC()
        {
            try
            {
                await _bllService.SyncBCTC_NganHang(true);
                await _bllService.SyncBCTC_BatDongSan(true);
                await _bllService.SyncBCTC_ChungKhoan(true);
                await _bllService.SyncBCTC_Thep(true);
                await _bllService.SyncBCTC_BanLe(true);
                await _bllService.SyncBCTC_Dien(true);
                await _bllService.SyncBCTC_CangBien(true);
                await _bllService.SyncBCTC_CaoSu(true);
                await _bllService.SyncBCTC_DetMay(true);
                await _bllService.SyncBCTC_Go(true);
                await _bllService.SyncBCTC_HangKhong(true);
                await _bllService.SyncBCTC_Logistic(true);
                await _bllService.SyncBCTC_Nhua(true);
                await _bllService.SyncBCTC_Oto(true);
                await _bllService.SyncBCTC_PhanBon(true);
                await _bllService.SyncBCTC_Than(true);
                await _bllService.SyncBCTC_ThuySan(true);
                await _bllService.SyncBCTC_Ximang(true);
                await _bllService.SyncBCTC_DauKhi(true);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"AnalyzeService.SyncBCTC|EXCEPTION| {ex.Message}");
            }
        }
    }
}
