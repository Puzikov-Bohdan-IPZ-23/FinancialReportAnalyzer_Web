using System;

namespace FinancialReportAnalyzer.Web.Models
{
    // Це "пласка" модель (DTO) для відповіді API v2
    // Вона ефективніша, бо не повертає вкладені об'єкти
    public class SavedReportDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime AnalyzedAt { get; set; }

        // v2 повертає лише потрібні нам рядки
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string ReportTypeName { get; set; }
    }
}