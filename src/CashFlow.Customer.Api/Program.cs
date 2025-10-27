using CashFlow.Customer.Api;
using CashFlow.Customer.Api.Domain;
using CashFlow.Customer.Api.Domain.Services;
using CashFlow.Customer.Api.Domain.Repositories;
using CashFlow.Lib.EventBus;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICustomerRepository,  CustomerRepository>();
builder.Services.AddScoped<ICreateCustomer,  CustomerService>();

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
    var customer = await service.HandleAsync(request, ct);
    
    return Results.Ok(customer);
})
.WithName("GetCustomer");

app.Run();

public partial class Program { }
