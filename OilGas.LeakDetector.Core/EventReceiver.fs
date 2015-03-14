namespace OilGas.LeakDetector.Core

open Microsoft.ServiceBus.Messaging
open System.Threading.Tasks
open System.Collections.Generic
open Core
open System.Text
open System.Diagnostics

module EventReceiver =

    type EventReceiverContext = {consumerGroup: EventHubConsumerGroup; processorHost: EventProcessorHost; }

    type SimpleEventProcessor =
            [<DefaultValue>]val mutable private _partitionContext : PartitionContext
            member x.partitionContext 
                with get() =
                    x._partitionContext
                and set(value) =
                    x._partitionContext <- value
            [<DefaultValue>]val mutable private _stopWatch : Stopwatch
            member x.stopWatch
                with get() =
                    x._stopWatch
                and set(value) =
                    x._stopWatch <- value

            interface IEventProcessor with
                member this.CloseAsync(context:PartitionContext, reason:CloseReason) =
                    System.Diagnostics.Trace.TraceInformation("I made it to Processing Events Async!")
                    match reason with
                    | CloseReason.Shutdown -> context.CheckpointAsync()
                    | _ -> 
                        let a = new System.Action(fun t -> ())
                        new Task(a)

                member this.OpenAsync(context:PartitionContext) =
                    System.Diagnostics.Trace.TraceInformation("I made it to Processing Events Async!")
                    this.partitionContext <- context
                    this._stopWatch <- new Stopwatch()
                    this._stopWatch.Start()
                    let a = new System.Action(fun t -> ())
                    new Task(a)

                member this.ProcessEventsAsync(context:PartitionContext, messages:IEnumerable<EventData>) =
                    messages 
                    |> Seq.map(fun eventData -> Encoding.UTF8.GetString(eventData.GetBytes())) 
                    |> Seq.iter(fun data -> System.Diagnostics.Trace.TraceInformation("I made it to Processing Events Async!"))
                    let a = new System.Action(fun t -> ())
                    new Task(a)