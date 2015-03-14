using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OilGas.TelemetryCore
{
    public interface ITelemetryMessage
    {
        string ID { get; set; }
    }
}
