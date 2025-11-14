using FinancialReportAnalyzer.Web.Data;
using FinancialReportAnalyzer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq; // Потрібен для .Select
using System.Threading.Tasks;

namespace FinancialReportAnalyzer.Web.Controllers
{
    [ApiVersion("2.0")] // <-- Вказуємо, що це V2
    [Route("api/v{version:apiVersion}/ApiSavedReports")] // <-- Роут /api/v2/ApiSavedReports
    [ApiController]
    public class ApiSavedReportsV2Controller : ControllerBase
    {
        private readonly ReportDbContext _context;

        public ApiSavedReportsV2Controller(ReportDbContext context)
        {
            _context = context;
        }

        // GET: /api/v2/ApiSavedReports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SavedReportDto>>> GetSavedReports()
        {
            // ВЕРСІЯ 2: Повертає "пласку" DTO
            return await _context.SavedReports
                .Include(s => s.Company)
                .Include(s => s.ReportType)
                .Select(s => new SavedReportDto // Проектуємо дані у DTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    AnalyzedAt = s.AnalyzedAt,
                    CompanyName = s.Company.Name,
                    Industry = s.Company.Industry,
                    ReportTypeName = s.ReportType.TypeName
                })
                .ToListAsync();
        }

        // GET: /api/v2/ApiSavedReports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SavedReportDto>> GetSavedReport(int id)
        {
            var savedReportDto = await _context.SavedReports
               .Where(s => s.Id == id)
               .Include(s => s.Company)
               .Include(s => s.ReportType)
               .Select(s => new SavedReportDto
               {
                   Id = s.Id,
                   Title = s.Title,
                   AnalyzedAt = s.AnalyzedAt,
                   CompanyName = s.Company.Name,
                   Industry = s.Company.Industry,
                   ReportTypeName = s.ReportType.TypeName
               })
               .FirstOrDefaultAsync();

            if (savedReportDto == null)
            {
                return NotFound();
            }

            return savedReportDto;
        }
    }
}