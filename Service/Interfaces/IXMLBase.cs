using Common;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    internal interface IXMLBase
    {
        List<Load> ReadData(DateTime date);
        bool WriteLoadInMemory(Load load);
        void WriteAuditData(int id, AuditType type, DateTime dateInput);
        bool CheckXMLBase(Load load, DateTime time);
    }
}
