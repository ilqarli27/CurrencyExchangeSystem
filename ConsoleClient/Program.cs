using ConsoleClient.ServiceReference1;
using System;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Service1Client client = new Service1Client();

            string mesaj = client.SayHello("Ahmet");
            Console.WriteLine(mesaj);

            Console.ReadKey();
        }
    }
}