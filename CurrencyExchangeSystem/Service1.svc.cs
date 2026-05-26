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

        public string GetAllRates()
        {
            try
            {
                string url = "http://api.nbp.pl/api/exchangerates/tables/a/?format=json";

                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string json = client.DownloadString(url);

                string result = "All exchange rates (PLN):\n";
                int pos = 0;

                while (true)
                {
                    int codeIdx = json.IndexOf("\"code\":", pos);
                    if (codeIdx == -1) break;

                    string afterCode = json.Substring(codeIdx + 8);
                    string code = afterCode.Split('"')[0];

                    int midIdx = json.IndexOf("\"mid\":", codeIdx);
                    string afterMid = json.Substring(midIdx + 6);
                    string mid = afterMid.Split(',')[0].Split('}')[0].Trim();

                    result += code + " = " + mid + " PLN\n";
                    pos = midIdx + 6;
                }

                return result;
            }
            catch
            {
                return "Error: could not retrieve rates.";
            }
        }

        public string GetHistoricalRates(string currencyCode, string startDate, string endDate)
        {
            try
            {
                string url = "http://api.nbp.pl/api/exchangerates/rates/a/"
                             + currencyCode.ToUpper() + "/" + startDate + "/" + endDate + "/?format=json";

                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string json = client.DownloadString(url);

                string result = currencyCode.ToUpper() + " historical rates:\n";
                int pos = 0;

                while (true)
                {
                    int dateIdx = json.IndexOf("\"effectiveDate\":", pos);
                    if (dateIdx == -1) break;

                    string afterDate = json.Substring(dateIdx + 17);
                    string date = afterDate.Split('"')[0];

                    int midIdx = json.IndexOf("\"mid\":", dateIdx);
                    string afterMid = json.Substring(midIdx + 6);
                    string mid = afterMid.Split(',')[0].Split('}')[0].Trim();

                    result += date + " = " + mid + " PLN\n";
                    pos = midIdx + 6;
                }

                return result;
            }
            catch
            {
                return "Error: could not retrieve historical rates for " + currencyCode;
            }
        }
    }
}