using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using Northwind.Features.Sorting;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq.Indexing;

namespace Northwind.Features.Polymorphism
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
