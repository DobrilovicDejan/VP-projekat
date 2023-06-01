using Common;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace Service
{
    [ServiceBehavior]
    public class ServiceController : IServiceController, IInMManipulation
    {

        /// <summary>
        /// Metoda koja proverava da li postoji objekat(Load ili Audit) u In-Memory bazi podataka 
        /// koji odgovara prosledjenom datumu
        /// </summary>
        /// <param name="time"></param>
        /// <returns>vraca true ako je pronadjen objekat sa istim datumom</returns>
        [OperationBehavior]
        public bool CheckInMemoryBase(DateTime time)
        {
            bool isHere = false;

            foreach (var obj in InMemoryBase.dbMemory.Values)
            {
                if (obj.GetType() == typeof(Load))
                {
                    try
                    {

                        Load load = (Load)obj;
                        if (load.Equals(time))
                        {
                            isHere = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Neispravan Load objekat, greska: " + ex.Message);
                    }
                }
                if (obj.GetType() == typeof(Audit))
                {
                    //da li treba vracati objekat i klase Audit koji se nalazi u inMemory bazi?
                    try
                    {
                        Audit audit = (Audit)obj;
                        if (audit.TimeStamp.Equals(time))
                            isHere |= true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Neispravan Audit objekat, greska: " + ex.Message);
                    }
                }
            }
            return isHere;
        }

        /// <summary>
        /// Metoda koja na zahtjev klijenta ispisuje sve Laod objekte iz In-Memory baze podataka
        /// </summary>
        /// <returns>vraca listu Load objekata</returns>
        [OperationBehavior]
        public List<Load> PrintLoad()
        {
            List<Load> list = new List<Load>(20);
            foreach (var obj in InMemoryBase.dbMemory.Values)
            {
                if (obj.GetType() == typeof(Load))
                {
                    try
                    {
                        Load load = (Load)obj;
                        list.Add(load);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Neispravan Load objekat, greska: " + ex.Message);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda koja na zahtjev klijenta ispisuje sve Audit objekte iz In-Memory baze podataka
        /// </summary>
        /// <returns>vraca listu Audit objekata</returns>
        [OperationBehavior]
        public List<Audit> PrintAudit()
        {
            List<Audit> list = new List<Audit>(10);
            foreach (var obj in InMemoryBase.dbMemory.Values)
            {
                if (obj.GetType() == typeof(Audit))
                {
                    try
                    {
                        Audit audit = (Audit)obj;
                        list.Add(audit);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Neispravan Audit objekat, greska: " + ex.Message);
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// Metoda koja na zahtev klijenta pristupa bazama podataka i trazi objekte 
        /// po prosledjenom datumu
        /// </summary>
        /// <param name="time"></param>
        /// <returns>Vraca listu Load objekata ukoliko postoje</returns>
        [OperationBehavior]
        public List<Load> PostRequest(DateTime time)
        {
            List<Load> list = new List<Load>(25);

            if (CheckInMemoryBase(time))
            {
                list = GetDataFromInMemory(time);
                Console.WriteLine("Objekat sa prosledjenim datum postoji u in memory bazi.");
            }
            else
            {
                XMLBase xmlBase = new XMLBase();
                list = xmlBase.ReadData(time);
                if (list.Count() != 0)
                {
                    Console.WriteLine("Objekat sa prosledjenim datum postoji u xml bazi.");
                }
                else
                {
                    Console.WriteLine("Objekat sa prosledjenim datum ne postoji u xml bazi.");
                }

            }
            return list;
        }

        
        /// <summary>
        /// Metoda koja trazi Load objekate u In-Memory bazi po prosledjenom vremenu
        /// </summary>
        /// <param name="time"></param>
        /// <returns>Vraca listu Load objekata</returns>
        public List<Load> GetDataFromInMemory(DateTime time)
        {
            List<Load> loads = new List<Load>(25);
            foreach (var l in InMemoryBase.dbMemory.Values)
            {

                if (l.GetType() == typeof(Load))
                {
                    try
                    {
                        Load load = (Load)l;
                        if (load.TimeStamp.Equals(time))
                            loads.Add(load);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Neispravan Load podatak u In-Memory bazi, greska: " + ex.Message);
                    }
                }
            }
            return loads;
        }

        /// <summary>
        /// Metoda koja se poziva kada je potrebno obrisati zastarele podatke iz In-Memory baze podataka
        /// </summary>
        public static void RemoveOldData()
        {
            int dataTimeout = 1;

            try
            {
                dataTimeout = Int32.Parse(ConfigurationManager.AppSettings["DataTimeout"]);

            }
            catch (Exception ex)
            {
                //Izmeniti za dalji rad, 
                //dodati validan ID
                //dodati exception poruku u audit

                Console.WriteLine("Greska prilikom brisanja zastarelih podataka, greska: " + ex.Message);

                XMLBase xML = new XMLBase();
                xML.WriteAuditData(IdCounter.AuditCounter++, AuditType.Warning, DateTime.Now);
            }

            while (true)
            {
                List<int> toRemove;
                object tempObj = new object();

                // trazenje zastarelih objekata
                FindToRemove(dataTimeout, out toRemove);
                RemoveFromInMemory(toRemove, tempObj);

                Thread.Sleep(TimeSpan.FromMinutes(1));

            }
        }

        /// <summary>
        /// Metoda koja trazi objekte u In-Memoty bazi podataka 
        /// cije je vrijeme kreiranja starije od 15 minuta 
        /// </summary>
        /// <param name="dataTimeout">parametar koji se cita iz app.config-a</param>
        /// <param name="toRemove"></param>
        private static void FindToRemove(int dataTimeout, out List<int> toRemove)
        {
            toRemove = new List<int>(25);
            foreach (var obj in InMemoryBase.dbMemory.Values)
            {
                Load load = new Load();
                if (obj.GetType() == typeof(Load))
                {
                    load = (Load)obj;
                    if (load.TimeStamp < DateTime.Now.AddMinutes(dataTimeout * -1))
                    {
                        toRemove.Add(load.Id);

                    }
                }

            }
        }

        /// <summary>
        /// Metoda koja brise objekte iz liste zastarelih objekata
        /// </summary>
        /// <param name="toRemove"></param>
        /// <param name="tempObj"></param>
        private static void RemoveFromInMemory(List<int> toRemove, object tempObj)
        {
            foreach (int key in toRemove)
            {

                bool removed = InMemoryBase.dbMemory.TryRemove(key, out tempObj);
                if (removed)
                    Console.WriteLine($"Obrisan je stari objekat {key}");
            }
        }

        /// <summary>
        /// Metoda koja dodaje novi Load objekat u In-Memory bazu podataka
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time"></param>
        /// <param name="forecast"></param>
        /// <param name="measured"></param>
        /// <returns>Vraca true ukoliko je Load objekat uspjesno dodat</returns>
        [OperationBehavior]
        public bool AddLoad(int id, DateTime time, double forecast, double measured)
        {
            Load load = new Load(id, time, forecast, measured);


            try
            {
                if (load != null)
                    InMemoryBase.dbMemory.TryAdd(id, load);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Neuspjesno upisivanje Load objekta u in memory bazu " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Metoda koja na zahtev klijenta brise Audit objekat iz In-Memory baze
        /// kome odgovara prosledjeni datum
        /// </summary>
        /// <param name="time"></param>
        /// <param name="temp"></param>
        /// <returns>Vraca true ukoliko je uspjesno pronadjen i obrisan Audit objekat</returns>

        [OperationBehavior]
        public bool RemoveAudit(DateTime time, object temp)
        {
            foreach (var obj in InMemoryBase.dbMemory.Values)
            {
                Audit audit = new Audit();
                if (obj.GetType() == typeof(Audit))
                {
                    try
                    {

                        audit = (Audit)obj;
                        if (audit.Equals(time))
                        {
                            return InMemoryBase.dbMemory.TryRemove(audit.ID, out temp);
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Greske pri brisanju Audit objekta." + ex.Message);
                    }
                }
            }


            return false;
        }
        

        /// <summary>
        /// Metoda koja na zahtev klijenta brise sve objekte iz In-Memomry baze 
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllDataFromInMemory()
        {
            InMemoryBase.dbMemory.Clear();

            if(InMemoryBase.dbMemory.Count == 0)
            {
                return true;
            }

            return false;
        }
    }
}
