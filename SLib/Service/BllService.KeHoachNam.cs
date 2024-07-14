using SLib.Util;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task<(int, string)> KeHoachNam(string code)
        {
            try
            {
                var dt = DateTime.Now;
                var KeHoach = await _apiService.GetKeHoachThucHien(code);
                if (!KeHoach.data.Any())
                    return (0, null);

                var first = KeHoach.data.First();
                if (first.year < dt.Year)
                    return (0, null);

                var strOutput = new StringBuilder();
                strOutput.AppendLine($"Kế hoạch năm:");
                int count = 1;
                foreach (var item in KeHoach.data)
                {
                    var quarterKey = item.quarter.FirstOrDefault(x => x.quarter == 0 || x.quarter == 5);
                    var strDT = string.Join(",", item.quarter.Where(x => x.quarter >= 1 && x.quarter <= 4)
                        .Select((x, index) => $"{index.ToQuarter()}({x.isa3_report.ToString("#,##0.##")}/{x.isa3_percent}%)"));
                    var strLN = string.Join(",", item.quarter.Where(x => x.quarter >= 1 && x.quarter <= 4)
                        .Select((x, index) => $"{index.ToQuarter()}({x.isa16_report.ToString("#,##0.##")}/{x.isa16_percent}%)"));
                    var strLNST = string.Join(",", item.quarter.Where(x => x.quarter >= 1 && x.quarter <= 4)
                        .Select((x, index) => $"{index.ToQuarter()}({x.isa22_report.ToString("#,##0.##")}/{x.isa22_percent}%)"));

                    strOutput.AppendLine($"+ {item.year}:");
                    strOutput.AppendLine($"\tDT({item.isa3.ToString("#,##0.##")})[{strDT}]: {quarterKey.isa3_percent}%");
                    strOutput.AppendLine($"\tLN({item.isa16.ToString("#,##0.##")})[{strLN}]: {quarterKey.isa16_percent}%");
                    strOutput.AppendLine($"\tLNST({item.isa22.ToString("#,##0.##")})[{strLNST}]: {quarterKey.isa22_percent}%");
                    if (++count > 5)
                        break;
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
