using Microsoft.Extensions.Logging;
using StockLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<EPoint> DG_CNTT(string code)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_CNTT|EXCEPTION| {ex.Message}");
            }
            return EPoint.Unknown;
        }
    }
}
