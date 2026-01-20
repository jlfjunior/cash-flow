using CashFlow.Data;
using CashFlow.Data.Repositories;
using CashFlow.Domain.Repositories;
using CashFlow.Features.Customers;
using CashFlow.Lib.EventBus;
using CashFlow.Worker;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Add MongoDB using Aspire integration
builder.AddMongoDBClient("mongodb");

// Configure MongoDB context
var mongoDatabaseName = builder.Configuration.GetValue<string>("MongoDB:DatabaseName") ?? "cashflow";

builder.Services.AddScoped<CashFlowMongoContext>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return new CashFlowMongoContext(client, mongoDatabaseName);
});

builder.Services.AddRabbitMQ(builder.Configuration);

// Register repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Register Features (Application services)
builder.Services.AddScoped<IPublishCustomerCreated, PublishCustomerCreated>();
builder.Services.AddScoped<IPublishCustomerUpdated, PublishCustomerUpdated>();

// Register workers
builder.Services.AddHostedService<CreateAccountWorker>();
builder.Services.AddHostedService<PublishCustomerCreatedWorker>();
builder.Services.AddHostedService<PublishCustomerUpdatedWorker>();

var host = builder.Build();

await host.RunAsync();