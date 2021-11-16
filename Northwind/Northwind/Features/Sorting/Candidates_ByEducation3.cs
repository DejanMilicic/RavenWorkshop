using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Sorting
{
    public static class EducationSorter
    {
        public static int GetSortValue(EducationStatus status)
        {
            switch (status.ToString())
            {
                case "HighSchool" : return 12;
                case "Associate" : return 14;
                case "Bachelor" : return 16;
                case "Master" : return 18;
                case "Doctor" : return 22;
                default : return -1;
            }
        }
    }

    public class Candidates_ByEducation3 : AbstractIndexCreationTask<Candidate, Candidates_ByEducation3.Entry>
    {
        public class Entry
        {
            public string Name { get; set; }

            public string Education { get; set; }

            public int EducationForSorting { get; set; }
        }

        public Candidates_ByEducation3()
        {
            Map = candidates => from c in candidates
                select new Entry
                {
                    Name = c.Name,
                    Education = c.Education.ToString(),
                    EducationForSorting = EducationSorter.GetSortValue(c.Education)
                };

            Conventions = new DocumentConventions
            {
                TypeIsKnownServerSide = t => t == typeof(EducationStatus)
            };

            AdditionalSources = new Dictionary<string, string>
            {
                {
                    "EducationStatus.cs",
                    @"
public enum EducationStatus
{
    HighSchool = 1,
    Associate = 2,
    Bachelor = 3,
    Master = 4,
    Doctor = 5
}
"
                },
                {
                    "EducationSorter.cs",
                    @"
public static class EducationSorter
{
    public static int GetSortValue(EducationStatus status)
    {
        switch (status.ToString())
        {
            case ""HighSchool"" : return 12;
            case ""Associate"" : return 14;
            case ""Bachelor"" : return 16;
            case ""Master"" : return 18;
            case ""Doctor"" : return 22;
            default : return -1;
        }
    }
}
"
                }
            };
        }
    }
}
