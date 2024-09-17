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
        private async Task<(EPoint, string)> DG_Go(string code)
        {
            try
            {
                return XNK(EStockType.Go, 5, 15);
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_Go|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
