using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace HelloWorld.ConsoleApp
{
    class Program
    {
        private readonly Sp500Repository _sp500Repository = new Sp500Repository();

        static int Main()
        {
            Console.WriteLine("Hello World!");

            try
            {
                new Program().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application error: {ex}");
                return -1;
            }
        }

        public void Run()
        {
            var stocks = _sp500Repository.GetStocks().ToArray();
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
