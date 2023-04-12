using System;
using Raven.Client.Documents;

namespace Northwind.Features.Identifiers;

public class PaypalUser
{
    public string Email { get; set; }

    public string Name { get; set; }
}

public static class EmailAsIdentifier
{
    public static void Demo()
    {
        #region Customize Store

        using var store = new DocumentStore { Urls = new[] { "http://127.0.0.1:8080" }, Database = "demo" };

        var defaultFindIdentityProperty = store.Conventions.FindIdentityProperty;

        store.Conventions.FindIdentityProperty = property =>
            typeof(PaypalUser).IsAssignableFrom(property.DeclaringType)
                ? property.Name == "Email"
                : defaultFindIdentityProperty(property);

        store.Initialize();

        #endregion

        using var session = store.OpenSession();

        PaypalUser user = new PaypalUser
        {
            Email = "john@doe.com",
            Name = "John"
        };

        session.Store(user);
        var userId = session.Advanced.GetDocumentId(user);
        Console.WriteLine($"id(user) = {userId}");

        session.SaveChanges();
            
        session.Advanced.Evict(user);
        PaypalUser loadedUser = session.Load<PaypalUser>(userId);
        Console.WriteLine($"loadedUser.Email = {loadedUser.Email}");
    }
}
