using Microsoft.EntityFrameworkCore;
using FinancialReportAnalyzer.Web.Models; // Підключаємо нові моделі

namespace FinancialReportAnalyzer.Web.Data
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
        {
        }

        // Реєструємо наші моделі як таблиці
        public DbSet<Company> Companies { get; set; }
        public DbSet<ReportType> ReportTypes { get; set; }
        public DbSet<SavedReport> SavedReports { get; set; }
        public DbSet<FinancialMetric> FinancialMetrics { get; set; }

        // Заповнення початковими даними (Вимога 1)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FinancialMetric>(entity =>
            {
                entity.Property(e => e.MetricValue).HasColumnType("decimal(18, 2)");
            });

            // Довідник компаній
            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Техносфера", Industry = "IT" },
                new Company { Id = 2, Name = "Агрохолдинг 'Степи України'", Industry = "Агро" },
                new Company { Id = 3, Name = "Металург", Industry = "Промисловість" }
            );

            // Довідник типів звітів
            modelBuilder.Entity<ReportType>().HasData(
                new ReportType { Id = 1, TypeName = "Квартальний (Q1)" },
                new ReportType { Id = 2, TypeName = "Квартальний (Q2)" },
                new ReportType { Id = 3, TypeName = "Річний" },
                new ReportType { Id = 4, TypeName = "Півріччя" }
            );

            // Приклад центральної таблиці
            modelBuilder.Entity<SavedReport>().HasData(
                new SavedReport
                {
                    Id = 1,
                    Title = "Звіт 'Техносфера' за півріччя 2025",

                    
                    AnalyzedAt = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc),

                    CompanyId = 1, // 'Техносфера'
                    ReportTypeId = 4 // 'Півріччя'
                }
            );

            // Приклад фінансових показників 
            modelBuilder.Entity<FinancialMetric>().HasData(
                new FinancialMetric { Id = 1, SavedReportId = 1, MetricName = "дохід", MetricValue = 1500000 },
                new FinancialMetric { Id = 2, SavedReportId = 1, MetricName = "витрати", MetricValue = 950000 },
                new FinancialMetric { Id = 3, SavedReportId = 1, MetricName = "прибуток", MetricValue = 500000 },
                new FinancialMetric { Id = 4, SavedReportId = 1, MetricName = "збиток", MetricValue = 50000 },
                new FinancialMetric { Id = 5, SavedReportId = 1, MetricName = "активи", MetricValue = 2300000 },
                new FinancialMetric { Id = 6, SavedReportId = 1, MetricName = "пасиви", MetricValue = 800000 }
            );
        }
    }
}