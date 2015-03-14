using System;

namespace OilGas.TelemetryCore
{
    public class PipeFlowTelemetryEvent : ITelemetryMessage
    {
        public string ID { get; set; }
        public int SensorID { get; set; }
        public float Flow { get; set; }
        public DateTime CollectionTime { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }

        public PipeFlowTelemetryEvent(string id, int sensorID, float flow,
            DateTime collectionTime, float Longitude, float Latitude)
        {
            this.ID = id;
            this.SensorID = sensorID;
            this.Flow = flow;
            this.CollectionTime = collectionTime;
            this.Longitude = Longitude;
            this.Latitude = Latitude;
        }
    }
}
