using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    [DataContract]
    public class InMemoryException
    {
        private string message;

        public InMemoryException(string message)
        {
            Message = message;
        }

        [DataMember]
        public string Message { get => message; set => message = value ; }
    }
}
