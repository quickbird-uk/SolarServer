﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
        public string Location { get; set; }
    }
}
