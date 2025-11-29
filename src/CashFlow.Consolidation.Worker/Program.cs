using CashFlow.Consolidation.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<CreateTransactionWorker>();

var host = builder.Build();
host.Run();