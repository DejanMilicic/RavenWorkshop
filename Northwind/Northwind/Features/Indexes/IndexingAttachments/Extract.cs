using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Northwind.Features.Indexes.IndexingAttachments
{
    public static class Extract
    {
        public static string DataFromZipFile(Stream attachmentObject)
        {
            StringBuilder builder = new();

            using var zip = new ZipArchive(attachmentObject, ZipArchiveMode.Read);

            foreach (var file in zip.Entries)
            {
                using var fileStream = file.Open();
                StreamReader reader = new(fileStream);
                builder.Append(reader.ReadToEnd());
                builder.Append(Environment.NewLine);
            }

            return builder.ToString();
        }
    }
}
