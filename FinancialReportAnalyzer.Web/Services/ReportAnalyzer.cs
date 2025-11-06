using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FinancialReportAnalyzer.Web.Services
{
    //  той самий клас з Лабораторної 1
    public class ReportAnalyzer
    {
        private readonly List<string> _keywords;

        public ReportAnalyzer()
        {
            _keywords = new List<string>
            {
                "дохід",
                "витрати",
                "прибуток",
                "збиток",
                "активи",
                "пасиви"
            };
        }

        public Dictionary<string, decimal> Analyze(string text)
        {
            var results = new Dictionary<string, decimal>();
            if (string.IsNullOrWhiteSpace(text))
            {
                return results;
            }

            foreach (var keyword in _keywords)
            {
                // Покращений Regex для пошуку чисел (включаючи пробіли і коми)
                var pattern = new Regex($@"{keyword}[^\d]*([\d\s,.]+)");
                var match = pattern.Match(text);

                if (match.Success)
                {
                    string numberAsString = match.Groups[1].Value;
                    // Очищуємо рядок від усього, крім цифр та десяткових роздільників
                    string cleanedNumber = Regex.Replace(numberAsString, @"[^\d,.]", "");

                    // Замінюємо кому на крапку для коректного парсингу
                    cleanedNumber = cleanedNumber.Replace(',', '.');

                    if (decimal.TryParse(cleanedNumber, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value))
                    {
                        results[keyword] = value;
                    }
                }
            }
            return results;
        }
    }
}
