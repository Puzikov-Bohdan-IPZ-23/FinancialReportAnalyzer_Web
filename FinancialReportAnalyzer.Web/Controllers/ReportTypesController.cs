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
    public class ReportTypesController : Controller
    {
        private readonly ReportDbContext _context;

        public ReportTypesController(ReportDbContext context)
        {
            _context = context;
        }

        // GET: ReportTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ReportTypes.ToListAsync());
        }

        // GET: ReportTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportType = await _context.ReportTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reportType == null)
            {
                return NotFound();
            }

            return View(reportType);
        }

        // GET: ReportTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReportTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TypeName")] ReportType reportType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reportType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reportType);
        }

        // GET: ReportTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportType = await _context.ReportTypes.FindAsync(id);
            if (reportType == null)
            {
                return NotFound();
            }
            return View(reportType);
        }

        // POST: ReportTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TypeName")] ReportType reportType)
        {
            if (id != reportType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reportType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportTypeExists(reportType.Id))
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
            return View(reportType);
        }

        // GET: ReportTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportType = await _context.ReportTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reportType == null)
            {
                return NotFound();
            }

            return View(reportType);
        }

        // POST: ReportTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reportType = await _context.ReportTypes.FindAsync(id);
            if (reportType != null)
            {
                _context.ReportTypes.Remove(reportType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportTypeExists(int id)
        {
            return _context.ReportTypes.Any(e => e.Id == id);
        }
    }
}
