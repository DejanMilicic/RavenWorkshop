using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.InnerJoin.DepartmentEmployees
{
    public static class Seed
    {
        public static void Data(IDocumentStore store)
        {
            using var session = store.OpenSession();

            session.Store(
                new Department
                {
                    Id = "Departments/NorthAmerica",
                    StoreIds = new string[] { "Stores/Chicago", "Stores/NewYork" }
                }
            );

            session.Store(
                new Department
                {
                    Id = "Departments/Europe",
                    StoreIds = new string[] { "Stores/London", "Stores/Paris" }
                }
            );

            session.Store(
                new Employee
                {
                    Id = "Employees/Joe",
                    StoreId = "Stores/Chicago"
                }
            );

            session.Store(
               new Employee
               {
                   Id = "Employees/Frank",
                   StoreId = "Stores/NewYork"
               }
            );

            session.Store(
               new Employee
               {
                   Id = "Employees/Jane",
                   StoreId = "Stores/NewYork"
               }
            );

            session.Store(
               new Employee
               {
                   Id = "Employees/Vivian",
                   StoreId = "Stores/Paris"
               }
            );

            session.Store(
               new Employee
               {
                   Id = "Employees/July",
                   StoreId = "Stores/London"
               }
            );

            session.SaveChanges();

            var departments = new List<Department>
            {
                new Department
                {
                    Id = "departments/1",
                    StoreIds = new string[] { "stores/1", "stores/2" }
                },
                new Department
                {
                    Id = "departments/2",
                    StoreIds = new string[] { "stores/1", "stores/3" }
                },
                new Department
                {
                    Id = "departments/3",
                    StoreIds = new string[] { "stores/2", "stores/3" }
                }
            };

            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = "employees/1",
                    StoreId = "stores/1"
                },
                new Employee
                {
                    Id = "employees/2",
                    StoreId = "stores/2"
                },
                new Employee
                {
                    Id = "employees/3",
                    StoreId = "stores/3"
                }
            };
        }
    }
}
