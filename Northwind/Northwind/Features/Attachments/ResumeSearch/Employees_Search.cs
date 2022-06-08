using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Attachments.ResumeSearch
{
    public class Employees_Search : AbstractMultiMapIndexCreationTask
    {
        public Employees_Search()
        {
            AddMap<Employee>(employees =>
                from emp in employees
                let txt = AttachmentsFor(emp)
                            .Where(att => att.Name.EndsWith(".txt"))
                            .Select(att => LoadAttachment(emp, att.Name).GetContentAsString())
                select new
                {
                    _ = CreateField("Content", txt)
                }
            );

            Index("Content", FieldIndexing.Search);
        }
    }
}
