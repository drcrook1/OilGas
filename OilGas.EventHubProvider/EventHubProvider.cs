using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using OilGas.TelemetryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OilGas.EventHubProvider
{
    public class EventHubProvider
    {
        public EventHubDescription EHDescription { get; set; }
        public EventHubClient EHClient { get; set; }
        //public static EventHubReceiver EHReceiver { get; set; }
        public EventHubSender EHSender { get; set; }

        /// <summary>
        /// Instantiate an Event Hub along with a Client.
        /// </summary>
        /// <param name="hubNameSpace">uri namespace</param>
        /// <param name="description">description</param>
        /// <param name="connectionString">connection string from app.config</param>
        public EventHubProvider(string hubNameSpace, string description, string connectionString, bool createClient = true)
        {
            ServiceBusConnectionStringBuilder builder = new ServiceBusConnectionStringBuilder(connectionString);
            builder.TransportType = TransportType.Amqp;
            NamespaceManager manager = NamespaceManager.CreateFromConnectionString(builder.ToString());
            //JP - Changed this to create if not exist
            this.EHDescription = manager.CreateEventHubIfNotExists(description);
            this.EHClient = EventHubClient.CreateFromConnectionString(connectionString, EHDescription.Path); 

            //if (EHSender == null)
            //{
            //    EHSender = this.EHClient.CreatePartitionedSender("0");
            //}
            
            //if (EHReceiver == null)
            //{
            //    //create a receiver too
            //    EventHubConsumerGroup group = EHClient.GetDefaultConsumerGroup();
            //    //we are defaulting to partition 0.  
            //    //Partitions are used for scale.  Each one can be thought of as a seperate lane 
            //    //for messages to go through.  As this is a demo, we are doing only parititon 0.
            //    EHReceiver = group.CreateReceiver(EHClient.GetRuntimeInformation().PartitionIds[0]);
            //}
        }

        //public string ReceiveMessage()
        //{
        //    EventData message = EHReceiver.Receive();
        //    if (message == null)
        //        return "Blank Message!";
        //    return Encoding.UTF8.GetString(message.GetBytes());
        //}

        /// <summary>
        /// Sends a message.  Note that if this fails, it will throw an exception.  For performance reasons, it is not try/catched here.
        /// </summary>
        /// <param name="eventHubName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendEvent(ITelemetryMessage message)
        {
            var serializedString = JsonConvert.SerializeObject(message);
            EventData data = new EventData(Encoding.UTF8.GetBytes(serializedString))
            {
                PartitionKey = message.ID
            };
            // Set user properties if needed 
            data.Properties.Add("Type", "PipeFlow" + DateTime.UtcNow.ToLongTimeString());
            return this.EHClient.SendAsync(data);
        }

        public async Task<bool> SendBatchEventsAsync(IEnumerable<ITelemetryMessage> messages)
        {
            try
            {
                List<Task> tasks = new List<Task>();

                foreach (ITelemetryMessage message in messages)
                {
                    var serializedString = JsonConvert.SerializeObject(message);
                    EventData data = new EventData(Encoding.UTF8.GetBytes(serializedString))
                    {
                        PartitionKey = message.ID
                    };
                    // Set user properties if needed 
                    data.Properties.Add("Type", "PipeFlow" + DateTime.Now.ToLongTimeString());

                    // Send the metric to Event Hub 
                    tasks.Add(this.EHClient.SendAsync(data));
                }

                //asynchronously execute sending of all messages
                return await Task.Run(() =>
                {
                    Task.WaitAll(tasks.ToArray());
                    return true;
                });
            }
            catch (Exception exp)
            {
                return false;
            }

        }
    }
}
