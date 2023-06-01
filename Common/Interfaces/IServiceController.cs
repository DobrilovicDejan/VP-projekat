using Common;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
   [ServiceContract]
   public interface IServiceController
   {
        [OperationContract]
        [FaultContract(typeof(InternalCommunicationException))]
        
        List<Load> PostRequest(DateTime time);

        [OperationContract]
        List<Load> PrintLoad();

        [OperationContract]
        List<Audit> PrintAudit();

        [OperationContract]
        bool AddLoad(int id, DateTime time, double forecast, double measured);

        [OperationContract]
        bool RemoveAudit(DateTime time, object temp);

        [OperationContract]
        bool RemoveAllDataFromInMemory();
   }
}
