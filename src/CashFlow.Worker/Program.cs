using CashFlow.Consolidation.Application;
using CashFlow.Consolidation.Data;
using CashFlow.Consolidation.Domain.Repositories;
using CashFlow.Lib.EventBus;
using CashFlow.Worker;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Add MongoDB using Aspire integration
builder.AddMongoDBClient("mongodb");

// Configure MongoDB contexts
var mongoDatabaseName = builder.Configuration.GetValue<string>("MongoDB:DatabaseName") ?? "cashflow";

builder.Services.AddScoped<ConsolidationMongoContext>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return new ConsolidationMongoContext(client, mongoDatabaseName);
});

builder.Services.AddRabbitMQ(builder.Configuration);

// Register Consolidation services
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IDailyClosureService, DailyClosureService>();
builder.Services.AddScoped<ICreateTransaction, CreateTransaction>();

// Register workers
builder.Services.AddHostedService<CreateAccountWorker>();
builder.Services.AddHostedService<CreateTransactionWorker>();
builder.Services.AddHostedService<PublishCustomerCreatedWorker>();
builder.Services.AddHostedService<PublishCustomerUpdatedWorker>();

var host = builder.Build();

await host.RunAsync();