using System.ServiceModel;

namespace CurrencyExchangeSystem
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string SayHello(string name);

        [OperationContract]
        string GetRate(string currencyCode);

        [OperationContract]
        string GetAllRates();

        [OperationContract]
        string GetHistoricalRates(string currencyCode, string startDate, string endDate);
    }
}