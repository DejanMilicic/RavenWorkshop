using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.InnerJoin.DepartmentEmployees
{
    public class Department
    {
        public string Id { get; set; }

        public string[] StoreIds { get; set; }
    }

    public class Employee
    {
        public string Id { get; set; }

        public string StoreId { get; set; }
    }
}
