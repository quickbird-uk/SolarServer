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
        public async Task<IEnumerable<Node>> Get()
        {
            return await objds.GetNodes();
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
        public async Task<Node> Post([FromBody]Node p)
        {
            await objds.UpsertNode(p);
            return p;
        }

        public async Task<IActionResult> Put(int id, [FromBody]Node p)
        {
            var product = objds.GetNode(id);
            if (product == null)
            {

                return StatusCode(404); 
            }

            await objds.UpsertNode(p);
            return new OkResult();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = objds.GetNode(id);
            if (product == null)
            {
                return  NotFound();
            }

            await objds.RemoveNode(product.Id);
            return new OkResult();
        }
    }
}
