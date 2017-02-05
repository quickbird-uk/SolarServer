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
            List<ChartSeries> returnlist = new List<ChartSeries>();
            watch.Stop();

            foreach (var doc in dbList)
            {

                var dateTime = doc.Elements.FirstOrDefault(el => el.Name == "UTCTimestamp")
                .Value
                .ToUniversalTime();

                if(dateTime < new DateTime(2016,0,0,0,0,0,0,DateTimeKind.Utc) || (dateTime > new DateTime(2019, 0, 0, 0, 0, 0, 0, DateTimeKind.Utc)))
                { continue; }

                long utcTimestamp = new DateTimeOffset(dateTime, TimeSpan.Zero).ToUnixTimeMilliseconds();

                foreach (var el in doc.Elements)
                {
                    //Items we don;t want to return
                    if (el.Name != "NodeId"
                        && el.Name != "_id"
                        && el.Name != "UploadTime"
                        && el.Name != "UTCTimestamp")
                    {
                        var collection = returnlist.FirstOrDefault(rl => rl.key == el.Name);
                        if (collection == null)
                        {
                            collection = new ChartSeries
                            {
                                key = el.Name,
                                values = new List<chartPoint>(dbList.Count)
                            };
                            returnlist.Add(collection);
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
                            if(el.Name == "Air Temperature - Internal")
                            {
                                if (point.y > 300)
                                    point.y = 410 - point.y; 
                            }
                        }

                        //These are erornous values, filter them
                        if(point.y > -1000 && point.y < 1000000)
                        collection.values.Add(point);
                    }
                }
            }

            return returnlist;
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


