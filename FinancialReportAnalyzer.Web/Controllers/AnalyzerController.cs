using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // <-- ВАЖЛИВО: Вимога 1d
using FinancialReportAnalyzer.Web.Services;
using System.Threading.Tasks;

namespace FinancialReportAnalyzer.Web.Controllers
{
    [Authorize] // <--- ЗАХИЩАЄ ВСІ СТОРІНКИ В ЦЬОМУ КОНТРОЛЕРІ
    public class AnalyzerController : Controller
    {
        // Підключаємо наші сервіси
        private readonly ReportAnalyzer _analyzer;
        private readonly PdfReportLoader _loader;

        public AnalyzerController()
        {
            _analyzer = new ReportAnalyzer();
            _loader = new PdfReportLoader();
        }

        // --- Підпрограма 1: Аналіз з файлу ---
        [HttpGet]
        public IActionResult AnalyzeFile()
        {
            ViewData["Title"] = "Аналіз PDF файлу";
            ViewData["Description"] = "Ця сторінка дозволяє завантажити PDF-документ. Система проаналізує його та витягне ключові фінансові показники.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeFile(IFormFile reportFile)
        {
            if (reportFile == null || reportFile.Length == 0)
            {
                ViewBag.Error = "Будь ласка, виберіть файл.";
                return View();
            }

            string text;
            await using (var stream = reportFile.OpenReadStream())
            {
                text = await _loader.LoadTextFromPdfStreamAsync(stream);
            }

            var results = _analyzer.Analyze(text);

            // Передаємо результати на сторінку "AnalyzeResult"
            return View("AnalyzeResult", results);
        }

        // --- Підпрограма 2: Аналіз з тексту ---
        [HttpGet]
        public IActionResult AnalyzeText()
        {
            ViewData["Title"] = "Аналіз тексту";
            ViewData["Description"] = "Ця сторінка дозволяє вставити текст звіту. Система проаналізує його та витягне ключові фінансові показники.";
            return View();
        }

        [HttpPost]
        public IActionResult AnalyzeText(string reportText)
        {
            if (string.IsNullOrWhiteSpace(reportText))
            {
                ViewBag.Error = "Будь ласка, введіть текст.";
                return View();
            }

            var results = _analyzer.Analyze(reportText);
            return View("AnalyzeResult", results);
        }

        // --- Підпрограма 3: Сторінка опису (наприклад, "Історія") ---
        [HttpGet]
        public IActionResult History()
        {
            ViewData["Title"] = "Історія аналізів";
            ViewData["Description"] = "На цій сторінці (в майбутньому) буде відображатися історія ваших попередніх аналізів.";
            return View();
        }
    }
}