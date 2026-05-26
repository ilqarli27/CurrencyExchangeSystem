using System.Collections.Generic;
using System.ServiceModel;

namespace CurrencyExchangeSystem
{
    [ServiceContract]
    public interface IService1
    {
        // --- Rates ---
        [OperationContract]
        string GetRate(string currencyCode);

        [OperationContract]
        string GetAllRates();

        [OperationContract]
        string GetHistoricalRates(string currencyCode, string startDate, string endDate);

        // --- Users ---
        [OperationContract]
        string Register(string username, string password);

        [OperationContract]
        string Login(string username, string password);

        // --- Account ---
        [OperationContract]
        string TopUp(string username, decimal amount);

        [OperationContract]
        string GetBalance(string username);

        // --- Exchange ---
        [OperationContract]
        string BuyCurrency(string username, string currencyCode, decimal amount);

        [OperationContract]
        string SellCurrency(string username, string currencyCode, decimal amount);

        [OperationContract]
        string GetTransactionHistory(string username);
    }
}