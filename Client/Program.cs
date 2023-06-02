using Common;
using Common.Exceptions;
using Common.Interfaces;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace Client
{
    public class Program
    {
        ClientController client = new ClientController();
        static void Main(string[] args)
        {
            ChannelFactory<IServiceController> channel = new ChannelFactory<IServiceController>("Service");
            Console.WriteLine("Klijent je spreman!");
            Program program = new Program();

            bool running = true;

            while (running)
            {
                IServiceController serviceController = channel.CreateChannel();
               // Load load = new Load(1, DateTime.Now, 23.56, 2011);

                try
                {
                    running = program.Run(serviceController);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(2000);
            }
        }
        private bool Run(IServiceController serviceController)
        {
            int br = -1;
            while (br != 0)
            {
                Console.WriteLine("\n========================================================");
                Console.WriteLine("Izaberite opciju:");
                Console.WriteLine("\t[1] Dobavljanje podataka iz baze podataka");
                Console.WriteLine("\t[2] Ispisi Load iz In-Memory bazi podataka");
                Console.WriteLine("\t[3] Ispisi Audit iz In-Memory bazi podataka");
                Console.WriteLine("\t[4] Dodavanje Load objekata");
                Console.WriteLine("\t[5] Obrisi Log (Audit) objekte iz In-Memory baze");
                Console.WriteLine("\t[6] Ocisti In-Memory bazu podataka");


                Console.WriteLine("\t[0] Izlazak iz programa");

                Console.WriteLine("---------------------------------------------------");
                Console.Write("Odaberi opciju: ");
                br = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("========================================================\n");
                provera(br, serviceController);
            }
            return false;
        }

        private void provera(int broj, IServiceController serviceController)
        {
            switch (broj)
            {
                case 1:
                    {
                        try
                        {

                            var requestDate = client.GetInput();
                            var loads = serviceController.PostRequest(requestDate);
                            client.GetRequest(loads);
                            if (loads.Count() != 0)
                            {
                                CSVData cSV = new CSVData();
                                cSV.LogCSV(loads);
                                Console.WriteLine("Upisano u datoteku " + cSV.FileName + " na putanji " + cSV.GetPathFullName());
                            }

                        }
                        catch (FaultException<InternalCommunicationException> ex)
                        {
                            Console.WriteLine(ex.Detail.Message);
                        }
                        break;
                    }
                case 2:
                    {
                        List<Load> list = serviceController.PrintLoad();
                        if (list.Count == 0)
                        {
                            Console.WriteLine("Nema Load podataka u In-Memory bazi");
                        }
                        else
                        {
                            foreach (Load l in list)
                            {
                                Console.WriteLine(l.ToString());
                            }
                        }


                        break;
                    }

                case 3:
                    {
                        List<Audit> list = serviceController.PrintAudit();
                        if (list.Count == 0)
                        {
                            Console.WriteLine("Nema Audit podataka u In-Memory bazi");
                        }
                        else
                        {
                            foreach (Audit a in list)
                            {
                                Console.WriteLine(a.ToString());
                            }
                        }


                        break;
                    }
                case 4:
                    {

                        Console.WriteLine("Unesite ID: ");
                        int br = -1;
                        string input = Console.ReadLine();
                        if(input != null && input != "" && input != "\n") 
                        {
                            br = Convert.ToInt32(input);
                        }
                        Console.WriteLine("Unesite TIME STAMP u formatu yyyy-mm-dd HH:MM: ");
                        DateTime date = DateTime.Now;
                        input = Console.ReadLine();
                        if (input != null && input != "" && input != "\n")
                        {
                            date = DateTime.Parse(input);
                        }
                        Console.WriteLine("Unesite FORECAST VALUE: ");
                        double forecast = double.Parse(Console.ReadLine());
                        Console.WriteLine("Unesite MEASURED VALUE:");
                        double measured = double.Parse(Console.ReadLine());

                        if (serviceController.AddLoad(br, date, forecast, measured))
                        {
                            if(br > 0) 
                            { 
                            Console.WriteLine("\nUspesno je dodat Load objekat sa Id-em:" + br + ".");
                            }
                            else
                            {
                            Console.WriteLine("\nUspesno je dodat Load objekat.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nNeuspesno upisivanje Load objekta u In-Memory bazu.");
                        }


                        break;
                    }
                case 5:
                    {
                        object tempObj = new object();
                        var dataTimeToRemove = client.GetInput();
                        if (serviceController.RemoveAudit(dataTimeToRemove, tempObj))
                        {
                            Console.WriteLine("\nUspesno je obrisan Audit objekat iz In-Memory baze podataka.");
                        }
                        else
                        {
                            Console.WriteLine("Neuspesno obrisan Audit objekat iz In-Memory baze sa prosledjenim datumom " + dataTimeToRemove.ToString());
                        }

                        break;
                    }

                case 6:
                    {
                        if (serviceController.RemoveAllDataFromInMemory())
                        {
                            Console.WriteLine("Uspesno je obrisani svi podaci iz In-Memory baze podataka.");
                        }
                        else 
                        {
                            Console.WriteLine("Neuspesno obrisani svi podaci iz In-Memory baze podataka.");
                        }

                        break;
                    }

                case 0:
                    {
                        InMemoryBase.dbMemory.Clear();
                        return;
                    }
                default:
                    {
                        throw new Exception("Niste izabrali nijednu od opcija. Ponovno biranje.");
                    }
            }
        }






    }
}
