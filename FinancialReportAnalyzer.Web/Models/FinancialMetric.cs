using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialReportAnalyzer.Web.Models
{
    // Таблиця для зберігання результатів аналізу (зв'язана з SavedReport)
    public class FinancialMetric
    {
        public int Id { get; set; }
        public string MetricName { get; set; } // "дохід", "витрати", "прибуток"
        public decimal MetricValue { get; set; } // 1500000

        // Ключ до центральної таблиці
        public int SavedReportId { get; set; }
        [ForeignKey("SavedReportId")]
        public virtual SavedReport SavedReport { get; set; }
    }
}