using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace WebApiTest.Model
{
    public enum ChargeState
    {
        Invalid, Charging, ChargeDone, NoCharge
    }
    public class Datapoint
    {
        [BsonId]
        public long id;

        public DateTime UTCTimestamp;

        public ChargeState chargeState;

        public float BatteryVoltage;

        public Dictionary<string, float> sensorReadings = new Dictionary<string, float>();

        private static Dictionary<short, string> _sensorIdLUT = null;
        
        public static Dictionary<short, string> SensorIdLUT { get
            {
                if(_sensorIdLUT == null)
                {
                    _sensorIdLUT = new Dictionary<short, string>()
                    {
                        [4] = "Light - outdoors",
                        [5] = "Pressure - Internal",
                        [6] = "Humidity - Internal",
                        [7] = "Air Temperature - Internal",
                        [8] = "Air Temperature - External",
                        [9] = "Humidity - External",
                        [10] = "Pressure - External",
                        [11] = "Soil Temperature 1", 
                        [12] = "Soil Temperature 2", 
                        [13] = "Soil Temperature 3", 
                        [14] = "Soil Moisture 1", 
                        [15] = "Soil Moisture 2",
                        [16] = "Soil Moisture 3"
                    };
                }
                return _sensorIdLUT; 
            }
        }


    }
}
