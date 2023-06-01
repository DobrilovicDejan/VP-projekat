using Common;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
     class Program
     {
        // manualno pokretanje brisanja
        private static ManualResetEvent resetEvent = new ManualResetEvent(false);
        public delegate void DelRemoveOldData();

         static void Main(string[] args)
        {
            InMemoryBase inMemoryBase = new InMemoryBase();
            IdCounter.GetIDs();

            
            string path = ConfigurationManager.AppSettings["LoadDatoteka"];
            Console.WriteLine(path);
            
            using (ServiceHost host = new ServiceHost(typeof(ServiceController)))
            {
                host.Open();
                Console.WriteLine("Servis je uspesno pokrenut");

                DelRemoveOldData removeOldData = ServiceController.RemoveOldData;
                Thread thread = new Thread(() => removeOldData.Invoke());
                thread.Start();
                

                resetEvent.Set();
                thread.Join();

                Console.ReadKey();
                host.Close();
                resetEvent.Dispose();
                
            }
                inMemoryBase = null;
         }
     }
}
