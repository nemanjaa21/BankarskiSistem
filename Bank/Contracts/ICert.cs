using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface ICert
    {
        [OperationContract]
        [FaultContract(typeof(CertException))]
        void TestCommunication();

        [OperationContract]
        [FaultContract(typeof(CertException))]
        string CardRequest();
        
    }
}
