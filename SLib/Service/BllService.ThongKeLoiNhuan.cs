using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task<(int, string)> ThongKeLoiNhuan(string code)
        {
            try
            {
                var lLoiNhuan = await _apiService.ThongKeLoiNhuan(code);
                lLoiNhuan.Reverse();
                if (!lLoiNhuan.Any())
                    return (0, null);

                var strOutput = new StringBuilder();
                strOutput.AppendLine($"Góc nhìn tài chính:");
                strOutput.AppendLine($" - Lợi nhuận:");
                foreach (var item in lLoiNhuan)
                {
                    strOutput.AppendLine($"  + {item.Name.Replace(" ","/")}: {item.Profit.ToString("#,##0.0")} tỷ/{item.Rate_qoq}%");
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
