using Microsoft.Extensions.Logging;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<EPoint> DG_DauKhi(string code)
        {
            try
            {
                return await DinhGia_Forex(EForex.CL, 5, 15);
            }
            catch(Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_DauKhi|EXCEPTION| {ex.Message}");
            }
            return EPoint.Unknown;
        }
    }
}
