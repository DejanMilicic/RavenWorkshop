using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Models.Entity;
using Raven.Client.Documents;

namespace NorthwindApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IDocumentStore _store;

        public EmployeesController(IDocumentStore store)
        {
            _store = store;
        }

        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            using var session = _store.OpenSession();

            return session.Query<Employee>().ToArray();
        }
    }
}
