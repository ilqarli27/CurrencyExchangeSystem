using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CurrencyExchangeSystem
{
    public class Service1 : IService1
    {
        // In-memory storage
        private static Dictionary<string, string> users = new Dictionary<string, string>();
        private static Dictionary<string, decimal> plnBalances = new Dictionary<string, decimal>();
        private static Dictionary<string, Dictionary<string, decimal>> wallets = new Dictionary<string, Dictionary<string, decimal>>();
        private static List<string> transactions = new List<string>();

        // --- Rates ---
        public string GetRate(string currencyCode)
        {
            try
            {
                string url = "http://api.nbp.pl/api/exchangerates/rates/a/"
                             + currencyCode.ToUpper() + "/?format=json";
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
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
                client.Encoding = Encoding.UTF8;
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
                client.Encoding = Encoding.UTF8;
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

        // --- Users ---
        public string Register(string username, string password)
        {
            if (users.ContainsKey(username))
                return "Error: username already exists.";

            users[username] = password;
            plnBalances[username] = 0;
            wallets[username] = new Dictionary<string, decimal>();
            return "Success: user " + username + " registered.";
        }

        public string Login(string username, string password)
        {
            if (!users.ContainsKey(username))
                return "Error: user not found.";
            if (users[username] != password)
                return "Error: wrong password.";
            return "Success: welcome " + username + "!";
        }

        // --- Account ---
        public string TopUp(string username, decimal amount)
        {
            if (!users.ContainsKey(username))
                return "Error: user not found.";

            plnBalances[username] += amount;
            transactions.Add(username + " | TOPUP | " + amount + " PLN | " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            return "Success: balance is now " + plnBalances[username] + " PLN.";
        }

        public string GetBalance(string username)
        {
            if (!users.ContainsKey(username))
                return "Error: user not found.";

            string result = "PLN balance: " + plnBalances[username] + "\n";
            foreach (var w in wallets[username])
                result += w.Key + ": " + w.Value + "\n";
            return result;
        }

        // --- Exchange ---
        public string BuyCurrency(string username, string currencyCode, decimal amount)
        {
            if (!users.ContainsKey(username))
                return "Error: user not found.";

            try
            {
                string url = "http://api.nbp.pl/api/exchangerates/rates/a/"
                             + currencyCode.ToUpper() + "/?format=json";
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(url);

                int idx = json.IndexOf("\"mid\":");
                string after = json.Substring(idx + 6);
                decimal mid = decimal.Parse(after.Split(',')[0].Split('}')[0].Trim(),
                    System.Globalization.CultureInfo.InvariantCulture);

                decimal askRate = Math.Round(mid * 1.01m, 4);
                decimal cost = Math.Round(amount * askRate, 2);

                if (plnBalances[username] < cost)
                    return "Error: not enough PLN balance. Need " + cost + " PLN.";

                plnBalances[username] -= cost;

                if (!wallets[username].ContainsKey(currencyCode.ToUpper()))
                    wallets[username][currencyCode.ToUpper()] = 0;
                wallets[username][currencyCode.ToUpper()] += amount;

                transactions.Add(username + " | BUY | " + amount + " " + currencyCode.ToUpper()
                    + " | rate: " + askRate + " | cost: " + cost + " PLN | " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                return "Success: bought " + amount + " " + currencyCode.ToUpper()
                    + " for " + cost + " PLN. New PLN balance: " + plnBalances[username];
            }
            catch
            {
                return "Error: could not complete purchase.";
            }
        }

        public string SellCurrency(string username, string currencyCode, decimal amount)
        {
            if (!users.ContainsKey(username))
                return "Error: user not found.";

            if (!wallets[username].ContainsKey(currencyCode.ToUpper()) ||
                wallets[username][currencyCode.ToUpper()] < amount)
                return "Error: not enough " + currencyCode.ToUpper() + " in wallet.";

            try
            {
                string url = "http://api.nbp.pl/api/exchangerates/rates/a/"
                             + currencyCode.ToUpper() + "/?format=json";
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(url);

                int idx = json.IndexOf("\"mid\":");
                string after = json.Substring(idx + 6);
                decimal mid = decimal.Parse(after.Split(',')[0].Split('}')[0].Trim(),
                    System.Globalization.CultureInfo.InvariantCulture);

                decimal bidRate = Math.Round(mid * 0.99m, 4);
                decimal earned = Math.Round(amount * bidRate, 2);

                wallets[username][currencyCode.ToUpper()] -= amount;
                plnBalances[username] += earned;

                transactions.Add(username + " | SELL | " + amount + " " + currencyCode.ToUpper()
                    + " | rate: " + bidRate + " | earned: " + earned + " PLN | " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                return "Success: sold " + amount + " " + currencyCode.ToUpper()
                    + " for " + earned + " PLN. New PLN balance: " + plnBalances[username];
            }
            catch
            {
                return "Error: could not complete sale.";
            }
        }

        public string GetTransactionHistory(string username)
        {
            if (!users.ContainsKey(username))
                return "Error: user not found.";

            string result = "Transaction history for " + username + ":\n";
            foreach (var t in transactions)
                if (t.StartsWith(username))
                    result += t + "\n";
            return result;
        }
    }
}