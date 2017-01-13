using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

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


        public Node GetNode(long id)
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
            _db.GetCollection<Datapoint>("Datapoints").Save(dp); 
        }

        public void UpdateNode(long id, Node p)
        {
            p.Id = id;
            var res = Query<Node>.EQ(pd => pd.Id, id);
            var operation = Update<Node>.Replace(p);
            _db.GetCollection<Node>("Nodes").Update(res, operation);
        }
        public void RemoveNode(long id)
        {
            var res = Query<Node>.EQ(e => e.Id, id);
            var operation = _db.GetCollection<Node>("Nodes").Remove(res);
        }

    }
}
