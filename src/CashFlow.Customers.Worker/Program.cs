using CashFlow.Customers.Application;
using CashFlow.Customers.Data;
using CashFlow.Customers.Worker;
using CashFlow.Lib.EventBus;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<PublishCustomerCreatedWorker>();
builder.Services.AddHostedService<PublishCustomerUpdatedWorker>();

builder.Services.AddDbContext<CustomerContext>(options => options.UseNpgsql(connectionString: builder.Configuration.GetConnectionString("CustomerContext")));
builder.Services.AddRabbitMQ(builder.Configuration);
builder.Services.AddTransient<IPublishCustomerCreated, PublishCustomerCreated>();
builder.Services.AddTransient<IPublishCustomerUpdated, PublishCustomerUpdated>();

var host = builder.Build();
host.Run();