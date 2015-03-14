using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OilGas.TelemetryCore
{
    public class PressureTelemetryEvent : ITelemetryMessage
    {
        public string ID { get; set; }
        public int SensorID { get; set; }
        public float Pressure { get; set; }
        public DateTime CollectionTime { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }

        public PressureTelemetryEvent(string id, int sensorID, float pressure,
            DateTime collectionTime, float Longitude, float Latitude)
        {
            this.ID = id;
            this.SensorID = sensorID;
            this.Pressure = pressure;
            this.CollectionTime = collectionTime;
            this.Longitude = Longitude;
            this.Latitude = Latitude;
        }
    }
}
