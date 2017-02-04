using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Linq;

namespace WebApiTest.Model
{
    public class DataAccess
    {
        MongoClient _client = new MongoClient();
        IMongoDatabase _db;



        public DataAccess()
        {
            _client = new MongoClient("mongodb://solariot:HnZVqnuEjQUXuYSSvps5P1QOaCafSk6pfJEKzY2cRLKo1zdmFIIdQCZYuaLWUdk6BXahMjsPXIUWcB15Co5g1g==@solariot.documents.azure.com:10250/?ssl=true");
            _db = _client.GetDatabase("1");
            Console.Out.WriteLine("Connection has been established.\n");
        }

        public async Task<IEnumerable<Node>> GetNodes()
        {
           // var filter = new BsonDocument("x", new BsonDocument("$gte", 100));
            return await _db.GetCollection<Node>("Nodes").AsQueryable().ToListAsync();
        }


        public async Task<Node> GetNode(int id)
        {
            var item  = await _db.GetCollection<Node>("Nodes").FindAsync(node => node.Id == id);
            return item.SingleOrDefault(); 
        }

        public async Task<Node> UpsertNode(Node p)
        { 
            var x = await _db.GetCollection<Node>("Nodes").ReplaceOneAsync(item => item.Id == p.Id, p);

            return p;
        }

        public async Task CreateDatapoint(List<Datapoint> dp)
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

            await _db.GetCollection<BsonDocument>("Datapoints").InsertManyAsync(documents);

        }

        public async Task<List<BsonDocument>> GetDatapointsAll() // Magic Vlaue
        {
            var documents = await _db.GetCollection<BsonDocument>("Datapoints").AsQueryable().ToListAsync();

            documents.Reverse();           
            return documents; 
        }

        public async Task<List<BsonDocument>> GetDatapointsOfNode(int id) // Magic Vlaue
        {
            var filter = Builders<BsonDocument>.Filter.Eq("NodeID", id);


            var documents = await _db.GetCollection<BsonDocument>("Datapoints")
                .FindAsync(filter);

            var docs = documents.ToList();
            docs.Reverse();
            return docs;
        }

        public List<BsonDocument> GetDatapoint(int nodeID = -9000) // Magic Vlaue
        {
            int takeNUmber = 200;

            var documents = _db.GetCollection<BsonDocument>("Datapoints").AsQueryable().Take(takeNUmber).ToList();

            documents.Reverse();
            return documents;
        }

        public async Task RemoveNode(int id)
        {
            var filter = Builders<Node>.Filter.Eq(s => s.Id, id);
            var operation = await _db.GetCollection<Node>("Nodes").DeleteOneAsync(filter);
        }

    }
}
