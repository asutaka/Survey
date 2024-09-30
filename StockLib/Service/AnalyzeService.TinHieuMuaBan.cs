using iTextSharp.text.pdf.qrcode;
using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        public async Task<(int, string)> TinHieuMuaBan()
        {
            try
            {
                var lDanCustom = new List<Stock>();
                foreach (var item in StaticVal._lStock)
                {
                    if (item.indicator is null
                        || !item.indicator.Any())
                        continue;

                    var lData = await _apiService.SSI_GetDataStock(item.s);
                    decimal a = 10;
                    if (item.e.Equals("Hose", StringComparison.OrdinalIgnoreCase))
                    {
                        a = 7;
                    }
                    else if (item.e.Equals("Upcom", StringComparison.OrdinalIgnoreCase))
                    {
                        a = 15;
                    }

                    var isDanCustomSignal = lData.CheckDanZangerCustom(a);
                    if (isDanCustomSignal)
                    {
                        lDanCustom.Add(item);
                    }
                }
                
                var sBuilder = new StringBuilder();

                if (lDanCustom.Any())
                {
                    sBuilder.AppendLine("[Tín hiệu Danz Volume]");

                    foreach (var item in lDanCustom.OrderByDescending(x => x.indicator.FirstOrDefault(x => x.type == (int)EIndicator.DanZangerVolumne).rank))
                    {
                        var indicator = item.indicator.FirstOrDefault(x => x.type == (int)EIndicator.DanZangerVolumne);
                        sBuilder.AppendLine($"{item.s}|TP trung bình: {indicator.avg_rate}%| Win/Loss: {indicator.win_rate}%/{indicator.loss_rate}%");
                    }
                }

                if(string.IsNullOrWhiteSpace(sBuilder.ToString()))
                {
                    return (0, null);
                }
                return (1, sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TinHieuMuaBan|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }
    }
}
