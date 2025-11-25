using CashFlow.Lib.EventBus;
using CashFlow.Transactions.Application;
using CashFlow.Transactions.Data;
using CashFlow.Transactions.Domain.Repositories;
using CashFlow.Transactions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<TransactionContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("TransactionContext")));

        // Register application services
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<ICreateAccount, CreateAccount>();

        // Register RabbitMQ
        services.AddRabbitMQ(context.Configuration);

        // Register hosted service (consumer)
        services.AddHostedService<CustomerCreatedConsumer>();
    })
    .Build();

await host.RunAsync();
