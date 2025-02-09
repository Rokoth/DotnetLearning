using Newtonsoft.Json;
using System.Linq;
using System.Text.Json.Serialization;

namespace Lesson2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var reader = new StreamReader("Data\\JSON_sample_1.json");
            var json = reader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<List<Deal>>(json);

            var numbers = GetNumbersOfDeals(data);
            var sums = GetSumsByMonth(data);

            Console.WriteLine($"numbers: {string.Join(", ", numbers)}");
            Console.WriteLine("sums: ");
            foreach (var sum in sums)
            {
                Console.WriteLine($"{sum.Month:yyyy-MM} : {sum.Sum}");
            }
        }

        private static IList<string> GetNumbersOfDeals(IEnumerable<Deal> deals)
        {
           return deals
                .Where(s=>s.Sum >= 100)
                .OrderBy(s=>s.Date)
                .Take(5)
                .OrderByDescending(s=>s.Sum)
                .Select(s=>s.Id)
                .ToList();
        }

        private static IList<SumByMonth> GetSumsByMonth(IEnumerable<Deal> deals)
        {
            return deals
                .GroupBy(s => new DateTime(s.Date.Year, s.Date.Month, 1))
                .Select(s => new SumByMonth(s.Key, s.Sum(f => f.Sum))).ToList();
        }
    }


    record SumByMonth(DateTime Month, int Sum);

    public class Deal
    {
        public int Sum { get; set; }
        public string Id { get; set; }
        public DateTime Date { get; set; }
    }
}