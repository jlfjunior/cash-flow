using CashFlow.Features.Customers;
using CashFlow.Features.Customers.Requests;
using CashFlow.Features.Customers.Responses;

namespace CashFlow.Api.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app)
    {
        app.MapGet("/customers", async () =>
            {
                var customers = Enumerable.Empty<CreateCustomerResponse>();

                return Results.Ok(customers);
            })
            .WithName("AddCustomer");

        app.MapPost("/customers", async (CreateCustomerRequest request, ICreateCustomer service, CancellationToken ct) =>
            {
                var customer = await service.ExecuteAsync(request, ct);
    
                return Results.Ok(customer);
            })
            .WithName("CreateCustomer");

        app.MapPost("/customers/{Id}", async (Guid id, CustomerRequest request, IUpdateCustomer service, CancellationToken ct) =>
            {

                var updateCustomerRequest = new UpdateCustomerRequest(id)
                {
                    FullName = request.FullName,
                };
                var customer = await service.ExecuteAsync(updateCustomerRequest, ct);
    
                return Results.Ok(customer);
            })
            .WithName("UpdateCustomer");
    }
}