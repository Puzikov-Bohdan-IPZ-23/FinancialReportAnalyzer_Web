using Microsoft.AspNetCore.Identity;

namespace FinancialReportAnalyzer.Web.Data
{
    public class ApplicationUser : IdentityUser
    {
        // Вимога 2b: Додаємо поле "Повне ім'я"
        public string? FullName { get; set; }
    }
}