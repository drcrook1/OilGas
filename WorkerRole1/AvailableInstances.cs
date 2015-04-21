using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole1
{
    public class AvailableInstances
    {
        private static List<int> _availableInstances;
        private static readonly object padLock = new object();
        private static readonly object padLock2 = new object();
        public static List<int> availableInstances
        {
            get
            {
                lock (padLock2)
                {
                    return _availableInstances;
                }
            }
            set
            {
                lock (padLock2)
                {
                    _availableInstances = value;
                }
            }
        }
    }
}
