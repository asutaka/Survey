using Microsoft.Extensions.Logging;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private (EPoint, string) DG_VanTaiBien(string code)
        {
            try
            {
                var vt = ModeThongKe(EKeyTongCucThongKe.VanTai_DuongBien, 5, 15);
                //Biểu đồ giá cước vận tải(BDI)?
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_VanTaiBien|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
