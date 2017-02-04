using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;

using WebApiTest.Model;

namespace WebApiTest.Controllers 
{
    [Route("")]
    public class ReadingsController : Controller
    {
        DataAccess objds;

        public ReadingsController()
        {
            objds = new DataAccess(); 
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<BsonDocument> list = await objds.GetDatapoint();
            ViewData["Message"] = "Your application description page.";
            ViewData["list"] = list; 

            return View("Views/ReadingsView.cshtml");
        }

        public async Task<IActionResult> Get(int quantity)
        {
            List<BsonDocument> list = await objds.GetDatapoint(quantity);
            ViewData["Message"] = "Your application description page.";
            ViewData["list"] = list;

            return View("Views/ReadingsView.cshtml");
        }
    }
}
