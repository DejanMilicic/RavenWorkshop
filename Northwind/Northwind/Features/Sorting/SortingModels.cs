using System.Collections.Generic;

namespace Northwind.Features.Sorting
{
    public enum EducationStatus
    {
        HighSchool = 1,
        Associate = 2,
        Bachelor = 3,
        Master = 4,
        Doctor = 5
    }

    public class Candidate
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public EducationStatus Education { get; set; }
    }
}
