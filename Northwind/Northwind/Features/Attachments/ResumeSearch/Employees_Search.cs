using System.IO;
using System.IO.Compression;
using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Attachments.ResumeSearch
{
    public class Employees_Search : AbstractMultiMapIndexCreationTask
    {
        public class Entry
        {
            public string Content { get; set; }
        }

        public Employees_Search()
        {
            //AddMap<Employee>(employees =>
            //    from emp in employees
            //    let txt = AttachmentsFor(emp)
            //                .Where(att => att.Name.EndsWith(".txt"))
            //                .Select(att => LoadAttachment(emp, att.Name).GetContentAsString())
            //    select new
            //    {
            //        _ = CreateField("Content", txt)
            //    }
            //);

            AddMap<Employee>(employees =>
                from emp in employees
                let zipStream = AttachmentsFor(emp)
                    .Where(att => att.Name.EndsWith(".zip"))
                    .Select(att => LoadAttachment(emp, att.Name).GetContentAsStream())
                    .First()

                //let gzipstream = new ZipArchive(zipStream)

                let content = "aaa" //new StreamReader(gzipstream)

                select new
                {
                    _ = CreateField("Content", content)
                }

            //select new Entry
            //{
            //    //Content = //(new StreamReader(new GZipStream(zipStream, CompressionMode.Decompress))).ReadToEnd()
            //}
            );

            Index("Content", FieldIndexing.Search);
        }
    }
}
