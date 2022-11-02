using System.Linq;
using FluentAssertions;
using Northwind.Models.Entity;
using Raven.Client.Documents.Operations;
using Xunit;

namespace Northwind.Features.Testing
{
    [Trait("Creating new employee", "Happy path")]
    public class IntegrationTests : Fixture
    {
        private readonly Employee employee;

        public IntegrationTests()
        {
            employee = new Employee
            {
                Id = "Employees/JaneDoe",
                FirstName = "Jane",
                LastName = "Doe"
            };
            
            SmallApp app = new SmallApp(Store);
            app.CreateNewEmployee(employee);

            // prevent operations that might result in stale results
            WaitForIndexing(Store);
            
            // enable ad-hoc studio during debugging session
            WaitForUserToContinueTheTest(Store);
        }

        [Fact(DisplayName = "1. One document was created in the database")]
        public void OneDocumentCreated()
        {
            Store.Maintenance.Send(new GetCollectionStatisticsOperation())
                .CountOfDocuments.Should().Be(1);
        }

        [Fact(DisplayName = "2. Created document has expected content")]
        public void DocumentHasExpectedContent()
        {
            using var session = Store.OpenSession();

            Employee dbEmployee = session.Query<Employee>().Single();

            dbEmployee.Id.Should().Be(employee.Id);
            dbEmployee.FirstName.Should().Be(employee.FirstName);
            dbEmployee.LastName.Should().Be(employee.LastName);
        }

        [Fact(DisplayName = "3. One session, one request")]
        public void OneSessionOneRequest()
        {
            SessionsRecorded.Count.Should().Be(1); // one sessions opened
            SessionsRecorded.Values.Single().Should().Be(1); // one request in that session
        }
    }
}
