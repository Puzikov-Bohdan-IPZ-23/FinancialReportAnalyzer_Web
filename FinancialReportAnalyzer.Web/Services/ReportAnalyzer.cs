using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FinancialReportAnalyzer.Web.Services
{
    // той самий клас з Лабораторної 1
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

            // Ми робимо текст у нижньому регістрі ОДИН РАЗ, щоб пошук був нечутливим
            var lowerText = text.ToLowerInvariant();

            foreach (var keyword in _keywords)
            {
               
                var pattern = new Regex($@"{keyword}[^\d]*([\d\s,.]+)");
                var matches = pattern.Matches(lowerText);

                if (matches.Count > 0)
                {
                    // Беремо ОСТАННЄ збіг для слова 
                    var match = matches[matches.Count - 1];
                    string numberAsString = match.Groups[1].Value;

               
                    string cleanedNumber = Regex.Replace(numberAsString, @"[,\.\s]", "");

                    // 2. Спробуємо розпарсити як ціле число
                    if (decimal.TryParse(cleanedNumber, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value))
                    {
                        // Якщо такого ключа ще немає, або ми знайшли нове значення
                        results[keyword] = value;
                    }
                }
            }
            return results;
        }
    }
}