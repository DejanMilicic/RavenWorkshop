using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Northwind.Models.Entity;

namespace Northwind.Features.GraphQueries
{
    public class GraphQueries
    {
        public void Q1()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var result = session.Advanced.RawQuery<JObject>(@"
            match
                (Products as products)-
                [Category as category]->
                (Categories as categories 
                    where Name=""Confections"" 
                        or Name = ""Condiments"")
            ").ToArray();

            Console.WriteLine(result[0]);
        }

        public void Q2()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var result = session.Advanced.RawQuery<JObject>(@"
                match (Employees as e) -[ReportsTo]-> (Employees as m)
            ").ToArray();

            Employee employee = JsonConvert.DeserializeObject<Employee>(result[0]["e"].ToString());

            Console.WriteLine(employee.FirstName);
            Console.WriteLine(result[0]["e"]);
        }
    }
}
