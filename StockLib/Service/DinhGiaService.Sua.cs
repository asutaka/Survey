using Microsoft.Extensions.Logging;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<(EPoint, string)> DG_Sua(string code)
        {
            try
            {
                var xk = XNK(EStockType.Sua, 5, 15);

                var bdi_qoq = await _apiService.Tradingeconimic_GetForex("milk");
                var bdi = EPointResponse(bdi_qoq, 5, 15, "Giá Sữa");

                var sBuilder = new StringBuilder();
                sBuilder.AppendLine(xk.Item2);
                sBuilder.AppendLine(bdi.Item2);

                return (MergeEnpoint(xk.Item1, bdi.Item1), sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_Sua|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
