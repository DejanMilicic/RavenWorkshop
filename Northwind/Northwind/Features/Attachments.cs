using System;
using Northwind.Models.Entity;
using Raven.Client.Documents.Operations.Attachments;

namespace Northwind.Features
{
    public class Attachments
    {
        public void GetAttachmentNames()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var employee = session.Load<Employee>("employees/8-A");
            var attNames = session.Advanced.Attachments.GetNames(employee);

            foreach (AttachmentName attName in attNames)
            {
                Console.WriteLine(attName.Name);
            }
        }
    }
}
