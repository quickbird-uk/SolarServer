using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;

using WebApiTest.Model;
using System.Dynamic;
using System.Diagnostics;

namespace WebApiTest.Controllers
{
    [Route("api/readings_to_graph")]
    public class ReadingsToGraphController : Controller
    {
        DataAccess objds;

        public ReadingsToGraphController()
        {
            objds = new DataAccess();
        }

        [HttpGet("{id}")]
        public async Task<List<ChartSeries>> Get(int id)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();
            List<BsonDocument> dbList = await objds.GetDatapointsOfNode(id);
            List<ChartSeries> tempList = new List<ChartSeries>();
            List<ChartSeries> returnList = new List<ChartSeries>(); 
            watch.Stop();

            foreach (var doc in dbList)
            {

                var dateTime = doc.Elements.FirstOrDefault(el => el.Name == "UTCTimestamp")
                .Value
                .ToUniversalTime();

                if(dateTime < new DateTime(2016,1,1,1,1,1,1,DateTimeKind.Utc) || (dateTime > new DateTime(2019, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc)))
                { continue; }

                long utcTimestamp = new DateTimeOffset(dateTime, TimeSpan.Zero).ToUnixTimeMilliseconds();

                foreach (var el in doc.Elements)
                {
                    //Items we don't want to return
                    if (el.Name != "NodeId"
                        && el.Name != "_id"
                        && el.Name != "UploadTime"
                        && el.Name != "UTCTimestamp")
                    {
                        var collection = tempList.FirstOrDefault(rl => rl.key == el.Name);
                        if (collection == null)
                        {
                            collection = new ChartSeries
                            {
                                key = el.Name,
                                values = new List<chartPoint>(dbList.Count)
                            };
                            tempList.Add(collection);
                        }

                        chartPoint point = new chartPoint
                        {
                            x = utcTimestamp
                        };



                        if (el.Value.IsInt32)
                            point.y = el.Value.AsInt32;
                        else if (el.Value.IsDouble)
                        {
                            point.y = (float)el.Value.ToDouble();
                        }

                        //We use BME 280 and the code for mbed has an error for negarive temperatures.
                        if (el.Name.Contains("Air Temperature") && point.y > 360)
                        {
                            point.y = point.y - 410;
                        }

                        //Use the soil moisture calibration equasion to convert raw readings to volumetric moisture content
                        else if(el.Name.Contains("Soil Moisture"))
                        {
                            point.y = (point.y - 340) / 5.50f; 
                        }

                        //Change light to kilo-lux, stops it screwing up the scale on all graphs
                        else if(el.Name.Contains("Light - outdoors"))
                        {                            
                            point.y = point.y / 1000;
                        }

                        //These are erornous values, filter them
                        if (point.y > -1000 && point.y < 1000000)
                        collection.values.Add(point);
                    }
                }
            }

            //Remove lists that contain no values
            foreach(ChartSeries series in tempList)
            {
                if (series.values.Count > 0)
                    returnList.Add(series);
            }
            return returnList;
        }
    }

    public class ChartSeries{
        public string key;
        public List<chartPoint> values; 
    }

    public class chartPoint
    {
        public long x;
        public float y; 
    }
}


