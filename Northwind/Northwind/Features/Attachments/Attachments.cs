using System;
using System.IO;
using Northwind.Models.Entity;
using Raven.Client.Documents.Operations.Attachments;

namespace Northwind.Features.Attachments
{
    public class Attachments
    {
        public void GetAttachmentNames()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var employee = session.Load<Employee>("employees/8-A");

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
        public void GetAttachmentContent()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            
            using AttachmentResult photo = session.Advanced.Attachments.Get("employees/8-A", "photo.jpg");
            
            Stream stream = photo.Stream;
            var ms = new MemoryStream();
            stream.CopyTo(ms);

            AttachmentDetails attachmentDetails = photo.Details;
            string name = attachmentDetails.Name;
            string contentType = attachmentDetails.ContentType;
            string hash = attachmentDetails.Hash;
            long size = attachmentDetails.Size;
            string documentId = attachmentDetails.DocumentId;
            string changeVector = attachmentDetails.ChangeVector;

            Console.WriteLine($"{ms.Length} \t {name} \t {contentType} \t {hash} \t {size} \t {documentId} \t {changeVector}");
        }

        public void StoreAttachment()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            using FileStream file1 = File.Open("./Features/Attachments/att1.txt", FileMode.Open);
            using FileStream file2 = File.Open("./Features/Attachments/att2.txt", FileMode.Open);

            var emp = new Employee()
            {
                FirstName = "Jonathan",
                LastName = "Doe"
            };
            session.Store(emp);

            session.Advanced.Attachments.Store(emp.Id, "att1.txt", file1, "text/plain");
            session.Advanced.Attachments.Store(emp.Id, "att2.txt", file2, "text/plain");

            session.SaveChanges();
        }
    }
}
