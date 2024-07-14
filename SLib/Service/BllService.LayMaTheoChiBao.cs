using SLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task<(int, string)> LayMaTheoChiBao()
        {
            try
            {
                var strOutput = new StringBuilder();

                var lMa = await _apiService.GetMaTheoChiBao();
                if (lMa is null
                    || !lMa.Any())
                    return (0, null);

                var lMaClean = lMa.Where(x => x.match_price > x.basic_price && x.accumylated_vol > 5000).OrderByDescending(x => Math.Abs(x.change_vol_percent_5));
                var lOutput = new List<Stock>();
                var lStock = _stockRepo.GetAll();
                foreach (var item in lMaClean)
                {
                    var entityStock = lStock.FirstOrDefault(x => x.s == item.symbol && x.status > 0);
                    if(entityStock != null)
                        lOutput.Add(entityStock);
                }

                strOutput.AppendLine($"[Thông báo] Top 10 cổ phiếu vừa cắt lên MA20:");
                var index = 1;
                foreach (var item in lOutput.OrderBy(x => x.rank).Take(10).ToList())
                {
                    var content = $"{index}. {item.s}";
                    strOutput.AppendLine(content);
                    index++;
                }
                strOutput.AppendLine();
                strOutput.AppendLine($"[Thông báo] Top 10 cổ phiếu vừa cắt lên MA20 với khối lượng đột biến:");
                index = 1;
                foreach (var item in lOutput.Take(10).ToList())
                {
                    var content = $"{index}. {item.s}";
                    strOutput.AppendLine(content);
                    index++;
                }

                return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return (0, null);
        }
    }
}
