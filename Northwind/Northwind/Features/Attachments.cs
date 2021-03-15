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

            AttachmentName[] attachmentNames = session.Advanced.Attachments.GetNames(employee);
            foreach (AttachmentName attachmentName in attachmentNames)
            {
                string name = attachmentName.Name;
                string contentType = attachmentName.ContentType;
                string hash = attachmentName.Hash;
                long size = attachmentName.Size;

                Console.WriteLine($"{name} \t {contentType} \t {hash} \t {size}");
            }
        }
    }
}
