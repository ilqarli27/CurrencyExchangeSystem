using System;
using ConsoleClient.ServiceReference1;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Service1Client client = new Service1Client();

            string mesaj = client.SayHello("Anar");
            Console.WriteLine(mesaj);

            string usd = client.GetRate("USD");
            Console.WriteLine(usd);

            string eur = client.GetRate("EUR");
            Console.WriteLine(eur);

            Console.ReadKey();
        }
    }
}