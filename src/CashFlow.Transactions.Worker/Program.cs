using CashFlow.Lib.EventBus;
using CashFlow.Transactions.Application;
using CashFlow.Transactions.Data;
using CashFlow.Transactions.Domain.Repositories;
using CashFlow.Transactions.Worker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<CreateAccountWorker>();

builder.Services.AddDbContext<TransactionContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TransactionContext")));

// Register application services
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<ICreateAccount, CreateAccount>();

// Register RabbitMQ
builder.Services.AddRabbitMQ(builder.Configuration);

var host = builder.Build();
host.Run();