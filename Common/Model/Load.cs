using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class Load
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime TimeStamp { get; set; }
        [DataMember]
        public double ForecastValue { get; set; }
        [DataMember]
        public double MeasuredValue { get; set; }


        public Load() : this(0, new DateTime(), 0, 0) { }

        public Load(int id, DateTime timeStamp, double forecastValue, double measuredValue)
        {
            Id = id;
            TimeStamp = timeStamp;
            ForecastValue = forecastValue;
            MeasuredValue = measuredValue;
        }

        public string CSVString()
        {
            return TimeStamp.ToString() + "," + ForecastValue.ToString() + "," + MeasuredValue.ToString();
        }

        public override string ToString()
        {
            return "ID: " + Id + "\n TimeStamp: " + TimeStamp + "\nForecast value: " + ForecastValue +
                "\n Measured value: " + MeasuredValue + "\n";
        }

        public bool Equals(DateTime time)
        {
            return this.TimeStamp.Date == time.Date;
        }
    }

}
