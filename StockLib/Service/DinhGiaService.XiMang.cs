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
        //Đầu tư công + xuất khẩu
        private (EPoint, string) DG_XiMang(string code)
        {
            try
            {
                var xnk = XNK(EStockType.XiMang, 5, 15);
                //ĐTC 

            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_XiMang|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
