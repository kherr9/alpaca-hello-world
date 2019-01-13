using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Text;

namespace HelloWorld.ConsoleApp
{
    class Program
    {
        private readonly Sp500Repository _sp500Repository = new Sp500Repository();

        static async Task<int> Main()
        {
            Console.WriteLine("Hello World!");

            try
            {
                await new Program().RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application error: {ex}");
                return -1;
            }
        }

        public async Task RunAsync()
        {
            var client = new Alpaca.Markets.RestClient(
                "TODO", "TODO", "https://paper-api.alpaca.markets");

            ////var clock = await client.GetClockAsync();
            ////var now = clock.Timestamp;
            ////if (clock.IsOpen)
            ////{
            ////    client.GetBarSetAsync()
            ////}

            var symbols = _sp500Repository.GetStocks().Select(x => x.Symbol).ToList();

            await GetPricesAsync(symbols);
        }

        private Task<object> GetPricesAsync(ICollection<string> symbols)
        {
            // new york time
            var now = NodaTimeUtility.Now;
            var endDate = now;

            if (now >= NodaTimeUtility.Today930AM)
            {
                endDate = NodaTimeUtility.LastMinuteOfYesterday;
            }

            return GetPricesAsync(symbols, endDate);
        }

        private Task<object> GetPricesAsync(ICollection<string> symbols, DateTimeOffset endDate)
        {
            throw new NotImplementedException();
        }
    }

    public class NodaTimeUtility
    {
        private static readonly DateTimeZone NewYorkStockExchangeTimeZone = DateTimeZoneProviders.Tzdb["EST"];

        public static DateTimeOffset Now =>
            SystemClock.Instance.GetCurrentInstant()
                .InZone(NewYorkStockExchangeTimeZone)
                .ToDateTimeOffset();

        public static DateTimeOffset Today930AM
        {
            get
            {
                // your inputs
                string time = "9:30am";

                // parse the time string using Noda Time's pattern API
                var pattern = LocalTimePattern.CreateWithCurrentCulture("h:mmtt");
                var parseResult = pattern.Parse(time);
                if (!parseResult.Success)
                {
                    throw new Exception("Failed to par");
                    // handle parse failure
                }
                var localTime = parseResult.Value;

                // get the current date in the target time zone
                var clock = SystemClock.Instance;
                var now = clock.GetCurrentInstant();
                var today = now.InZone(NewYorkStockExchangeTimeZone).Date;

                // combine the date and time
                var ldt = today.At(localTime);

                // bind it to the time zone
                return ldt.InZoneLeniently(NewYorkStockExchangeTimeZone)
                    .ToDateTimeOffset();
            }
        }

        public static DateTimeOffset LastMinuteOfYesterday
        {
            get
            {
                // your inputs
                string time = "12:59pm";

                // parse the time string using Noda Time's pattern API
                var pattern = LocalTimePattern.CreateWithCurrentCulture("h:mmtt");
                var parseResult = pattern.Parse(time);
                if (!parseResult.Success)
                {
                    throw new Exception("Failed to par");
                    // handle parse failure
                }
                var localTime = parseResult.Value;

                // get the current date in the target time zone
                var clock = SystemClock.Instance;
                var now = clock.GetCurrentInstant();
                var today = now.InZone(NewYorkStockExchangeTimeZone).Date;

                // combine the date and time
                var ldt = today.At(localTime);

                // bind it to the time zone
                return ldt.InZoneLeniently(NewYorkStockExchangeTimeZone)
                    .Minus(Duration.FromDays(1))
                    .ToDateTimeOffset();
            }
        }
    }

    public class Sp500Repository
    {
        // https://raw.githubusercontent.com/datasets/s-and-p-500-companies/master/data/constituents.csv
        public IEnumerable<Stock> GetStocks()
        {
            var lines = System.IO.File.ReadAllLines("sp500.csv")
                .Skip(1);

            foreach (var line in lines)
            {
                var cells = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

                yield return new Stock(
                    symbol: cells[0],
                    name: cells[1]);
            }
        }
    }

    public class Stock
    {
        public Stock(string symbol, string name)
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Symbol { get; }

        public string Name { get; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
