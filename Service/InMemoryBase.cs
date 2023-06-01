using System.Collections.Concurrent;

namespace Service
{
    public class InMemoryBase
    {
        public static ConcurrentDictionary<int, object> dbMemory = new ConcurrentDictionary<int, object>();
      
        public InMemoryBase() : base() { }

        //destruktor
        ~InMemoryBase()
        {
            dbMemory.Clear();
        }
    }
}
