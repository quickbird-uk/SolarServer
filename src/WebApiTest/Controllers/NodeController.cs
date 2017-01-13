using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;

using WebApiTest.Model;

namespace WebApiTest.Controllers 
{
    [Route("api/Nodes")]
    public class NodeController : Controller
    {
        DataAccess objds;

        public NodeController()
        {
            objds = new DataAccess(); 
        }

        [HttpGet]
        public IEnumerable<Node> Get()
        {
            return objds.GetNodes();
        }

        public IActionResult Get(int id)
        {
            var product = objds.GetNode(id);
            if (product == null)
            {
                return NotFound();
            }
            return new ObjectResult(product);
        }

        [HttpPost]
        public Node Post([FromBody]Node p)
        {
            objds.CreateNode(p);
            return p;
        }

        public IActionResult Put(int id, [FromBody]Node p)
        {
            var product = objds.GetNode(id);
            if (product == null)
            {

                return StatusCode(404); 
            }

            objds.UpdateNode(id, p);
            return new OkResult();
        }

        public IActionResult Delete(int id)
        {
            var product = objds.GetNode(id);
            if (product == null)
            {
                return  NotFound();
            }

            objds.RemoveNode(product.Id);
            return new OkResult();
        }
    }
}
