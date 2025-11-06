namespace FinancialReportAnalyzer.Web.Models
{
    // Довідник 1: Компанії
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Industry { get; set; } // Галузь
    }
}