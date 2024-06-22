using SurveyStock.Model.MongoModel;
using SurveyStock.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SurveyStock.Service
{
    public interface IFileService
    {
        List<Transaction> HSX(string path);
    }
    public class FileService : IFileService
    {
        public List<Transaction> HSX(string path)
        {
            double posSTT = 0;
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
                var isComplete = false;
                int step = 0;

                var lData = new List<Transaction>();
                var localData = new Transaction();
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
                        if (!checkNgay
                            && word.Contains("Ngày"))
                        {
                            if (isComplete)
                            {
                                lData.Add(localData);
                                return lData;
                            }
                            checkNgay = true;
                            continue;
                        }
                        if (checkNgay)
                        {
                            checkNgay = false;
                            isComplete = true;
                            posSTT = location.X;
                            posma_ck = letters.ElementAt(letters.Count - 1).Location.X;

                            date = word.ToDateTime("dd/MM/yyyy");
                            if ((DateTime.Now - date).TotalDays >= 1)
                            {
                                var result = MessageBox.Show($"Thống kê không phải của ngày hiện tại( {word} ), tiếp tục?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (result == DialogResult.No)
                                    return lData;
                            }
                            continue;
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

                        if (location.X < posSTT)
                        {
                            var isSTT = int.TryParse(word, out var stt);
                            if (!isSTT)
                            {
                                continue;
                            }
                            //Console.WriteLine(word);
                            step = 1;

                            if (localData.stt > 0)
                            {
                                lData.Add(new Transaction
                                {
                                    stt = localData.stt,
                                    ngay = date,
                                    type = ETransactionType.TuDoanh.ToString(),
                                    ma_ck = localData.ma_ck,
                                    kl_mua = localData.kl_mua,
                                    kl_ban = localData.kl_ban,
                                    giatri_mua = localData.giatri_mua,
                                    giatri_ban = localData.giatri_ban
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
                        else if (location.X >= posSTT
                            && location.X < posma_ck
                            && step == 1)
                        {
                            step = 2;
                            localData.ma_ck = word;
                            //Console.WriteLine(word);
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
    }
}
