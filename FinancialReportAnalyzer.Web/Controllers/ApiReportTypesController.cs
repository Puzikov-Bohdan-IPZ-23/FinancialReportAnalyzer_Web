using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinancialReportAnalyzer.Web.Data;
using FinancialReportAnalyzer.Web.Models;

namespace FinancialReportAnalyzer.Web.Controllers
{
    [Route("api/[controller]")] // Роут буде /api/ApiReportTypes
    [ApiController]
    public class ApiReportTypesController : ControllerBase
    {
        private readonly ReportDbContext _context;

        public ApiReportTypesController(ReportDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiReportTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportType>>> GetReportTypes()
        {
            return await _context.ReportTypes.ToListAsync();
        }

        // GET: api/ApiReportTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportType>> GetReportType(int id)
        {
            var reportType = await _context.ReportTypes.FindAsync(id);

            if (reportType == null)
            {
                return NotFound();
            }

            return reportType;
        }
    }
}