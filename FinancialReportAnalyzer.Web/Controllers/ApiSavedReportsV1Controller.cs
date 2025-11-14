using FinancialReportAnalyzer.Web.Data;
using FinancialReportAnalyzer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancialReportAnalyzer.Web.Controllers
{
    [ApiVersion("1.0")] // <-- Вказуємо, що це V1
    [Route("api/v{version:apiVersion}/ApiSavedReports")] // <-- Роут буде /api/v1/ApiSavedReports
    [ApiController]
    public class ApiSavedReportsV1Controller : ControllerBase
    {
        private readonly ReportDbContext _context;

        public ApiSavedReportsV1Controller(ReportDbContext context)
        {
            _context = context;
        }

        // GET: /api/v1/ApiSavedReports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SavedReport>>> GetSavedReports()
        {
            // ВЕРСІЯ 1: Повертає повні вкладені об'єкти (сумісність)
            return await _context.SavedReports
                                 .Include(s => s.Company)
                                 .Include(s => s.ReportType)
                                 .ToListAsync();
        }

        // GET: /api/v1/ApiSavedReports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SavedReport>> GetSavedReport(int id)
        {
            var savedReport = await _context.SavedReports
                                            .Include(s => s.Company)
                                            .Include(s => s.ReportType)
                                            .FirstOrDefaultAsync(s => s.Id == id);

            if (savedReport == null)
            {
                return NotFound();
            }

            return savedReport;
        }
    }
}