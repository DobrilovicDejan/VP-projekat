using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum AuditType { Info, Warning, Error }
    [DataContract]
    public class Audit
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public DateTime TimeStamp { get; set; }
        [DataMember]
        public AuditType Type { get; set; }

        [DataMember]
        public string Message { get; set; }

        public Audit() : base() { }

        public Audit(int iD, DateTime timeStamp, AuditType type, string message)
        {
            ID = iD;
            TimeStamp = timeStamp;
            Type = type;
            Message = message;
        }

        public override string ToString()
        {
            return "ID:" + ID + "\n" + "Time stamp: " + TimeStamp + "\n" +
                "Audit message type: " + Type + "\n" + "Message: " + Message + "\n";
        }

        public bool Equals(DateTime time)
        {
            return this.TimeStamp.Date == time.Date;
        }
    }
}
