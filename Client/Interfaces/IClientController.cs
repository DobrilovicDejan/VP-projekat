using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IClientController
    {
        DateTime GetInput();
        void GetRequest(List<Load> loads);
        bool VerifyDate(string dateString, out DateTime dateTime);
    }
}
