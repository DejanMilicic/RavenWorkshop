using Northwind.Models.Entity;
using System.Linq;
using Raven.Client.Documents.Indexes.Counters;

namespace Northwind.Features.Attachments
{
    public class Employees_ByLikes : AbstractCountersIndexCreationTask<Employee>
    {
        public Employees_ByLikes()
        {
            AddMap("Likes", counters => from counter in counters
                select new
                {
                    Likes = counter.Value,
                    Name = counter.Name,
                    Employee = counter.DocumentId
                });
        }
    }
}
