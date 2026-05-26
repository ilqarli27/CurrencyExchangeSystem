using System;
using ConsoleClient.ServiceReference1;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Service1Client client = new Service1Client();

            // Test SayHello
            Console.WriteLine(client.SayHello("Anar"));

            // Test GetRate
            Console.WriteLine(client.GetRate("USD"));
            Console.WriteLine(client.GetRate("EUR"));

            // Test GetAllRates
            Console.WriteLine(client.GetAllRates());

            // Test GetHistoricalRates
            Console.WriteLine(client.GetHistoricalRates("USD", "2024-01-01", "2024-01-10"));

            Console.ReadKey();
        }
    }
}