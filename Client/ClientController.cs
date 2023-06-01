using Client.Interfaces;
using Common;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Client
{
    public  class ClientController : IClientController

    {
        /// <summary>
        /// Metoda koja od korisnika trazi da unese datum u pravilnom formatu
        /// </summary>
        /// <returns></returns>
        public DateTime GetInput() {
            do { 
            string temp;
            Console.WriteLine("Unesite datum u formatu yyyy-mm-dd: ");
            temp = Console.ReadLine();

            DateTime dateTime;
            if(VerifyDate(temp, out dateTime))
            {
            return dateTime;
            }

            Console.WriteLine("Molimo unesite datum u trazenom formatu");
            } while(true);

        }


        /// <summary>
        /// Metoda koja proverava da li je uneseni datum ispravno unesen
        /// </summary>
        /// <param name="dateString"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool VerifyDate(string dateString, out DateTime dateTime)
        {
            var dateValid = dateString.Split('-');
            if (dateValid.Length == 0 && 12 < int.Parse(dateValid[1]))
            {
                dateTime = DateTime.Now;
                return false;

            }

            return DateTime.TryParse(dateString, out dateTime);
        }

        /// <summary>
        /// Metoda koja dobijeni rezultat od servera ispisuje na konzolu
        /// </summary>
        /// <param name="loads"></param>
         public void GetRequest(List<Load> loads)
        {
            if (loads.Count() != 0)
            {
                foreach (Load load in loads)
                {
                    Console.WriteLine(load.ToString());
                }
            }
        }

       
    }
}
