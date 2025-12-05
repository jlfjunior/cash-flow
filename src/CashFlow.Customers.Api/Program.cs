using CashFlow.Customers.Application;
using CashFlow.Customers.Application.Requests;
using CashFlow.Customers.Application.Responses;
using CashFlow.Customers.Data;
using CashFlow.Customers.Domain.Repositories;
using CashFlow.Lib.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<CustomerContext>(options => options.UseNpgsql(connectionString: builder.Configuration.GetConnectionString("CustomerContext")));
builder.Services.AddScoped<IRepository,  Repository>();
builder.Services.AddScoped<ICreateCustomer,  CreateCustomer>();
builder.Services.AddScoped<IUpdateCustomer,  UpdateCustomer>();

builder.Services.AddRabbitMQ(builder.Configuration);

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();
app.MapGet("/health", async (HealthCheckService service) =>
    {
        var report = await service.CheckHealthAsync();
        return Results.Json(new
        {
            status = report.Status.ToString()
        });
    })
    .WithName("Health Check")
    .WithSummary("Shows the API health status (detailed).")
    .WithDescription("Runs the registered health checks and returns a JSON report suitable for reading in Swagger / OpenAPI UI.");

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
