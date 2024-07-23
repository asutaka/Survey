using MongoDB.Driver;
using SLib.Model;
using SLib.Model.GoogleSheetModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

                var range = $"DoanhThu!A:Z";
                var request = _googleSheetValues.Get(entityNganHang.ssi, range);
                var response = request.Execute();
                var values = response.Values;
                var lRes = values.MapFromGoogleData();
                lRes.RemoveAt(0);
                foreach ( var row in lRes )
                {
                    if (string.IsNullOrWhiteSpace(row.Code))
                        continue;

                    var filterFinance = Builders<Financial>.Filter.Eq(x => x.s, row.Code);
                    var lFinance = _financialRepo.GetByFilter(filterFinance);
                    lFinance = lFinance?.Where(x => x.lengthReport > 0 && x.lengthReport < 5).OrderByDescending(x => x.d).ToList();
                    if (lFinance is null)
                        continue;
                }
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
                    item.Code = value[0].ToString().Trim();
                }
                if(count > 1)
                {
                    item.Q2_2024 = value[1].ToString().Trim();
                }
                if (count > 2)
                {
                    item.Q1_2024 = value[2].ToString().Trim();
                }
                if (count > 3)
                {
                    item.Q4_2023 = value[3].ToString().Trim();
                }
                if (count > 4)
                {
                    item.Q3_2023 = value[4].ToString().Trim();
                }
                if (count > 5)
                {
                    item.Q2_2023 = value[5].ToString().Trim();
                }
                if (count > 6)
                {
                    item.Q1_2023 = value[6].ToString().Trim();
                }
                if (count > 7)
                {
                    item.Q4_2022 = value[7].ToString().Trim();
                }
                if (count > 8)
                {
                    item.Q3_2022 = value[8].ToString().Trim();
                }
                if (count > 9)
                {
                    item.Q2_2022 = value[9].ToString().Trim();
                }
                if (count > 10)
                {
                    item.Q1_2022 = value[10].ToString().Trim();
                }
                if (count > 11)
                {
                    item.Q4_2021 = value[11].ToString().Trim();
                }
                if (count > 12)
                {
                    item.Q3_2021 = value[12].ToString().Trim();
                }
                if (count > 13)
                {
                    item.Q2_2021 = value[13].ToString().Trim();
                }
                if (count > 14)
                {
                    item.Q1_2021 = value[14].ToString().Trim();
                }
                if (count > 15)
                {
                    item.Q4_2020 = value[15].ToString().Trim();
                }
                if (count > 16)
                {
                    item.Q3_2020 = value[16].ToString().Trim();
                }
                if (count > 17)
                {
                    item.Q2_2020 = value[17].ToString().Trim();
                }
                if (count > 18)
                {
                    item.Q1_2020 = value[18].ToString().Trim();
                }
                items.Add(item);
            }
            return items;
        }
    }
}
