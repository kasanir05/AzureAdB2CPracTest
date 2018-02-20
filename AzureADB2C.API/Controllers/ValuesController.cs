using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AzureADB2C.API;

namespace AzureADB2C.API.Controllers
{
    
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [Authorize]
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            var scopes = HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/scope")?.Value;
            if (!string.IsNullOrEmpty(Startup.ReadScope) && scopes != null && scopes.Split(' ').Any(s => s.Equals(Startup.ReadScope)))
            {
                return Ok(new string[] { "value1", "value2" });
            }
            else
                return View();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
