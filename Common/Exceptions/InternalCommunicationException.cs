using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    [DataContract]
    public class InternalCommunicationException
    {
        private string message;

        public InternalCommunicationException(string message) 
        { 
            Message = message;
        }

        [DataMember]
        public string Message { get => message; set => message = value; }

    }
}
