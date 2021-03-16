using System;
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
            try
            {
                await DocumentStoreHolder.Store.Subscriptions.GetSubscriptionStateAsync("LondonEmployees");
            }
            catch (SubscriptionDoesNotExistException)
            {
                await DocumentStoreHolder.Store.Subscriptions.CreateAsync(new SubscriptionCreationOptions<Employee>
                {
                    Name = "LondonEmployees",
                    Filter = employee => employee.Address.City == "London"
                });
            }

            var subscription = DocumentStoreHolder.Store.Subscriptions.GetSubscriptionWorker<Employee>(
                new SubscriptionWorkerOptions("LondonEmployees"));
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
