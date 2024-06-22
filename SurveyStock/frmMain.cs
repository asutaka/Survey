using DevExpress.XtraEditors;
using SurveyStock.Service;
using System;
using System.Threading;
using System.Windows.Forms;

namespace SurveyStock
{
    public partial class frmMain : XtraForm
    {
        private readonly IFileService _fileService;
        private readonly IBllService _bllService;
        public frmMain(IFileService fileService,
                        IBllService bllService)
        {
            InitializeComponent();
            _fileService = fileService;
            _bllService = bllService;
        }

        private void btnReadFile_Click(object sender, EventArgs e)
        {
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var lData = _fileService.HSX(fileDialog.FileName);
                    var taskInsert = _bllService.InsertTransaction(lData);
                    while (true)
                    {
                        if (taskInsert.IsCompleted)
                            break;
                        Thread.Sleep(1000);
                    }

                    MessageBox.Show("Đã lưu dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Lỗi không xác định", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //public string HSX(string path)
        //{
        //    var posMuaKL = 0;
        //    var posBanKL = 0;
        //    var posMuaGiaTri = 0;
        //    var posBanGiaTri = 0;
        //    var posMuaKL_ThoaThuan = 0;
        //    var posBanKL_ThoaThuan = 0;
        //    var posMuaGiaTri_ThoaThuan = 0;

        //    using (PdfDocument document = PdfDocument.Open(path))
        //    {
        //        for (int i = 0; i < document.NumberOfPages; i++)
        //        {
        //            var page = document.GetPage(i + 1);
        //            var mcs = page.GetMarkedContents();

        //            foreach (var mc in mcs)
        //            {
        //                var letters = mc.Letters;
        //                var paths = mc.Paths;
        //                var images = mc.Images;
        //                //
        //                if (!letters.Any())
        //                    continue;
        //                var location = letters.ElementAt(0).Location;
        //                var word = string.Join("", letters.Select(x => x.Value));

        //                Console.WriteLine($"{word} {location}");


        //                //foreach (var letter in letters)
        //                //{
        //                //    if (letter.Value.Contains("ANV"))
        //                //    {
        //                //        var tmp = 1;
        //                //    }
        //                //    Console.Write(letter.Value);
        //                //    Console.Write(letter.Location);
        //                //}
        //                Console.WriteLine();
        //            }
        //        }
        //    }

        //    string text = string.Empty;
        //    return text;
        //}

        public string pdfText1(string path)
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

        //public void HSX(string path)
        //{
        //    var content = pdfText(fileDialog.FileName);
        //    var lIndexKeyword = content.AllIndexesOf("SỞ GIAO DỊCH CHỨNG KHOÁN TP. HỒ CHÍ MINH");
        //    if (!lIndexKeyword.Any())
        //        return;

        //    if(lIndexKeyword.Count >= 2)
        //    {
        //        content = content.Substring(lIndexKeyword.ElementAt(0), lIndexKeyword.ElementAt(1) - lIndexKeyword.ElementAt(0));
        //    }
        //    else
        //    {
        //        content = content.Substring(lIndexKeyword.ElementAt(0));
        //    }

        //    var indexNgay = content.IndexOf("Ngày");
        //    if (indexNgay < 0)
        //        return;
        //    content = content.Substring(indexNgay + 4);
        //    var lLine = content.Split(new string[] { "\n" }, StringSplitOptions.None);
        //    var dateStr = lLine.ElementAt(0).Trim();
        //    var date = dateStr.ToDateTime("dd/MM/yyyy");
        //    if ((DateTime.Now - date).TotalDays >= 1)
        //    {
        //        var result = MessageBox.Show($"Thống kê không phải của ngày hiện tại( {dateStr} ), tiếp tục?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //        if (result == DialogResult.No)
        //            return;
        //    }

        //    var isRead = false;
        //    var isBegin = false;
        //    var lData = new List<clsData>();
        //    var localModel = new clsData();
        //    foreach (var item in lLine)
        //    {
        //        try
        //        {
        //            if (item.Contains("Tổng cộng"))
        //            {
        //                isRead = true;
        //            }

        //            if (!isRead)
        //                continue;

        //            var arrData = item.Split(new string[] { " " }, StringSplitOptions.None);
        //            if (!isBegin
        //                && arrData.ElementAt(0) == "1")
        //            {
        //                isBegin = true;
        //            }
        //            if (!isBegin)
        //                continue;
        //            var count = arrData.Length;
        //            if (count >= 6)
        //            {
        //                var model = new clsData
        //                {
        //                    STT = int.Parse(arrData.ElementAt(0)),
        //                    MaCK = arrData.ElementAt(1),
        //                    KLMua = int.Parse(arrData.ElementAt(2).Replace(",", "").Replace(".", "")),
        //                    KLBan = int.Parse(arrData.ElementAt(3).Replace(",", "").Replace(".", "")),
        //                    GiaTriMua = int.Parse(arrData.ElementAt(4).Replace(",", "").Replace(".", "")),
        //                    GiaTriBan = int.Parse(arrData.ElementAt(5).Replace(",", "").Replace(".", "")),
        //                };
        //                lData.Add(model);
        //            }
        //            else
        //            {
        //                if (count == 2)
        //                {
        //                    localModel.STT = int.Parse(arrData.ElementAt(0));
        //                    localModel.MaCK = arrData.ElementAt(1);
        //                }
        //                else if (count == 4)
        //                {
        //                    var model = new clsData
        //                    {
        //                        STT = localModel.STT,
        //                        MaCK = localModel.MaCK,
        //                        KLMua = int.Parse(arrData.ElementAt(0).Replace(",", "").Replace(".", "")),
        //                        KLBan = int.Parse(arrData.ElementAt(1).Replace(",", "").Replace(".", "")),
        //                        GiaTriMua = int.Parse(arrData.ElementAt(2).Replace(",", "").Replace(".", "")),
        //                        GiaTriBan = int.Parse(arrData.ElementAt(3).Replace(",", "").Replace(".", "")),
        //                    };
        //                    lData.Add(model);
        //                }
        //                else
        //                {
        //                    localModel.STT = 0;
        //                    localModel.MaCK = string.Empty;
        //                }
        //            }
        //        }
        //        catch(Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }

        //    var tmp1 = 1;
        //}
    }
}