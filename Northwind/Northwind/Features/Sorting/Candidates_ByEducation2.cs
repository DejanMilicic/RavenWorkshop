using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Sorting
{
    public class Candidates_ByEducation2 : AbstractIndexCreationTask<Candidate, Candidates_ByEducation2.Entry>
    {
        public class Entry
        {
            public string Name { get; set; }

            public int EducationForSorting { get; set; }
        }

        public Candidates_ByEducation2()
        {
            Map = candidates => from c in candidates
                let dict = new Dictionary<string, int>
                {
                    { "Doctor", 12 }
                }

                select new Entry
                {
                    Name = c.Name,
                    EducationForSorting = dict.ContainsKey(c.Education.ToString()) ? dict[c.Education.ToString()] : -1
                };
        }
    }
}
