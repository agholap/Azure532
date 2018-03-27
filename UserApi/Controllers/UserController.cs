using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UserApi.Models;
using Swashbuckle.Swagger.Annotations;
namespace UserApi.Controllers
{
    public class UserController : ApiController
    {
        // GET: api/User
        [SwaggerOperation("GetAll")]
        public IEnumerable<User> Get()
        {
            return new User[]
            {
                new User{Id=1,FirstName=  "Mark", LastName="Waugh"},
                new User{Id=2,FirstName=  "Steve", LastName="Waugh"},
                new User{Id=3,FirstName=  "Shane", LastName="W"}
            };
        }
        // GET: api/User/5
        [SwaggerOperation("GetById")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/User
        [SwaggerOperation("Create")]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/User/5
        [SwaggerOperation("Update")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/User/5
        [SwaggerOperation("Delete")]
        public void Delete(int id)
        {
        }
    }
}
