using Microsoft.Extensions.Logging;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private (EPoint, string) DG_Than(string code)
        {
            try
            {
                return XNK(EStockType.Than, 5, 15);
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_Than|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
