using Common;
using Common.Exceptions;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.Xml;

namespace Service
{
    public class XMLBase : IXMLBase, IDisposable
    {
        private bool disposedValue;
        private List<Load> lista = new List<Load>();

        private XmlDocument doc = new XmlDocument();
        private XmlDocument xmlDoc = new XmlDocument();


        /// <summary>
        /// Metoda koja po datumu trazi Load objekte u XML bazi podataka i ako postoje
        /// vraca listu tih objekata
        /// </summary>
        /// <param name="dateInput"></param>
        /// <returns></returns>
        public List<Load> ReadData(DateTime dateInput)
        {
            string path = ConfigurationManager.AppSettings["LoadDatoteka"];
            doc.Load(path);

            bool isHere = false;

            foreach (XmlNode node in doc.DocumentElement)
            {
                Load load = null;
                try
                {

                    int id = int.Parse(node.FirstChild.InnerText);
                    DateTime time = DateTime.Parse(node.FirstChild.NextSibling.InnerText);
                    double forecast_value = double.Parse(node.FirstChild.NextSibling.NextSibling.InnerText);
                    double measured_value = double.Parse(node.LastChild.InnerText);
                    load = new Load(id, time, forecast_value, measured_value);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Problem prilikom ucitavanja (parsiranja) Load objekta iz XML baze: " + ex.Message);
                }



                if (CheckXMLBase(load, dateInput))
                {
                    lista.Add(load);
                    try
                    {
                        WriteLoadInMemory(load);
                    }
                    catch (FaultException<InternalCommunicationException> ex)
                    {
                        Console.WriteLine("Greska prilikom upisa Load objekata u In-Memory bazu podataka: " + ex.Detail.Message);
                    }
                    isHere = true;
                }
            }
            if (isHere)
            {
                try
                {
                    WriteAuditData(IdCounter.AuditCounter++, AuditType.Info, dateInput);
                }
                catch (FaultException<InternalCommunicationException> ex)
                {
                    Console.WriteLine("Greska prilikom upisa Audit objekata: " + ex.Detail.Message);
                }
            }
            else
            {
                try
                {
                    WriteAuditData(IdCounter.AuditCounter++, AuditType.Error, dateInput);
                }
                catch (FaultException<InternalCommunicationException> ex)
                {
                    Console.WriteLine("Greska prilikom upisa Audit objekata: " + ex.Detail.Message);
                }
            }

            return lista;
        }


        /// <summary>
        /// Metoda koja provjerava datum postojeceg Load objekta u XML bazi podataka
        /// i poredi ga sa prosledjenim datumom.
        /// </summary>
        /// <param name="load"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool CheckXMLBase(Load load, DateTime time)
        {
            if (load == null)
                return false;
            return (load.Equals(time)) ? true : false;
        }


        /// <summary>
        /// Metoda koja procitani Load objekat iz XML baze upisuje u In-Memory bazu
        /// </summary>
        /// <param name="load"></param>
        /// <returns></returns>
        /// <exception cref="FaultException{InternalCommunicationException}"></exception>
        public bool WriteLoadInMemory(Load load)
        {
            if (load == null)
            {
                InternalCommunicationException izuzetak =
                    new InternalCommunicationException("Desila se nepravilnost pri upisivanju Load objekta u in-Memory bazu podataka");

                throw new FaultException<InternalCommunicationException>(izuzetak);
            }
            return InMemoryBase.dbMemory.TryAdd(load.Id, load);
        }

        /// <summary>
        /// Metoda koja pravi novi Audit objekat i upisuje ga u obe baze podataka
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="dateInput"></param>
        /// <exception cref="FaultException{InternalCommunicationException}"></exception>
        public void WriteAuditData(int id, AuditType type, DateTime dateInput)
        {
            string message = "";
            DateTime time = DateTime.Now;

            if (type == AuditType.Info)
            {
                message = "Podaci su procitani i prosledjeni";
            }
            else if (type == AuditType.Error)
            {
                message = "U bazi ne postoje podaci za datum " + dateInput.Date.ToShortDateString();
            }
            else
            {
                message = "Desila se nepravilnost pri radu" + dateInput.Date.ToShortDateString();
                InternalCommunicationException izuzetak =
                    new InternalCommunicationException("Desila se nepravilnost pri rukovanju sa Audit XML bazom podataka");

                throw new FaultException<InternalCommunicationException>(izuzetak);
            }
            var audit = new Audit(id, time, type, message);
            InMemoryBase.dbMemory.TryAdd(id, audit);

            string path = ConfigurationManager.AppSettings["AuditDatoteka"];

            LogAudit(path, audit);

        }

        /// <summary>
        /// Metoda koja upisuje Audit objekat u XML bazu na prosledjenoj putanji
        /// </summary>
        /// <param name="path"></param>
        /// <param name="audit"></param>
        private void LogAudit(string path, Audit audit)
        {
            xmlDoc.Load(path);
            XmlElement element = xmlDoc.CreateElement("row");
            try
            {

                XmlElement Id = xmlDoc.CreateElement("ID");
                Id.InnerText = audit.ID.ToString();
                element.AppendChild(Id);

                XmlElement timestamp = xmlDoc.CreateElement("TIME_STAMP");
                timestamp.InnerText = audit.TimeStamp.ToString();
                element.AppendChild(timestamp);

                XmlElement typem = xmlDoc.CreateElement("MESSAGE_TYPE");
                typem.InnerText = audit.Type.ToString();
                element.AppendChild(typem);

                XmlElement msg = xmlDoc.CreateElement("MESSAGE");
                msg.InnerText = audit.Message;
                element.AppendChild(msg);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem pri upisu Audit objekta u XML bazu: " + ex.Message);
            }
            xmlDoc.DocumentElement.AppendChild(element);
            xmlDoc.Save(path);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    doc.RemoveAll();
                    doc = null;
                    xmlDoc.RemoveAll();
                    xmlDoc = null;
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~XMLBase()
        {
            //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
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
