using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OilGas.EventHubProvider;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace OilGas.EventReceiverApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //string connString = ConfigurationManager.AppSettings["EHConnectionString"];
            //string ehNS = ConfigurationManager.AppSettings["EHNameSpace"];
            //string ehDesc = ConfigurationManager.AppSettings["EHDescription"];
            //EventHubProvider.EventHubProvider EHProvider = new EventHubProvider.EventHubProvider(ehNS, ehDesc, connString);
            string eventHubConnectionString = ConfigurationManager.AppSettings["EHConnectionString"];
            string eventHubName = ConfigurationManager.AppSettings["EHDescription"];

            string storageAccountName = "sbusexperiment";
            string storageAccountKey = "wXhvEVf/8bF2ZTaDr0vmynE+ha9zXRdAkpU81UeG7Pg3Bmxg3ny2Sbmocd1fOMvFlI2Q82IkYfI3FOqVb1rpqQ==";

            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                        storageAccountName, storageAccountKey);

            string eventProcessorHostName = Guid.NewGuid().ToString();
            EventProcessorHost eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
           // eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>().Wait();

            //Console.WriteLine("Receiving. Press enter key to stop worker.");
            while(true)
            {
                
            }

        }
    }
}
