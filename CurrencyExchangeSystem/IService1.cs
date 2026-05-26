using System.ServiceModel;

namespace CurrencyExchangeSystem
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string SayHello(string name);
    }
}