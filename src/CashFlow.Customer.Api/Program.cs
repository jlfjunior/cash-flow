using CashFlow.Customer.Api.Application;
using CashFlow.Customer.Api.Application.Requests;
using CashFlow.Customer.Api.Application.Responses;
using CashFlow.Customer.Api.Domain.Repositories;
using CashFlow.Customer.Api.Infrastructure;
using CashFlow.Lib.EventBus;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICustomerRepository,  CustomerRepository>();
builder.Services.AddScoped<ICreateCustomer,  CreateCustomer>();
builder.Services.AddScoped<IUpdateCustomer,  UpdateCustomer>();

builder.Services.AddRabbitMQ(builder.Configuration);
builder.Services.Configure<MongoDbConfiguration>(builder.Configuration.GetSection("MongoDB"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();

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

app.Run();

public partial class Program { }
