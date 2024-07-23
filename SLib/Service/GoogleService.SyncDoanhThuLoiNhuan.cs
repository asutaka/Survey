using MongoDB.Driver;
using SLib.Model;
using SLib.Model.GoogleSheetModel;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;

namespace SLib.Service
{
    public partial class GoogleService 
    {
        private List<GoogleData> GetGoogleData(string spreadId, string sheetName)
        {
            var range = $"{sheetName}!A:Z";
            var request = _googleSheetValues.Get(spreadId, range);
            var response = request.Execute();
            var values = response.Values;
            return values.MapFromRangeData();
        }
        public async Task GGDoanhThu_NganHang()
        {
            try
            {
                var sheetName = "DoanhThu";
                var lGoogle = _googleRepo.GetAll();
                var entityNganHang = lGoogle.FirstOrDefault(x => x.fn == "NganHang");
                if (entityNganHang is null)
                    return;

                var lRecord = GetGoogleData(entityNganHang.ssi, sheetName);
                var entityTitle = lRecord.First();
                lRecord.RemoveAt(0);
                var rowIndex = 1;
                foreach ( var row in lRecord)
                {
                    rowIndex++;
                    if (string.IsNullOrWhiteSpace(row.Code))
                        continue;

                    var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, row.Code));
                    lFinancial = lFinancial?.Where(x => x.lengthReport > 0 && x.lengthReport < 5).OrderByDescending(x => x.d).ToList();
                    if (lFinancial is null)
                        continue;

                    var updateOn = row.lValues.Count();
                    updateOn++;

                    var demTitle = 0;
                    foreach (var itemTitle in entityTitle.lValues)
                    {
                        try
                        {
                            demTitle++;
                            if (demTitle <= updateOn)
                                continue;
                            if (string.IsNullOrWhiteSpace(itemTitle))
                                continue;
                            var mq = itemTitle.Replace("Q", "").Split('/');
                            var quarter = int.Parse(mq[0]);
                            var year = int.Parse(mq[1]);
                            var entityFinancial = lFinancial.FirstOrDefault(x => x.yearReport == year && x.lengthReport == quarter);
                            if (entityFinancial is null)
                            {
                                row.lValues.Add(string.Empty);
                                continue;
                            }
                            if(rowIndex >= 50)
                            {
                                row.lValues.Add(entityFinancial.revenue.ToString());
                            }
                            else
                            {
                                //tỉ lệ
                                var entityFinancialPrev = lFinancial.FirstOrDefault(x => x.yearReport == year - 1 && x.lengthReport == quarter);
                                if(entityFinancialPrev is null)
                                {
                                    row.lValues.Add(string.Empty);
                                    continue;
                                }
                                var rate = Math.Round(100 * (-1 + (entityFinancial.revenue ?? 0) / (entityFinancialPrev.revenue ?? 1)), 1);
                                row.lValues.Add($"'{rate}");
                            }
                        }
                        catch (Exception ex) 
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    if (!row.lValues.Any())
                        continue;

                    updateOn++;
                    var lastColUpdate = updateOn + row.lValues.Count();
                    var rangeUpdate = $"{sheetName}!{((ESheetCol)updateOn).ToString()}{rowIndex}:{((ESheetCol)lastColUpdate).ToString()}{rowIndex}";
                    //
                    Put(rowIndex, row, entityNganHang.ssi, sheetName, rangeUpdate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

   
}
