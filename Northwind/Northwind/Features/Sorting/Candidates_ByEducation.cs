using System.Collections.Generic;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Sorting
{
    public class Candidates_ByEducation : AbstractJavaScriptIndexCreationTask
    {
        public Candidates_ByEducation()
        {
            Maps = new HashSet<string>
            {
                @"map('Candidates', function (candidate){ 

                    function EducationSortingNdx(c)
                    {
                        if (c.Education == 'HighSchool') return 12;
                        if (c.Education == 'Associate') return 14;
                        if (c.Education == 'Bachelor') return 16;
                        if (c.Education == 'Master') return 18;
                        if (c.Education == 'Doctor') return 22;
                        return -1;
                    }

                    return { 
                        Education : candidate.Education,
                        Name: candidate.Name,
                        EducationForSorting : EducationSortingNdx(candidate)
                    };
                })",
            };
        }
    }
}
