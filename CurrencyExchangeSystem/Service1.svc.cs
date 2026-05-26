using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace CurrencyExchangeSystem
{
    public class Service1 : IService1
    {
        private string connStr = ConfigurationManager.ConnectionStrings["CurrencyDB"].ConnectionString;

        private decimal GetMidRate(string currencyCode)
        {
            string url = "http://api.nbp.pl/api/exchangerates/rates/a/" + currencyCode.ToUpper() + "/?format=json";
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string json = client.DownloadString(url);
            int idx = json.IndexOf("\"mid\":");
            string after = json.Substring(idx + 6);
            return decimal.Parse(after.Split(',')[0].Split('}')[0].Trim(), System.Globalization.CultureInfo.InvariantCulture);
        }

        public string GetRate(string currencyCode)
        {
            try
            {
                decimal mid = GetMidRate(currencyCode);
                decimal bid = Math.Round(mid * 0.99m, 4);
                decimal ask = Math.Round(mid * 1.01m, 4);
                return currencyCode.ToUpper() + " | mid: " + mid + " | buy: " + ask + " | sell: " + bid + " PLN";
            }
            catch { return "Error: currency code " + currencyCode + " not found."; }
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
                    string code = json.Substring(codeIdx + 8).Split('"')[0];
                    int midIdx = json.IndexOf("\"mid\":", codeIdx);
                    string mid = json.Substring(midIdx + 6).Split(',')[0].Split('}')[0].Trim();
                    result += code + " = " + mid + " PLN\n";
                    pos = midIdx + 6;
                }
                return result;
            }
            catch { return "Error: could not retrieve rates."; }
        }

        public string GetHistoricalRates(string currencyCode, string startDate, string endDate)
        {
            try
            {
                DateTime start = DateTime.Parse(startDate);
                DateTime end = DateTime.Parse(endDate);
                if (start > end) return "Error: start date cannot be after end date.";
                if ((end - start).TotalDays > 93) return "Error: date range cannot exceed 93 days.";
                string url = "http://api.nbp.pl/api/exchangerates/rates/a/" + currencyCode.ToUpper() + "/" + start.ToString("yyyy-MM-dd") + "/" + end.ToString("yyyy-MM-dd") + "/?format=json";
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(url);
                string result = currencyCode.ToUpper() + " historical rates:\n";
                int pos = 0;
                while (true)
                {
                    int dateIdx = json.IndexOf("\"effectiveDate\":", pos);
                    if (dateIdx == -1) break;
                    string date = json.Substring(dateIdx + 17).Split('"')[0];
                    int midIdx = json.IndexOf("\"mid\":", dateIdx);
                    string mid = json.Substring(midIdx + 6).Split(',')[0].Split('}')[0].Trim();
                    result += date + " = " + mid + " PLN\n";
                    pos = midIdx + 6;
                }
                return result;
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string Register(string username, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string check = "SELECT COUNT(*) FROM Users WHERE Username = @u";
                    SqlCommand cmd = new SqlCommand(check, conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0) return "Error: username already exists.";

                    string insert = "INSERT INTO Users (Username, Password) VALUES (@u, @p)";
                    cmd = new SqlCommand(insert, conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);
                    cmd.ExecuteNonQuery();

                    string getUserId = "SELECT Id FROM Users WHERE Username = @u";
                    cmd = new SqlCommand(getUserId, conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    int userId = (int)cmd.ExecuteScalar();

                    string insertBalance = "INSERT INTO Balances (UserId, CurrencyCode, Amount) VALUES (@id, 'PLN', 0)";
                    cmd = new SqlCommand(insertBalance, conn);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }
                return "Success: user " + username + " registered.";
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string Login(string username, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @u AND Password = @p";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0) return "Success: welcome " + username + "!";
                    return "Error: invalid username or password.";
                }
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string TopUp(string username, decimal amount)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    int userId = GetUserId(conn, username);
                    if (userId == -1) return "Error: user not found.";

                    string update = "UPDATE Balances SET Amount = Amount + @a WHERE UserId = @id AND CurrencyCode = 'PLN'";
                    SqlCommand cmd = new SqlCommand(update, conn);
                    cmd.Parameters.AddWithValue("@a", amount);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();

                    string insertTx = "INSERT INTO Transactions (UserId, Type, CurrencyCode, Amount, Rate, PlnValue) VALUES (@id, 'TOPUP', 'PLN', @a, 1, @a)";
                    cmd = new SqlCommand(insertTx, conn);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.Parameters.AddWithValue("@a", amount);
                    cmd.ExecuteNonQuery();

                    decimal balance = GetBalance(conn, userId, "PLN");
                    return "Success: balance is now " + balance + " PLN.";
                }
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string GetBalance(string username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    int userId = GetUserId(conn, username);
                    if (userId == -1) return "Error: user not found.";

                    string query = "SELECT CurrencyCode, Amount FROM Balances WHERE UserId = @id AND Amount > 0";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    string result = "";
                    while (reader.Read())
                        result += reader["CurrencyCode"] + ": " + reader["Amount"] + "\n";
                    return result == "" ? "No balances found." : result;
                }
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string BuyCurrency(string username, string currencyCode, decimal amount)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    int userId = GetUserId(conn, username);
                    if (userId == -1) return "Error: user not found.";

                    decimal mid = GetMidRate(currencyCode);
                    decimal askRate = Math.Round(mid * 1.01m, 4);
                    decimal cost = Math.Round(amount * askRate, 2);

                    decimal plnBalance = GetBalance(conn, userId, "PLN");
                    if (plnBalance < cost) return "Error: not enough PLN. Need " + cost + " PLN.";

                    string updatePln = "UPDATE Balances SET Amount = Amount - @cost WHERE UserId = @id AND CurrencyCode = 'PLN'";
                    SqlCommand cmd = new SqlCommand(updatePln, conn);
                    cmd.Parameters.AddWithValue("@cost", cost);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();

                    string checkWallet = "SELECT COUNT(*) FROM Balances WHERE UserId = @id AND CurrencyCode = @c";
                    cmd = new SqlCommand(checkWallet, conn);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.Parameters.AddWithValue("@c", currencyCode.ToUpper());
                    int exists = (int)cmd.ExecuteScalar();

                    if (exists == 0)
                    {
                        string insertWallet = "INSERT INTO Balances (UserId, CurrencyCode, Amount) VALUES (@id, @c, @a)";
                        cmd = new SqlCommand(insertWallet, conn);
                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.Parameters.AddWithValue("@c", currencyCode.ToUpper());
                        cmd.Parameters.AddWithValue("@a", amount);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string updateWallet = "UPDATE Balances SET Amount = Amount + @a WHERE UserId = @id AND CurrencyCode = @c";
                        cmd = new SqlCommand(updateWallet, conn);
                        cmd.Parameters.AddWithValue("@a", amount);
                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.Parameters.AddWithValue("@c", currencyCode.ToUpper());
                        cmd.ExecuteNonQuery();
                    }

                    string insertTx = "INSERT INTO Transactions (UserId, Type, CurrencyCode, Amount, Rate, PlnValue) VALUES (@id, 'BUY', @c, @a, @r, @pln)";
                    cmd = new SqlCommand(insertTx, conn);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.Parameters.AddWithValue("@c", currencyCode.ToUpper());
                    cmd.Parameters.AddWithValue("@a", amount);
                    cmd.Parameters.AddWithValue("@r", askRate);
                    cmd.Parameters.AddWithValue("@pln", cost);
                    cmd.ExecuteNonQuery();

                    decimal newPln = GetBalance(conn, userId, "PLN");
                    return "Success: bought " + amount + " " + currencyCode.ToUpper() + " for " + cost + " PLN. New PLN balance: " + newPln;
                }
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string SellCurrency(string username, string currencyCode, decimal amount)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    int userId = GetUserId(conn, username);
                    if (userId == -1) return "Error: user not found.";

                    decimal currBalance = GetBalance(conn, userId, currencyCode.ToUpper());
                    if (currBalance < amount) return "Error: not enough " + currencyCode.ToUpper() + " in wallet.";

                    decimal mid = GetMidRate(currencyCode);
                    decimal bidRate = Math.Round(mid * 0.99m, 4);
                    decimal earned = Math.Round(amount * bidRate, 2);

                    string updateWallet = "UPDATE Balances SET Amount = Amount - @a WHERE UserId = @id AND CurrencyCode = @c";
                    SqlCommand cmd = new SqlCommand(updateWallet, conn);
                    cmd.Parameters.AddWithValue("@a", amount);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.Parameters.AddWithValue("@c", currencyCode.ToUpper());
                    cmd.ExecuteNonQuery();

                    string updatePln = "UPDATE Balances SET Amount = Amount + @earned WHERE UserId = @id AND CurrencyCode = 'PLN'";
                    cmd = new SqlCommand(updatePln, conn);
                    cmd.Parameters.AddWithValue("@earned", earned);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();

                    string insertTx = "INSERT INTO Transactions (UserId, Type, CurrencyCode, Amount, Rate, PlnValue) VALUES (@id, 'SELL', @c, @a, @r, @pln)";
                    cmd = new SqlCommand(insertTx, conn);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.Parameters.AddWithValue("@c", currencyCode.ToUpper());
                    cmd.Parameters.AddWithValue("@a", amount);
                    cmd.Parameters.AddWithValue("@r", bidRate);
                    cmd.Parameters.AddWithValue("@pln", earned);
                    cmd.ExecuteNonQuery();

                    decimal newPln = GetBalance(conn, userId, "PLN");
                    return "Success: sold " + amount + " " + currencyCode.ToUpper() + " for " + earned + " PLN. New PLN balance: " + newPln;
                }
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string GetTransactionHistory(string username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    int userId = GetUserId(conn, username);
                    if (userId == -1) return "Error: user not found.";

                    string query = "SELECT Type, CurrencyCode, Amount, Rate, PlnValue, Date FROM Transactions WHERE UserId = @id ORDER BY Date DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    string result = "Transaction history for " + username + ":\n";
                    while (reader.Read())
                        result += reader["Date"].ToString().Substring(0, 16) + " | " + reader["Type"] + " | " + reader["Amount"] + " " + reader["CurrencyCode"] + " | rate: " + reader["Rate"] + " | PLN: " + reader["PlnValue"] + "\n";
                    return result;
                }
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        private int GetUserId(SqlConnection conn, string username)
        {
            SqlCommand cmd = new SqlCommand("SELECT Id FROM Users WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            object result = cmd.ExecuteScalar();
            return result == null ? -1 : (int)result;
        }

        private decimal GetBalance(SqlConnection conn, int userId, string currencyCode)
        {
            SqlCommand cmd = new SqlCommand("SELECT Amount FROM Balances WHERE UserId = @id AND CurrencyCode = @c", conn);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@c", currencyCode);
            object result = cmd.ExecuteScalar();
            return result == null ? 0 : (decimal)result;
        }
    }
}