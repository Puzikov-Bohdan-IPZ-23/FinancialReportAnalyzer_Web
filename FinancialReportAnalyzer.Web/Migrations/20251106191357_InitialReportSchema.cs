using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinancialReportAnalyzer.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialReportSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Industry = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SavedReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnalyzedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ReportTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedReports_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedReports_ReportTypes_ReportTypeId",
                        column: x => x.ReportTypeId,
                        principalTable: "ReportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetricName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetricValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SavedReportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialMetrics_SavedReports_SavedReportId",
                        column: x => x.SavedReportId,
                        principalTable: "SavedReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Industry", "Name" },
                values: new object[,]
                {
                    { 1, "IT", "Техносфера" },
                    { 2, "Агро", "Агрохолдинг 'Степи України'" },
                    { 3, "Промисловість", "Металург" }
                });

            migrationBuilder.InsertData(
                table: "ReportTypes",
                columns: new[] { "Id", "TypeName" },
                values: new object[,]
                {
                    { 1, "Квартальний (Q1)" },
                    { 2, "Квартальний (Q2)" },
                    { 3, "Річний" },
                    { 4, "Півріччя" }
                });

            migrationBuilder.InsertData(
                table: "SavedReports",
                columns: new[] { "Id", "AnalyzedAt", "CompanyId", "ReportTypeId", "Title" },
                values: new object[] { 1, new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Utc), 1, 4, "Звіт 'Техносфера' за півріччя 2025" });

            migrationBuilder.InsertData(
                table: "FinancialMetrics",
                columns: new[] { "Id", "MetricName", "MetricValue", "SavedReportId" },
                values: new object[,]
                {
                    { 1, "дохід", 1500000m, 1 },
                    { 2, "витрати", 950000m, 1 },
                    { 3, "прибуток", 500000m, 1 },
                    { 4, "збиток", 50000m, 1 },
                    { 5, "активи", 2300000m, 1 },
                    { 6, "пасиви", 800000m, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialMetrics_SavedReportId",
                table: "FinancialMetrics",
                column: "SavedReportId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedReports_CompanyId",
                table: "SavedReports",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedReports_ReportTypeId",
                table: "SavedReports",
                column: "ReportTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialMetrics");

            migrationBuilder.DropTable(
                name: "SavedReports");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "ReportTypes");
        }
    }
}
