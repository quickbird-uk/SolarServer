using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;

using WebApiTest.Model;

namespace WebApiTest.Controllers 
{
    [Route("api/Readings")]
    public class ReadingsController : Controller
    {
        DataAccess objds;

        public ReadingsController()
        {
            objds = new DataAccess(); 
        }

        [HttpGet]
        public IActionResult Get()
        {
            //List<Datapoint> list =
            ViewData["Message"] = "Your application description page.";
            ViewData["list"] = objds.GetDatapoint();

            return View("Views/ReadingsView.cshtml");
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
    }
}
