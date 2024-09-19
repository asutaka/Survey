using Microsoft.Extensions.Logging;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<(EPoint, string)> DG_Thep(string code)
        {
            try
            {
                var xk = XNK(EStockType.Thep, 5, 15);

                var bdi_qoq = await _apiService.Tradingeconimic_GetForex("steel");
                var bdi = EPointResponse(bdi_qoq, 5, 15, "Giá Thép");

                var sBuilder = new StringBuilder();
                sBuilder.AppendLine(xk.Item2);
                sBuilder.AppendLine(bdi.Item2);

                return (MergeEnpoint(xk.Item1, bdi.Item1), sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_Thep|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
