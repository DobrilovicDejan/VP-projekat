using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Service
{
    public class IdCounter
    {
        public static int LoadCounter { get; set; }
        public static int AuditCounter { get; set;}

        public static void GetIDs()
        {
            LoadCounter = XMLload() +1;
            AuditCounter = XMLaudit() +1;
        }


        /// <summary>
        /// Metoda koja varca najveci id Audit objekta iz liste svih procitanih Audit objekata 
        /// </summary>
        /// <param name="nesto"></param>
        /// <returns></returns>
        private static int GetMaxId(List<int> nesto) 
        {
            int max = nesto[0];
            for(int i = 1; i < nesto.Count; i++)
            {
                if (max < nesto[i])
                {
                    max = nesto[i];
                }
            }
            return max;
        }

        /// <summary>
        ///  Metoda koja vraca najveci id ucitanih Audit objekata iz XML baze podataka
        /// </summary>
        /// <returns></returns>
        private static int XMLaudit() 
        {
            XmlDocument xmlDoc = new XmlDocument();
            string path = ConfigurationManager.AppSettings["AuditDatoteka"];
            xmlDoc.Load(path);
            List<int> ids = new List<int>();

            foreach(XmlNode node  in xmlDoc.DocumentElement)
            {
                int id = 0;
                try
                {
                    id = int.Parse(node.FirstChild.InnerText);
                } catch(Exception) 
                { 
                }
                ids.Add(id);
            }

            return GetMaxId(ids);

        }

        /// <summary>
        /// Metoda koja vraca najveci id ucitanih Load objekata iz XML baze podataka
        /// </summary>
        /// <returns></returns>
        private static int XMLload() 
        {
            XmlDocument xmlDoc = new XmlDocument();
            string path = ConfigurationManager.AppSettings["LoadDatoteka"];
            xmlDoc.Load(path);
            List<int> ids = new List<int>();

            foreach (XmlNode node in xmlDoc.DocumentElement)
            {
                int id = 0;
                try
                {
                    id = int.Parse(node.FirstChild.InnerText);
                }
                catch (Exception)
                {
                }
                ids.Add(id);
            }
            return GetMaxId(ids);
        }
    }
}
