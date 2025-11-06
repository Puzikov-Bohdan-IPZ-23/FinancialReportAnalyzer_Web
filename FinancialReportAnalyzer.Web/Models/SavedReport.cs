using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialReportAnalyzer.Web.Models
{
    // Центральна таблиця: Збережені звіти
    public class SavedReport
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime AnalyzedAt { get; set; } // (пошук по даті)

        // Ключі для JOIN 
        public int CompanyId { get; set; }
        public int ReportTypeId { get; set; }

        // Властивості навігації (для JOIN в EF)
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        [ForeignKey("ReportTypeId")]
        public virtual ReportType ReportType { get; set; }

        // Зв'язок "один до багатьох" з фінансовими показниками
        public virtual ICollection<FinancialMetric> Metrics { get; set; }
    }
}