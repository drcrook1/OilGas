namespace OilGas.DataGeneration

open System.Configuration
open Core
open System.Threading
open OilGas.EventHubProvider
open System
open OilGas.DataGeneration
open FSharp.Collections.ParallelSeq


type EHConfiguration = {ns: string; connstring: string; description: string}
type TelemetryDevice = {id : int; longitude : float32; latitude: float32}

module DataGenerator =
    open OilGas.TelemetryCore

    let CreateEHConfiguration ns conn des = 
        { ns = ns; connstring = conn; description = des } 

    let CreateTelemetryDevice id =
        match id with
        | 1 -> { id = 1; longitude = 49.899f; latitude =  -97.139f } |> succeed
        | 2 -> {id = 2; longitude = 50.450f; latitude = -104.600f;} |> succeed
        | 3 -> {id = 3; longitude = 40.036f; latitude = -97.022f;} |> succeed
        | 4 -> {id = 4; longitude = 35.482f; latitude = -97.535f;} |> succeed
        | _ -> [UnknownDeviceIdNumber] |> Failure

    let GetAppSetting (setting : string) =
        let s = ConfigurationManager.AppSettings.[setting]
        let q = ConfigurationManager.AppSettings
        if s <> Unchecked.defaultof<_>
        then [SettingDoesNotExist] |> Failure
        else s |> succeed

    let createProvider() =
        let config = CreateEHConfiguration
                            <!> succeed @"https://rtoieventhub-ns.servicebus.windows.net"
                            <*> succeed @"Endpoint=sb://rtoieventhub-ns.servicebus.windows.net/;SharedAccessKeyName=master;SharedAccessKey=iAZa4YpxH4IeEatOC1MpImgVLby45NGZq+6TRcqrRAU="
                            <*> succeed @"oilgasmockinput"
        match config with
        | Failure f -> Failure f
        | Success s -> 
            new EventHubProvider(s.ns, s.description, s.connstring) |> succeed

    let generateTempEvent(provider:EventHubProvider, device:TelemetryDevice) =
        let eventData = new TempTelemetryEvent(
                                device.id.ToString() + "T" + DateTime.UtcNow.ToString(), 
                                device.id, 73.0f, DateTime.UtcNow, device.longitude, device.latitude)
        provider.SendEvent(eventData).Wait()
        device

    let generatePressureEvent(provider:EventHubProvider, device:TelemetryDevice) =
        let eventData = new PressureTelemetryEvent(
                            device.id.ToString() + "P" + DateTime.UtcNow.ToString(),
                            device.id, 9.0f, DateTime.UtcNow, device.longitude, device.latitude)
        provider.SendEvent(eventData).Wait()
        device

    let generateFlowEvent(provider:EventHubProvider, device:TelemetryDevice) =
        let eventData = new PipeFlowTelemetryEvent(
                            device.id.ToString() + "F" + DateTime.UtcNow.ToString(),
                            device.id, 3.0f, DateTime.UtcNow, device.longitude, device.latitude)
        provider.SendEvent(eventData).Wait()
        device

    let GenerateSensorData (device : TelemetryDevice) =
        let p = createProvider()
        match p with
        | Success s -> generateFlowEvent(s, device) |> succeed
        | Failure f -> Failure f

    let Run () =
        let s = seq { for i in 1 .. 5 -> CreateTelemetryDevice i } 
                    |> Seq.filter(fun d ->
                                            match d with
                                            | Success s -> true
                                            | Failure f -> false)
                    |> Seq.map(fun d -> d.SuccessValue)
        while(true) do
            Thread.Sleep(4000)
            s |> PSeq.iter(fun d ->
                                Thread.Sleep(1000)
                                d |> GenerateSensorData |> ignore
                                )
