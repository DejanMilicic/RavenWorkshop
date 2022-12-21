using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.Patching.Upsert
{
    public class Appointment
    {
        public string Id { get; set; }

        public string Doctor { get; set; }

        public DateTime Time { get; set; }

        public List<Patient> Patients { get; set; } = new List<Patient>();
    }

    public class Patient
    {
        public string Name { get; set; }
    }
}
