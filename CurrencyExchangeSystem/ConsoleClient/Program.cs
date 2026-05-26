using System;
using ConsoleClient.ServiceReference1;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Service1Client client = new Service1Client();
            Console.WriteLine(client.GetRate("USD"));
            Console.WriteLine(client.GetRate("EUR"));

            
            Console.WriteLine(client.Register("Anar", "1234"));

            
            Console.WriteLine(client.Login("Anar", "1234"));

            
            Console.WriteLine(client.TopUp("Anar", 1000));

            
            Console.WriteLine(client.GetBalance("Anar"));

            
            Console.WriteLine(client.BuyCurrency("Anar", "USD", 100));

            
            Console.WriteLine(client.GetBalance("Anar"));

            
            Console.WriteLine(client.SellCurrency("Anar", "USD", 50));

            
            Console.WriteLine(client.GetTransactionHistory("Anar"));

            Console.WriteLine(client.GetHistoricalRates("USD", "2024-01-01", "2024-01-31"));
            Console.WriteLine(client.GetHistoricalRates("USD", "2024-01-31", "2024-01-01")); 
            Console.WriteLine(client.GetHistoricalRates("USD", "2024-01-01", "2024-06-01")); 

            Console.ReadKey();
        }
    }
}