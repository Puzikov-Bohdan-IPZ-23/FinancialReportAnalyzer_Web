using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinancialReportAnalyzer.Web.Data;
using FinancialReportAnalyzer.Web.Models;

namespace FinancialReportAnalyzer.Web.Controllers
{
    // Атрибут [ApiController] вказує, що це API-контролер
    // Роут буде /api/ApiCompanies (бо ми назвали клас ApiCompaniesController)
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCompaniesController : ControllerBase
    {
        private readonly ReportDbContext _context;

        public ApiCompaniesController(ReportDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiCompanies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            // Цей метод повертає список компаній у форматі JSON
            return await _context.Companies.ToListAsync();
        }

        // GET: api/ApiCompanies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound(); // Повертає 404
            }

            return company; // Повертає 200 OK з об'єктом
        }

        // PUT: api/ApiCompanies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (id != company.Id)
            {
                return BadRequest(); // Повертає 400
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Companies.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Повертає 204
        }

        // POST: api/ApiCompanies
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Повертає 201 Created з посиланням на новий ресурс
            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        // DELETE: api/ApiCompanies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent(); // Повертає 204
        }
    }
}