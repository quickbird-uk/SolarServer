using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApiTest.Controllers
{
    [Route("api/[controller]")]
    public class BatchUploadController : Controller
    {
        // GET api/batchUplaod
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/batchUplaod/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/batchUplaod
        [HttpPost]
        public IActionResult Post([FromBody]List<Model.Datapoint> list)
        {
            bool idPresent =  Request.Headers.Any(header => header.Key == "Authorization");
            if (idPresent == false) {
                return BadRequest(); 
            }
            

            return StatusCode(201); 
        }

        // PUT api/batchUplaod/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/batchUplaod/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
