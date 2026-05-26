namespace CurrencyExchangeSystem
{
    public class Service1 : IService1
    {
        public string SayHello(string name)
        {
            return $"Hello, {name}! Service is working.";
        }
    }
}