
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        void Test();
    }
}
