using Microsoft.Extensions.Logging;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<(EPoint, string)> DG_CaPhe(string code)
        {
            try
            {
                var xk = XNK(EStockType.CaPhe, 5, 15);

                var bdi_qoq = await _apiService.Tradingeconimic_GetForex("coffee");
                var bdi = EPointResponse(bdi_qoq, 5, 15, "Giá Cà phê");

                var sBuilder = new StringBuilder();
                sBuilder.AppendLine(xk.Item2);
                sBuilder.AppendLine(bdi.Item2);

                return (MergeEnpoint(xk.Item1, bdi.Item1), sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_CaPhe|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
