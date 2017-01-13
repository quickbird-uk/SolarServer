using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApiTest.Model;
using System.Text;

namespace WebApiTest.Controllers
{
    [Route("api/[controller]/{id}")]
    public class BatchUploadController : Controller
    {
        DataAccess objds;

        public BatchUploadController()
        {
            objds = new DataAccess();
        }


        // POST api/batchUplaod
        [HttpPut]
        public IActionResult Put(int id,[FromBody]List<Model.Datapoint> list)
        {
            int nodeId = id; 
            var node = objds.GetNode(nodeId);
            if (node == null)
            {

                node = new Node
                {
                    Id = nodeId,
                    LastConnection = DateTime.UtcNow,
                    NickName = ""
                };
                objds.CreateNode(node);
            }
            else
                node.LastConnection = DateTime.UtcNow;

            foreach(Datapoint dp in list)
            {
                //We know we will never exceed unix seconds range, and that the device works with Unix seconds
                int unixSeconds = (int) (new DateTimeOffset(dp.UTCTimestamp, TimeSpan.Zero).ToUnixTimeSeconds()); 
                //Move unix seconds to the top 32 bit 
                dp.id = ((long)unixSeconds << 32);
                //Put nodeID in the lower 32 bit
                dp.id += nodeId; 
            }

            objds.CreateDatepoint(list); 

            return StatusCode(201); 
        }

        private string[] ParseAuthHeader(string authHeader)
        {
            // Check this is a Basic Auth header
            if (authHeader == null || authHeader.Length == 0 || !authHeader.StartsWith("Basic")) return null;

            // Pull out the Credentials with are seperated by ':' and Base64 encoded
            string base64Credentials = authHeader.Substring(6);
            string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(base64Credentials)).Split(new char[] { ':' });

            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[0])) return null;

            // Okay this is the credentials
            return credentials;
        }
    }
}
