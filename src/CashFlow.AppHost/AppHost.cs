var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CashFlow_Api>("apis");
builder.AddProject<Projects.CashFlow_Worker>("workers");

builder.Build().Run();