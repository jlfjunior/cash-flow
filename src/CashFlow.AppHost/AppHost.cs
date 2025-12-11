var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CashFlow_Transactions_Api>("transactions");
//builder.AddProject<Projects.CashFlow_Consolidation_Api>("consolidation");
//builder.AddProject<Projects.CashFlow_Customers_Api>("customers");

builder.Build().Run();