using iTextSharp.text;
using SLib.Util;
using System;
using System.Collections.Generic;
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
                strOutput.AppendLine($" - Kế hoạch năm:");
                var quarterKey = first.quarter.FirstOrDefault(x => x.quarter == 0 || x.quarter == 5);
                var strDT = string.Join(",", first.quarter.Where(x => x.quarter >= 1 && x.quarter <= 4)
                    .Select((x, index) => $"{index.ToQuarter()}({x.isa3_report.ToString("#,##0.##")}/{x.isa3_percent}%)"));
                var strLN = string.Join(",", first.quarter.Where(x => x.quarter >= 1 && x.quarter <= 4)
                    .Select((x, index) => $"{index.ToQuarter()}({x.isa16_report.ToString("#,##0.##")}/{x.isa16_percent}%)"));
                var strLNST = string.Join(",", first.quarter.Where(x => x.quarter >= 1 && x.quarter <= 4)
                    .Select((x, index) => $"{index.ToQuarter()}({x.isa22_report.ToString("#,##0.##")}/{x.isa22_percent}%)"));

                strOutput.AppendLine($"+ {first.year}:");
                strOutput.AppendLine($"\tDT({first.isa3.ToString("#,##0.##")})[{strDT}]: {quarterKey.isa3_percent}%");
                strOutput.AppendLine($"\tLN({first.isa16.ToString("#,##0.##")})[{strLN}]: {quarterKey.isa16_percent}%");
                strOutput.AppendLine($"\tLNST({first.isa22.ToString("#,##0.##")})[{strLNST}]: {quarterKey.isa22_percent}%");
                return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (0, null);
        }

        private async Task<decimal> LoiNhuanDN_TB(string code)
        {
            var dt = DateTime.Now;
            var KeHoach = await _apiService.GetKeHoachThucHien(code);
            if (!KeHoach.data.Any())
                return 0;
            KeHoach.data = KeHoach.data.Where(x => x.year < dt.Year).ToList();
            var lKetquaNam = KeHoach.data.Select(x => x.quarter.FirstOrDefault(y => y.quarter == 0));
            var lLoiNhuan = new List<decimal>();
            var count = lKetquaNam.Count();
            if(count > 1)
            {
                for (var i = 0; i < count - 1; i++)
                {
                    var itemCur = lKetquaNam.ElementAt(i).isa16_report;
                    var itemPrev = lKetquaNam.ElementAt(i + 1).isa16_report;



                    var loinhuanTB = Math.Abs(Math.Round(100 * (-1 + itemCur / itemPrev)));
                    loinhuanTB = itemCur >= itemPrev ? loinhuanTB : -loinhuanTB;
                    lLoiNhuan.Add(loinhuanTB);
                }
            }
            if (!lLoiNhuan.Any())
                return 0;
            return lLoiNhuan.Average();
        }
    }
}
