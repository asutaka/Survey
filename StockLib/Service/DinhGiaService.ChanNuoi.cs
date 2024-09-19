using Microsoft.Extensions.Logging;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<(EPoint, string)> DG_ChanNuoi(string code)
        {
            try
            {
                var bdi_qoq = await _apiService.Tradingeconimic_GetForex("lean-hogs");
                return EPointResponse(bdi_qoq, 5, 15, "Giá Thịt Lợn");
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_ChanNuoi|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
