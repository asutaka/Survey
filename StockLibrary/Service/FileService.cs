using StockLibrary.DAL;
using StockLibrary.Model;
using StockLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockLibrary.Service
{
    public interface IFileService
    {
        List<TuDoanh> HSX(string path);
        List<TuDoanh> HSX2(string path);
        List<TuDoanh> HNX(string path);
    }
    public class FileService : IFileService
    {
        private readonly IStockMongoRepo _stockRepo;
        public FileService(IStockMongoRepo stockRepo)
        {
            _stockRepo = stockRepo;
        }

        public List<TuDoanh> HSX(string path)
        {
            double posstt = 0;
            double posma_ck = 0;

            double posMuaKL = 0;
            double posBanKL = 0;
            double posMuaGiaTri = 0;
            double posBanGiaTri = 0;
            double posMuaKL_ThoaThuan = 0;
            double posBanKL_ThoaThuan = 0;
            double posMuaGiaTri_ThoaThuan = 0;
            double tempVal = 0;

            using (var document = UglyToad.PdfPig.PdfDocument.Open(path))
            {
                var checkNgay = false;
                var setDate = false;
                int step = 0;

                var lData = new List<TuDoanh>();
                var localData = new TuDoanh();
                DateTime date = DateTime.MinValue;
                for (int i = 0; i < document.NumberOfPages; i++)
                {
                    var page = document.GetPage(i + 1);
                    var mcs = page.GetMarkedContents();

                    foreach (var mc in mcs)
                    {
                        var letters = mc.Letters;
                        var paths = mc.Paths;
                        var images = mc.Images;
                        //
                        if (!letters.Any())
                            continue;
                        var location = letters.ElementAt(0).Location;
                        var word = string.Join("", letters.Select(x => x.Value));
                        word = word.Replace(",", "").Replace(".", "");
                        if(!checkNgay)
                        {
                            if (!word.Contains("Ngày"))
                                continue;
                            checkNgay = true;
                            continue;
                        }

                        if (checkNgay && !setDate)
                        {
                            posstt = location.X;
                            posma_ck = letters.ElementAt(letters.Count - 1).Location.X;

                            date = word.ToDateTime("dd/MM/yyyy");
                            setDate = true;
                            continue;
                        }

                        if (word.Contains("Ngày"))
                        {
                            lData.Add(localData);
                            return lData;
                        }

                        if (!setDate)
                            continue;

                        if(date == DateTime.MinValue)
                        {
                            return null;
                        }

                        if (posMuaGiaTri_ThoaThuan <= 0)
                        {
                            if (tempVal <= 0)
                            {
                                if (!word.Contains("Mua"))
                                    continue;
                                tempVal = location.X;
                                continue;
                            }

                            if (posMuaKL <= 0)
                            {
                                posMuaKL = (location.X + tempVal) / 2;
                            }
                            else if (posBanKL <= 0)
                            {
                                posBanKL = (location.X + tempVal) / 2;
                            }
                            else if (posMuaGiaTri <= 0)
                            {
                                posMuaGiaTri = (location.X + tempVal) / 2;
                            }
                            else if (posBanGiaTri <= 0)
                            {
                                posBanGiaTri = (location.X + tempVal) / 2;
                            }
                            else if (posMuaKL_ThoaThuan <= 0)
                            {
                                posMuaKL_ThoaThuan = (location.X + tempVal) / 2;
                            }
                            else if (posBanKL_ThoaThuan <= 0)
                            {
                                posBanKL_ThoaThuan = (location.X + tempVal) / 2;
                            }
                            else if (posMuaGiaTri_ThoaThuan <= 0)
                            {
                                posMuaGiaTri_ThoaThuan = (location.X + tempVal) / 2;
                            }

                            tempVal = location.X;
                            continue;
                        }

                        if (location.X < posstt)
                        {
                            var isstt = int.TryParse(word, out var stt);
                            if (!isstt)
                            {
                                continue;
                            }
                            step = 1;

                            if (localData.stt > 0)
                            {
                                lData.Add(new TuDoanh
                                {
                                    stt = localData.stt,
                                    d = new DateTimeOffset(date, TimeSpan.FromHours(0)).ToUnixTimeSeconds(),
                                    type = ETransactionType.TuDoanh.ToString(),
                                    ma_ck = localData.ma_ck,
                                    kl_mua = localData.kl_mua,
                                    kl_ban = localData.kl_ban,
                                    giatri_mua = localData.giatri_mua,
                                    giatri_ban = localData.giatri_ban,
                                    t = DateTimeOffset.Now.ToUnixTimeSeconds()
                                });
                                localData.ma_ck = string.Empty;
                                localData.kl_mua = 0;
                                localData.kl_ban = 0;
                                localData.giatri_mua = 0;
                                localData.giatri_ban = 0;
                            }

                            localData.stt = stt;
                            continue;
                        }
                        else if (location.X >= posstt
                            && location.X < posma_ck
                            && step == 1)
                        {
                            step = 2;
                            localData.ma_ck = word;
                        }
                        else if (location.X >= posma_ck
                            && location.X < posMuaKL
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.kl_mua = val;
                        }
                        else if (location.X >= posMuaKL
                            && location.X < posBanKL
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.kl_ban = val;
                        }
                        else if (location.X >= posBanKL
                            && location.X < posMuaGiaTri
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.giatri_mua = val;
                        }
                        else if (location.X >= posMuaGiaTri
                            && location.X < posBanGiaTri
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.giatri_ban = val;
                        }
                        else if (location.X >= posBanGiaTri
                            && location.X < posMuaKL_ThoaThuan
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.kl_mua += val;
                        }
                        else if (location.X >= posMuaKL_ThoaThuan
                           && location.X < posBanKL_ThoaThuan
                           && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.kl_ban += val;
                        }
                        else if (location.X >= posBanKL_ThoaThuan
                          && location.X < posMuaGiaTri_ThoaThuan
                          && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.giatri_mua += val;
                        }
                        else if (location.X >= posMuaGiaTri_ThoaThuan
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.giatri_ban += val;
                        }
                    }
                }

                return lData;
            }
        }

        public List<TuDoanh> HNX(string path)
        {
            var content = pdfText(path);
            return MapHNX(content);
        }

        private List<TuDoanh> MapHNX(string content)
        {
            var indexFileHNX = content.IndexOf("Sở Giao dịch Chứng khoán Hà Nội");
            if (indexFileHNX < 0)
                return null;

            var strTieuDe = "GIAO DỊCH TỰ DOANH THEO MÃ CHỨNG KHOÁN";
            var indexNgay = content.IndexOf(strTieuDe);
            if (indexNgay < 0)
                return null;
            content = content.Substring(indexNgay);
            var lLine = content.Split(new string[] { "\n" }, StringSplitOptions.None);
            var dateStr = lLine.ElementAt(0).Replace(strTieuDe, "").Trim();
            var date = dateStr.ToDateTime("dd/MM/yyyy");

            var lStock = _stockRepo.GetAll();

            var isRead = false;
            var isBegin = false;
            var lData = new List<TuDoanh>();
            foreach (var item in lLine)
            {
                try
                {
                    if (item.Contains("Tổng GTGD Tự doanh"))
                    {
                        isRead = true;
                    }

                    if (!isRead)
                        continue;

                    var arrData = item.Split(new string[] { " " }, StringSplitOptions.None);
                    if (!isBegin
                        && arrData.ElementAt(0) == "1")
                    {
                        isBegin = true;
                    }
                    if (!isBegin)
                        continue;
                    var count = arrData.Length;
                    if (count < 8)
                        continue;

                    var maCKCheck = arrData.ElementAt(1).Trim();
                    var check = lStock.Any(x => x.MaCK == maCKCheck);
                    if (check)
                    {
                        var model = new TuDoanh
                        {
                            stt = int.Parse(arrData.ElementAt(0)),
                            d = new DateTimeOffset(date, TimeSpan.FromHours(0)).ToUnixTimeSeconds(),
                            type = ETransactionType.TuDoanh.ToString(),
                            ma_ck = arrData.ElementAt(1),
                            kl_mua = int.Parse(arrData.ElementAt(2).Replace(",", "").Replace(".", "")),
                            giatri_mua = (int)(long.Parse(arrData.ElementAt(3).Replace(",", "").Replace(".", ""))/1000),
                            kl_ban = int.Parse(arrData.ElementAt(4).Replace(",", "").Replace(".", "")),
                            giatri_ban = (int)(long.Parse(arrData.ElementAt(5).Replace(",", "").Replace(".", "")) / 1000),
                            t = DateTimeOffset.Now.ToUnixTimeSeconds()
                        };
                        lData.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return lData;
        }

        public List<TuDoanh> HSX2(string path)
        {
            var content = pdfText(path);
            return MapHSX2(content);
        }

        private string pdfText(string path)
        {
            var reader = new iTextSharp.text.pdf.PdfReader(path);

            string text = string.Empty;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
                text += "\n";
            }
          
            reader.Close();
            return text;
        }

        private List<TuDoanh> MapHSX2(string content)
        {
            var lIndexKeyword = content.AllIndexesOf("SỞ GIAO DỊCH CHỨNG KHOÁN TP. HỒ CHÍ MINH");
            if (!lIndexKeyword.Any())
                return null;
            
            if (lIndexKeyword.Count >= 2)
            {
                content = content.Substring(lIndexKeyword.ElementAt(0), lIndexKeyword.ElementAt(1) - lIndexKeyword.ElementAt(0));
            }
            else
            {
                content = content.Substring(lIndexKeyword.ElementAt(0));
            }

            var indexNgay = content.IndexOf("Ngày");
            if (indexNgay < 0)
                return null;
            content = content.Substring(indexNgay + 4);
            var lLine = content.Split(new string[] { "\n" }, StringSplitOptions.None);
            var dateStr = lLine.ElementAt(0).Trim();
            var date = dateStr.ToDateTime("dd/MM/yyyy");

            var lStock = _stockRepo.GetAll();

            var isRead = false;
            var isBegin = false;
            var lData = new List<TuDoanh>();
            foreach (var item in lLine)
            {
                try
                {
                    if (item.Contains("Tổng cộng"))
                    {
                        isRead = true;
                    }

                    if (!isRead)
                        continue;

                    var arrData = item.Split(new string[] { " " }, StringSplitOptions.None);
                    if (!isBegin
                        && arrData.ElementAt(0) == "1")
                    {
                        isBegin = true;
                    }
                    if (!isBegin)
                        continue;
                    var count = arrData.Length;
                    if (count < 2)
                        continue;

                    var maCKCheck = arrData.ElementAt(1).Trim();
                    var check = lStock.Any(x => x.MaCK == maCKCheck);
                    if(check)
                    {
                        var model = new TuDoanh
                        {
                            stt = int.Parse(arrData.ElementAt(0)),
                            d = new DateTimeOffset(date, TimeSpan.FromHours(0)).ToUnixTimeSeconds(),
                            type = ETransactionType.TuDoanh.ToString(),
                            ma_ck = arrData.ElementAt(1),
                            t = DateTimeOffset.Now.ToUnixTimeSeconds(),
                            recheck = true
                        };

                        if (count >= 6)
                        {
                            model.kl_mua = int.Parse(arrData.ElementAt(2).Replace(",", "").Replace(".", ""));
                            model.kl_ban = int.Parse(arrData.ElementAt(3).Replace(",", "").Replace(".", ""));
                            model.giatri_mua = int.Parse(arrData.ElementAt(4).Replace(",", "").Replace(".", ""));
                            model.giatri_ban = int.Parse(arrData.ElementAt(5).Replace(",", "").Replace(".", ""));
                        }
                        else if (count >= 4)
                        {
                            model.kl_ban = int.Parse(arrData.ElementAt(2).Replace(",", "").Replace(".", ""));
                            model.giatri_ban = int.Parse(arrData.ElementAt(3).Replace(",", "").Replace(".", ""));
                        }
                        lData.Add(model);
                    }
                    else
                    {
                        var last = lData.Last();
                        if (count >= 4)
                        {
                            last.kl_mua = int.Parse(arrData.ElementAt(0).Replace(",", "").Replace(".", ""));
                            last.kl_ban = int.Parse(arrData.ElementAt(1).Replace(",", "").Replace(".", ""));
                            last.giatri_mua = int.Parse(arrData.ElementAt(2).Replace(",", "").Replace(".", ""));
                            last.giatri_ban = int.Parse(arrData.ElementAt(3).Replace(",", "").Replace(".", ""));
                        }
                        else if (count >= 2)
                        {
                            last.kl_ban = int.Parse(arrData.ElementAt(0).Replace(",", "").Replace(".", ""));
                            last.giatri_ban = int.Parse(arrData.ElementAt(1).Replace(",", "").Replace(".", ""));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return lData;
        }
    }
}
