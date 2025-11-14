using FinancialReportAnalyzer.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FinancialReportAnalyzer.Web.Models
{
    // Цей клас "ViewModel" містить і фільтри, і результати пошуку
    public class SearchViewModel
    {
        // --- Фільтри для форми пошуку ---

        // 2c.i: Пошук по даті
        [Display(Name = "Дата аналізу (Від)")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Дата аналізу (До)")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        // 2c.iii: Пошук по початку рядка (для центральної таблиці)
        [Display(Name = "Назва звіту починається з...")]
        public string? TitleStartsWith { get; set; }

        // 2c.iii: Пошук по кінцю рядка (для залежної таблиці)
        [Display(Name = "Назва компанії закінчується на...")]
        public string? CompanyNameEndsWith { get; set; }

        // 2c.ii: Пошук по списку елементів (з залежної таблиці)
        [Display(Name = "Вибрані компанії")]
        public List<int>? SelectedCompanyIds { get; set; }


        // --- Для заповнення <select> у формі ---
        public SelectList? CompanyOptions { get; set; }


        // --- Результати пошуку ---
        public List<SavedReport> Results { get; set; } = new List<SavedReport>();
    }
}