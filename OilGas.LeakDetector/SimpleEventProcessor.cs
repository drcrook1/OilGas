using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using OilGas.TelemetryCore;
using OilGas.LeakDetector.Core;

namespace OilGas.LeakDetector
{
    public class SimpleEventProcessor : IEventProcessor
    {

        PartitionContext partitionContext;
        Stopwatch checkpointStopWatch;
        public static List<PipeFlowTelemetryEvent> flowEvents = new List<PipeFlowTelemetryEvent>();

        public SimpleEventProcessor()
        {

        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine(string.Format("SimpleEventProcessor initialize.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset));
            this.partitionContext = context;
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            //this.flowEvents = new List<PipeFlowTelemetryEvent>();
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Only async at the moment, because it has to be.  You can use Task.Run(lambda) and await stuff if you want...
        /// </summary>
        /// <param name="context"></param>
        /// <param name="events"></param>
        /// <returns></returns>
        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> events)
        {
            //Iterate through all events that we got this time around and process them.
            foreach (EventData eventData in events)
            {

                string s = Encoding.UTF8.GetString(eventData.GetBytes());
                SimpleEventProcessor.flowEvents.Add(JsonConvert.DeserializeObject<PipeFlowTelemetryEvent>(s));
                Console.WriteLine(s);
            }
            if(flowEvents.Count > 5)
            {
                LeakJobs.scheduleLeakJob(flowEvents);
                flowEvents.Clear();
                await context.CheckpointAsync();
            }
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine(string.Format("Processor Shuting Down.  Partition '{0}', Reason: '{1}'.", this.partitionContext.Lease.PartitionId, reason.ToString()));
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }
    }

    public static class Receiver
    {

        public static void CreateAndRun(string eventHubName, string eventHubConnectionString, string storageConnString)
        {
            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString, eventHubName);

            // Get the default Consumer Group
            EventHubConsumerGroup defaultConsumerGroup = eventHubClient.GetDefaultConsumerGroup();
            EventProcessorHost eventProcessorHost = new EventProcessorHost("singleworker", eventHubClient.Path, defaultConsumerGroup.GroupName, eventHubConnectionString, storageConnString);
            //this specifies the implementation of IEventProcessor
            eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>().Wait();
        }

    }
}