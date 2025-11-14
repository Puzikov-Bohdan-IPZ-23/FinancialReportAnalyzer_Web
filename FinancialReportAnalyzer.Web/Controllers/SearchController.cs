using FinancialReportAnalyzer.Web.Data;
using FinancialReportAnalyzer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialReportAnalyzer.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly ReportDbContext _context;

        public SearchController(ReportDbContext context)
        {
            _context = context;
        }

        // Головний метод, який приймає фільтри з форми і повертає результати
        [HttpGet] // Важливо, щоб форма працювала через GET
        public async Task<IActionResult> Index(SearchViewModel model)
        {
            // 1. Починаємо з базового запиту.
            // 2c.iv: Одразу робимо два JOIN (Include)
            var query = _context.SavedReports
                                .Include(s => s.Company)  // JOIN 1
                                .Include(s => s.ReportType) // JOIN 2
                                .AsQueryable(); // .AsQueryable() дозволяє нам додавати .Where()

            // --- Динамічно будуємо запит на основі фільтрів ---

            // 2c.i: Пошук по даті (діапазон)
            if (model.StartDate.HasValue)
            {
                query = query.Where(s => s.AnalyzedAt.Date >= model.StartDate.Value.Date);
            }
            if (model.EndDate.HasValue)
            {
                // Беремо кінець дня для EndDate
                var endDate = model.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(s => s.AnalyzedAt <= endDate);
            }

            // 2c.iii: Пошук по початку рядка (центральна таблиця SavedReport.Title)
            if (!string.IsNullOrEmpty(model.TitleStartsWith))
            {
                query = query.Where(s => s.Title.StartsWith(model.TitleStartsWith));
            }

            // 2c.iii: Пошук по кінцю рядка (залежна таблиця Company.Name)
            // 2c.iv: Тут ми шукаємо по залежній таблиці
            if (!string.IsNullOrEmpty(model.CompanyNameEndsWith))
            {
                query = query.Where(s => s.Company.Name.EndsWith(model.CompanyNameEndsWith));
            }

            // 2c.ii: Пошук по списку елементів (залежна таблиця Company)
            if (model.SelectedCompanyIds != null && model.SelectedCompanyIds.Any())
            {
                query = query.Where(s => model.SelectedCompanyIds.Contains(s.CompanyId));
            }

            // 2. Виконуємо запит до БД і отримуємо результати
            model.Results = await query.ToListAsync();

            // 3. Готуємо дані для форми (щоб заповнити список компаній)
            await PrepareSearchForm(model);

            return View(model);
        }

        // Допоміжний метод для завантаження довідника компаній у форму
        private async Task PrepareSearchForm(SearchViewModel model)
        {
            var companies = await _context.Companies.OrderBy(c => c.Name).ToListAsync();
            // Передаємо список вибраних ID, щоб форма відновила вибір після відправки
            model.CompanyOptions = new SelectList(companies, "Id", "Name", model.SelectedCompanyIds);
        }
    }
}