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

            // Register a user
            Console.WriteLine(client.Register("Anar", "1234"));

            // Login
            Console.WriteLine(client.Login("Anar", "1234"));

            // Top up balance
            Console.WriteLine(client.TopUp("Anar", 1000));

            // Check balance
            Console.WriteLine(client.GetBalance("Anar"));

            // Buy USD
            Console.WriteLine(client.BuyCurrency("Anar", "USD", 100));

            // Check balance again
            Console.WriteLine(client.GetBalance("Anar"));

            // Sell USD
            Console.WriteLine(client.SellCurrency("Anar", "USD", 50));

            // Transaction history
            Console.WriteLine(client.GetTransactionHistory("Anar"));

            Console.ReadKey();
        }
    }
}