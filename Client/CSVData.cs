using Common;
using Common.Interfaces;
using Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class CSVData : ICSVManipulation, IDisposable
    {
        public static string pathCSV = ConfigurationManager.AppSettings["CsvDatoteka"] + DateTime.Now.Ticks + ".csv";
        private string fileName = Path.GetFileName(pathCSV);
        
        private bool disposedValue;
        

        public static string PathCSV { get => pathCSV; set => pathCSV = value; }
        public string FileName { get => fileName; set => fileName = value; }    
        
        
        private FileStream stream = new FileStream(PathCSV, FileMode.Create);
        public string GetPathFullName()
        {
            return Path.GetFullPath(pathCSV);
        }
        /// <summary>
        /// Metoda koja upisuje dobijene Load objekte u csv datoteku
        /// </summary>
        /// <param name="load"></param>
        public void LogCSV(List<Load> load)
        {
            
            using(StreamWriter streamWriter = new StreamWriter(stream))
            {
                if(load.Count > 0)
                {
                    streamWriter.WriteLine("TIME_STAMP,FORECAST_VALUE,MEASURED_VALUE");
                    foreach (var l in load)
                    {
                        streamWriter.WriteLine(l.CSVString());        
                    }
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    stream.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~CSVData()
        {
             // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
             Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
