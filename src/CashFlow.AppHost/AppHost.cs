var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CashFlow_Transactions_Api>("transactions");

builder.Build().Run();