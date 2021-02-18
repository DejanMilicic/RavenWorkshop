using System;
using Raven.Client.Documents;
using Raven.Client.Documents.Subscriptions;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Exceptions.Documents.Subscriptions;

namespace Northwind.Features.Subscriptions
{
    public class SubscriptionLondonEmployees
    {
        public async Task Consume()
        {
            var store = new DocumentStore
            {
                Urls = new[] { "http://live-test.ravendb.net" },
                Database = "demo",
            };
            store.Initialize();

            try
            {
                await store.Subscriptions.GetSubscriptionStateAsync("LondonEmployees");
            }
            catch (SubscriptionDoesNotExistException)
            {
                await store.Subscriptions.CreateAsync(new SubscriptionCreationOptions<Employee>
                {
                    Name = "LondonEmployees",
                    Filter = employee => employee.Address.City == "London"
                });
            }

            var subscription = store.Subscriptions.GetSubscriptionWorker<Employee>(new SubscriptionWorkerOptions("LondonEmployees"));
            await subscription.Run(batch =>
            {
                foreach (var item in batch.Items)
                {
                    Console.WriteLine(item.Id);
                }
            });
        }
    }
}
