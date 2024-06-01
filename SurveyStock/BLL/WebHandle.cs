﻿using Newtonsoft.Json;
using SurveyStock.DAL;
using SurveyStock.Model;
using SurveyStock.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace SurveyStock.BLL
{
    public static class WebHandle
    {
        public static void SyncCompany()
        {
            //EStockExchange
            var dicLink = new Dictionary<EStockExchange, string>();
            dicLink.Add(EStockExchange.Hose, "https://bgapidatafeed.vps.com.vn/getlistckindex/hose");
            dicLink.Add(EStockExchange.HNX, "https://bgapidatafeed.vps.com.vn/getlistckindex/hnx");
            dicLink.Add(EStockExchange.Upcom, "https://bgapidatafeed.vps.com.vn/getlistckindex/upcom");

            var lCom = sqliteComDB.GetData_Company();
            foreach (var itemType in dicLink)
            {
                var client = new HttpClient { BaseAddress = new Uri(itemType.Value) };
                var responseMessage = client.GetAsync("", HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
                var resultArray = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var lData = JsonConvert.DeserializeObject<List<string>>(resultArray);
                int index = lCom.Count() + 1;
                foreach (var itemData in lData)
                {
                    if (lCom.Any(x => x.stock_code.Equals(itemData)))
                        continue;

                    sqliteComDB.Insert_Company(new CompanyModel
                    {
                        id = index++,
                        company_name = string.Empty,
                        stock_code = itemData,
                        stock_exchange = (int)itemType.Key,
                        cap = 0,
                        status = 0
                    });
                }
            }

            SyncTable();
        }

        public static void SyncTable()
        {
            var lData = sqliteComDB.GetData_Company();
            foreach (var item in lData)
            {
                var checkExist = sqliteDayDB.CheckTableExists(item.stock_code, item.stock_exchange);
                if(!checkExist)
                {
                    sqliteDayDB.CreateTable(item.stock_code, item.stock_exchange);
                }
            }
        }

        private const string _urlData = "https://histdatafeed.vps.com.vn/tradingview/history?symbol={0}&resolution={1}&from={2}&to={3}";
        public static void SyncDataDay()
        {
            var year = 2005;
            var lData = sqliteComDB.GetData_Company();
            int dem = 1;
            foreach (var item in lData)
            {
                if (item.status <= 0)
                {
                    continue;
                }

                year = 2005;
                try
                {
                    Console.WriteLine($"{dem++}: {item.stock_code}");
                    DateTime dtLast = DateTime.MinValue;
                    var ldataDb = sqliteDayDB.GetData(item.stock_code);
                    if (ldataDb.Any())
                    {
                        var last = ldataDb.Last();
                        dtLast = last.t.UnixTimeStampToDateTime();
                    }

                    DateTime dt;
                    do
                    {
                        dt = new DateTime(year++, 1, 1);
                        var dtNext = dt.AddYears(1);
                        if(dt < dtLast)
                        {
                            dt = dtLast.AddSeconds(1);
                            dtNext = new DateTime(dtLast.Year, 12, 31, 23, 59, 59);
                            year = dtLast.Year + 1;
                        }
                        var dtOffsetPrev = new DateTimeOffset(dt).ToUnixTimeSeconds();
                        var dtOffsetNext = new DateTimeOffset(dtNext).ToUnixTimeSeconds();
                        var url = string.Format(_urlData, item.stock_code, "1D", dtOffsetPrev, dtOffsetNext);

                        var client = new HttpClient { BaseAddress = new Uri(url) };
                        var responseMessage = client.GetAsync("", HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
                        var resultArray = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var responseModel = JsonConvert.DeserializeObject<ResponseDataModel>(resultArray);
                        if(responseModel.t.Any())
                        {
                            var count = responseModel.t.Count();
                            for (int i = 0; i < count; i++)
                            {
                                var t = responseModel.t.ElementAt(i);
                                var o = responseModel.o.ElementAt(i);
                                var c = responseModel.c.ElementAt(i);
                                var h = responseModel.h.ElementAt(i);
                                var l = responseModel.l.ElementAt(i);
                                var v = responseModel.v.ElementAt(i);
                                sqliteDayDB.Insert(item.stock_code, new DataModel
                                {
                                    t = t,
                                    o = o,
                                    c = c,
                                    h = h,
                                    l = l,
                                    v = v
                                }, item.stock_exchange);
                            }
                        }
                        Thread.Sleep(100);
                    }
                    while (dt < DateTime.Now);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
