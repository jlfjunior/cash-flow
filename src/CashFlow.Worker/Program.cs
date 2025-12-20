using CashFlow.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<CreateAccountWorker>();
builder.Services.AddHostedService<CreateTransactionWorker>();
builder.Services.AddHostedService<PublishCustomerCreatedWorker>();
builder.Services.AddHostedService<PublishCustomerUpdatedWorker>();

var host = builder.Build();

await host.RunAsync();