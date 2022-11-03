using Northwind.Models.Entity;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Transactions
{
    public class Transactions
    {
        public void LocalNodeTransaction()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee emp = new Employee { FirstName = "Jane", LastName = "Doe" };

            session.Store(emp);
            session.SaveChanges();
        }

        public void ClusterWideTransaction()
        {
            using var session = DocumentStoreHolder.Store.OpenSession(
                new SessionOptions { TransactionMode = TransactionMode.ClusterWide });

            Employee emp = new Employee { FirstName = "Jane", LastName = "Doe" };

            session.Store(emp);
            session.SaveChanges();
        }

        public void WaitForReplication()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee emp = new Employee { FirstName = "Jane", LastName = "Doe" };

            // SaveChanges() should wait for two nodes to confirm
            // they accepted changes before returning
            session.Advanced.WaitForReplicationAfterSaveChanges(replicas: 2);

            session.Store(emp);
            session.SaveChanges();
        }
    }
}
