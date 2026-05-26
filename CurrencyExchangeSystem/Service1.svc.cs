using System.Net;

namespace CurrencyExchangeSystem
{
    public class Service1 : IService1
    {
        public string SayHello(string name)
        {
            return $"Hello, {name}! Service is working.";
        }

        public string GetRate(string currencyCode)
        {
            try
            {
                string url = "http://api.nbp.pl/api/exchangerates/rates/a/"
                             + currencyCode.ToUpper() + "/?format=json";

                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string json = client.DownloadString(url);

                int idx = json.IndexOf("\"mid\":");
                string after = json.Substring(idx + 6);
                string rate = after.Split(',')[0].Split('}')[0].Trim();

                return currencyCode.ToUpper() + " = " + rate + " PLN";
            }
            catch
            {
                return "Error: currency code " + currencyCode + " not found.";
            }
        }
    }
}