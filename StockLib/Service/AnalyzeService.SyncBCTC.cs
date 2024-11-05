namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        public async Task SyncBCTC()
        {
            try
            {
                await _bllService.SyncBCTC(true);
                await _bllService.SyncBCTC_NganHang(true);
                await _bllService.SyncBCTC_BatDongSan(true);
                await _bllService.SyncBCTC_ChungKhoan(true);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"AnalyzeService.SyncBCTC|EXCEPTION| {ex.Message}");
            }
        }
    }
}
