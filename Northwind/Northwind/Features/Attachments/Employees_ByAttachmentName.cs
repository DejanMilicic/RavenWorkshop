using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Northwind.Features.Attachments
{
    public class Employees_ByAttachmentName : AbstractIndexCreationTask<Employee, Employees_ByAttachmentName.Entry>
    {
        public class Entry
        {
            public string[] Query { get; set; }
        }

        public Employees_ByAttachmentName()
        {
            Map = employees => from employee in employees

                let attachmentNames = AttachmentsFor(employee)

                select new Entry
                {
                    Query = attachmentNames.Select(x => x.Name).ToArray()
                };

            Index(x => x.Query, FieldIndexing.Search);
        }
    }
}
