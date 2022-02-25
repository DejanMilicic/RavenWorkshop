using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Infra;
using NorthwindApi.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace NorthwindApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IDocumentStore _store;

    public OrdersController(IDocumentStore store)
    {
        _store = store;
    }

    [HttpGet]
    public AsyncQueryResult<Order> Get()
    {
        IAsyncDocumentSession session = _store.OpenAsyncSession();

        var query = session.Advanced.AsyncDocumentQuery<Order>(collectionName: "Orders");

        return new AsyncQueryResult<Order>(session, query);
    }
}