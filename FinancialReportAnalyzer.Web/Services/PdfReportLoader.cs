using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Text;
using System.Threading.Tasks;

namespace FinancialReportAnalyzer.Web.Services
{
    // Цей клас трохи змінено для роботи з веб-завантаженнями
    public class PdfReportLoader
    {
        public async Task<string> LoadTextFromPdfStreamAsync(Stream stream)
        {
            var textBuilder = new StringBuilder();
            try
            {
                // Використовуємо асинхронне читання
                using (var pdfDocument = new PdfDocument(new PdfReader(stream)))
                {
                    for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                    {
                        var strategy = new SimpleTextExtractionStrategy();
                        var pageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i), strategy);
                        textBuilder.Append(pageText);
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Помилка читання PDF: {ex.Message}";
            }

            return await Task.FromResult(textBuilder.ToString());
        }
    }
}