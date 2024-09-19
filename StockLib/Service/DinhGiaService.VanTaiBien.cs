using Microsoft.Extensions.Logging;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<(EPoint, string)> DG_VanTaiBien(string code)
        {
            try
            {
                var vt = ModeThongKe(EKeyTongCucThongKe.VanTai_DuongBien, 5, 15);
                
                var bdi_qoq = await _apiService.Tradingeconimic_GetForex("baltic");
                var bdi = EPointResponse(bdi_qoq, 5, 15, "Cước vận tải");

                var sBuilder = new StringBuilder();
                sBuilder.AppendLine(vt.Item2);
                sBuilder.AppendLine(bdi.Item2);

                return (MergeEnpoint(vt.Item1, bdi.Item1), sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_VanTaiBien|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
