using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinancialReportAnalyzer.Web.Data;
using FinancialReportAnalyzer.Web.Models;

namespace FinancialReportAnalyzer.Web.Controllers
{
    public class SavedReportsController : Controller
    {
        private readonly ReportDbContext _context;

        public SavedReportsController(ReportDbContext context)
        {
            _context = context;
        }

        // GET: SavedReports
        public async Task<IActionResult> Index()
        {
            // Виконуємо JOIN (Include) до Company і ReportType
            var reportDbContext = _context.SavedReports.Include(s => s.Company).Include(s => s.ReportType);
            return View(await reportDbContext.ToListAsync());
        }

        // GET: SavedReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Виконуємо JOIN (Include) до Company і ReportType
            var savedReport = await _context.SavedReports
                .Include(s => s.Company)
                .Include(s => s.ReportType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (savedReport == null)
            {
                return NotFound();
            }

            return View(savedReport);
        }

        // GET: SavedReports/Create
        public IActionResult Create()
        {
            // --- ВИПРАВЛЕНО ---
            // Тепер у випадаючому списку буде "Name" і "TypeName"
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "TypeName");
            return View();
        }

        // POST: SavedReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,AnalyzedAt,CompanyId,ReportTypeId")] SavedReport savedReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(savedReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // --- ВИПРАВЛЕНО ---
            // Якщо валідація не пройде, список заповниться коректно
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", savedReport.CompanyId);
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "TypeName", savedReport.ReportTypeId);
            return View(savedReport);
        }

        // GET: SavedReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedReport = await _context.SavedReports.FindAsync(id);
            if (savedReport == null)
            {
                return NotFound();
            }

            // --- ВИПРАВЛЕНО ---
            // Тепер у випадаючому списку буде "Name" і "TypeName"
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", savedReport.CompanyId);
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "TypeName", savedReport.ReportTypeId);
            return View(savedReport);
        }

        // POST: SavedReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,AnalyzedAt,CompanyId,ReportTypeId")] SavedReport savedReport)
        {
            if (id != savedReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(savedReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavedReportExists(savedReport.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // --- ВИПРАВЛЕНО ---
            // Якщо валідація не пройде, список заповниться коректно
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", savedReport.CompanyId);
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "TypeName", savedReport.ReportTypeId);
            return View(savedReport);
        }

        // GET: SavedReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Виконуємо JOIN (Include) до Company і ReportType
            var savedReport = await _context.SavedReports
                .Include(s => s.Company)
                .Include(s => s.ReportType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (savedReport == null)
            {
                return NotFound();
            }

            return View(savedReport);
        }

        // POST: SavedReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var savedReport = await _context.SavedReports.FindAsync(id);
            if (savedReport != null)
            {
                _context.SavedReports.Remove(savedReport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavedReportExists(int id)
        {
            return _context.SavedReports.Any(e => e.Id == id);
        }
    }
}