using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiTest.Model
{
    public enum ChargeState
    {
        Invalid, Charging, ChargeDone, NoCharge
    }
    public class Datapoint
    {
        public DateTime UTCTimestamp;

        public ChargeState chargeState;

        public float BatteryVoltage;

        public Dictionary<string, float> sensorReadings = new Dictionary<string, float>();

        public static Dictionary<short, string> SensorIdLUT = new Dictionary<short, string>()
        {
            { 4, "Light - outdoors"},
            { 5, "Pressure - Internal"},
            { 6, "Humidity - Internal"},
            { 7, "Air Temperature - Internal" },
            { 8, "Air Temperature - External" },
            { 9, "Humidity - External" },
            { 10, "Pressure - External" },
            { 11, "Soil Temperature 1" },
            { 12, "Soil Temperature 2" },
            { 13, "Soil Temperature 3" },
            { 13, "Soil Moisture 1" },
            { 14, "Soil Moisture 2" },
            { 15, "Soil Moisture 3" },
         };

    }
}
