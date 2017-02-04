using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel; 

namespace WebApiTest.Model
{
    public class Node
    {

        [BsonId]
        public int Id { get; set; }
        
        [BsonElement("NickName")]
        public string NickName { get; set; }

        [BsonElement("LastConnection")]
        public DateTime LastConnection { get; set; }

        [BsonElement("Location")]
        public GeoJson2DGeographicCoordinates Location { get; set; }
    }
}
