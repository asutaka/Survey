using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task BCTCRead(string path)
        {
            try
            {
                var stream = await _apiService.BCTCRead(path);
                if (stream is null)
                    return;
                var text = pdfText(stream);
                var tmp = 1;

            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.BCTCRead|EXCEPTION| {ex.Message}");
            }
        }

        private string pdfText(Stream data)
        {
            var reader = new PdfReader(data);

            string text = string.Empty;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += PdfTextExtractor.GetTextFromPage(reader, page);
                text += "\n";
            }

            reader.Close();
            return text;
        }
    }
}
