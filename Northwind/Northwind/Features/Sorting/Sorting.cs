using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.Sorting
{
    public class Sorting
    {
        public void Seed()
        {
            Candidate highschool = new Candidate
            {
                Id = "highschool",
                Name = "Hsch 1",
                Education = EducationStatus.HighSchool
            };

            Candidate associate = new Candidate
            {
                Id = "associate",
                Name = "Ass A1",
                Education = EducationStatus.Associate
            };

            Candidate bachelor = new Candidate
            {
                Id = "bachelor",
                Name = "Bsc A",
                Education = EducationStatus.Bachelor
            };

            Candidate master = new Candidate
            {
                Id = "master",
                Name = "Msc A",
                Education = EducationStatus.Master
            };

            Candidate doctor = new Candidate
            {
                Id = "doctor",
                Name = "Dr. Y",
                Education = EducationStatus.Doctor
            };

            Candidate doctor2 = new Candidate
            {
                Id = "doctor2",
                Name = "Dr. X",
                Education = EducationStatus.Doctor
            };

            using var session = DocumentStoreHolder.Store.OpenSession();
            session.Store(highschool);
            session.Store(associate);
            session.Store(bachelor);
            session.Store(master);
            session.Store(doctor);
            session.Store(doctor2);
            session.SaveChanges();
        }
    }
}
