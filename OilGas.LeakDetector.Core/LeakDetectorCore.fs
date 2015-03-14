namespace OilGas.LeakDetector.Core

open Core
open System.Threading
open FSharp.Collections.ParallelSeq
open Microsoft.ServiceBus.Messaging
open EventReceiver

module LeakDetectorCore =
        let Run () =
            ()
//            //let deviceInputQueues = Seq.empty
//            let connString = @"Endpoint=sb://rtoieventhub-ns.servicebus.windows.net/;SharedAccessKeyName=master;SharedAccessKey=iAZa4YpxH4IeEatOC1MpImgVLby45NGZq+6TRcqrRAU="
//            let eName = @"oilgasmockinput"
//            let blobConnectionString = @"DefaultEndpointsProtocol=https;AccountName=portalvhds88z53rq311gj2;AccountKey=d2v7SFqG+zbC5ERpC/E2JQ43dxilkrNsE3vG9mNWQXdG68e2oXqKuz2DJApcxejcoNrMDCGtKctvktjgqvm13w=="
//            let eventHubClient = EventHubClient.CreateFromConnectionString(connString, eName)
//            let dConsumerGroup = eventHubClient.GetDefaultConsumerGroup()
//            let processorHost = new EventProcessorHost("singleWorker3", eName, dConsumerGroup.GroupName, connString, blobConnectionString)
//            processorHost.RegisterEventProcessorAsync<SimpleEventProcessor>().Wait()
//            printfn "%s" "hellO!"
//              



