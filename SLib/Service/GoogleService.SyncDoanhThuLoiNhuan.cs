using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using SLib.Google;
using SLib.Model.GoogleSheetModel;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace SLib.Service
{
    public partial class GoogleService 
    {
        public async Task GGDoanhThu_NganHang()
        {
            try
            {
                var lGoogle = _googleRepo.GetAll();
                var entityNganHang = lGoogle.FirstOrDefault(x => x.fn == "NganHang");
                if (entityNganHang is null)
                    return;

                var range = $"DoanhThu!A:S";
                var request = _googleSheetValues.Get(entityNganHang.ssi, range);
                var response = request.Execute();
                var values = response.Values;
                var res = values.MapFromGoogleData();
                var tmp = 1;
                //    return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public static class Mapper
    {
        public static List<GoogleData> MapFromGoogleData(this IList<IList<object>> values)
        {
            var items = new List<GoogleData>();
            foreach (var value in values)
            {
                var item = new GoogleData();
                if(!value.Any())
                {
                    items.Add(item);
                    continue;
                }
                var count = value.Count();
                if(count > 0)
                {
                    item.Coin = value[0].ToString();
                }
                if(count > 1)
                {
                    item.Q2_2024 = value[1].ToString();
                }
                if (count > 2)
                {
                    item.Q1_2024 = value[2].ToString();
                }
                if (count > 3)
                {
                    item.Q4_2023 = value[3].ToString();
                }
                if (count > 4)
                {
                    item.Q3_2023 = value[4].ToString();
                }
                if (count > 5)
                {
                    item.Q2_2023 = value[5].ToString();
                }
                if (count > 6)
                {
                    item.Q1_2023 = value[6].ToString();
                }
                if (count > 7)
                {
                    item.Q4_2022 = value[7].ToString();
                }
                if (count > 8)
                {
                    item.Q3_2022 = value[8].ToString();
                }
                if (count > 9)
                {
                    item.Q2_2022 = value[9].ToString();
                }
                if (count > 10)
                {
                    item.Q1_2022 = value[10].ToString();
                }
                if (count > 11)
                {
                    item.Q4_2021 = value[11].ToString();
                }
                if (count > 12)
                {
                    item.Q3_2021 = value[12].ToString();
                }
                if (count > 13)
                {
                    item.Q2_2021 = value[13].ToString();
                }
                if (count > 14)
                {
                    item.Q1_2021 = value[14].ToString();
                }
                if (count > 15)
                {
                    item.Q4_2020 = value[15].ToString();
                }
                if (count > 16)
                {
                    item.Q3_2020 = value[16].ToString();
                }
                if (count > 17)
                {
                    item.Q2_2020 = value[17].ToString();
                }
                if (count > 18)
                {
                    item.Q1_2020 = value[18].ToString();
                }
                items.Add(item);
            }
            return items;
        }
    }
}
