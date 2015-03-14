using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OilGas.LeakDetector.Core;
using OilGas.LeakDetector;

namespace TesterApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            string connString = @"Endpoint=sb://rtoieventhub-ns.servicebus.windows.net/;SharedAccessKeyName=master;SharedAccessKey=iAZa4YpxH4IeEatOC1MpImgVLby45NGZq+6TRcqrRAU=";
            string eName = @"oilgasmockinput";
            string blobConnectionString = @"DefaultEndpointsProtocol=https;AccountName=portalvhds88z53rq311gj2;AccountKey=d2v7SFqG+zbC5ERpC/E2JQ43dxilkrNsE3vG9mNWQXdG68e2oXqKuz2DJApcxejcoNrMDCGtKctvktjgqvm13w==";

            Receiver.CreateAndRun(eName, connString, blobConnectionString);
            //LeakDetectorCore.Run();
            Console.WriteLine("Press enter key to stop worker.");
            Console.ReadLine();
        }
    }
}
