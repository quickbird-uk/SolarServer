using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace WebApiTest.Model
{
    public class DataAccess
    {
        MongoClient _client;
        MongoServer _server;
        MongoDatabase _db;

        public DataAccess()
        {
            _client = new MongoClient("mongodb://solariot:HnZVqnuEjQUXuYSSvps5P1QOaCafSk6pfJEKzY2cRLKo1zdmFIIdQCZYuaLWUdk6BXahMjsPXIUWcB15Co5g1g==@solariot.documents.azure.com:10250/?ssl=true");
            _server = _client.GetServer();
            _db = _server.GetDatabase("1");
        }

        public IEnumerable<Node> GetNodes()
        {
            return _db.GetCollection<Node>("Nodes").FindAll();
        }


        public Node GetNode(int id)
        {
            var res = Query<Node>.EQ(p => p.Id, id);
            return _db.GetCollection<Node>("Nodes").FindOne(res);
        }

        public Node CreateNode(Node p)
        {
            _db.GetCollection<Node>("Nodes").Save(p);
            return p;
        }

        public void CreateDatepoint(List<Datapoint> dp)
        {
            List<BsonDocument> documents = new List<BsonDocument>(dp.Count); 

            foreach(var datapoint in dp)
            {
                BsonDocument document = datapoint.ToBsonDocument();

                foreach (var kv in datapoint.sensorReadings)
                {
                    document.Add(kv.Key, kv.Value); 
                }
                documents.Add(document);
            }

            _db.GetCollection("Datapoints").InsertBatch(documents);

        }

        public List<BsonDocument> GetDatapoint(int nodeID = -9000) // Magic Vlaue
        {
            int takeNUmber = 200; 
            List<Datapoint> dp = new List<Datapoint>(takeNUmber); 
             SortByBuilder sbb = new SortByBuilder();
            sbb.Ascending("_id");
            long number = _db.GetCollection<BsonDocument>("Datapoints").Count(); 

            var documents = _db.GetCollection<BsonDocument>("Datapoints").AsQueryable().Skip((int)(number - takeNUmber)).Take(takeNUmber).ToList();

            documents.Reverse();           
            return documents; 
        }

        public void UpdateNode(int id, Node p)
        {
            p.Id = id;
            var res = Query<Node>.EQ(pd => pd.Id, id);
            var operation = Update<Node>.Replace(p);
            _db.GetCollection<Node>("Nodes").Update(res, operation);
        }
        public void RemoveNode(int id)
        {
            var res = Query<Node>.EQ(e => e.Id, id);
            var operation = _db.GetCollection<Node>("Nodes").Remove(res);
        }

    }
}
