using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;

using WebApiTest.Model;
using System.Dynamic;

namespace WebApiTest.Controllers 
{
    [Route("api/Readings")]
    public class ReadingsJsonController : Controller
    {
        DataAccess objds;

        public ReadingsJsonController()
        {
            objds = new DataAccess(); 
        }

        [HttpGet]
        public List<ExpandoObject> Get()
        {
            List<BsonDocument> list = objds.GetDatapointsAll();
            List<ExpandoObject> returnlist = new List<ExpandoObject>(list.Count);
            foreach (var doc in list) {
                dynamic expando = new ExpandoObject();
                var x = expando as IDictionary<string, object>; 
                foreach (var el in doc.Elements)
                {
                    if (el.Value.IsInt32)
                        x.Add(el.Name, el.Value.AsInt32);
                    else if(el.Value.IsDouble)
                        x.Add(el.Name, el.Value.ToDouble());
                    else if(el.Value.IsValidDateTime)
                        x.Add(el.Name, el.Value.ToUniversalTime());

                }
                returnlist.Add(expando); 
            }

            return returnlist;
        }
    }
}
