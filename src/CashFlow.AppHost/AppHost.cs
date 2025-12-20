var builder = DistributedApplication.CreateBuilder(args);

//var postgresDb = builder.AddPostgres("db");

builder.AddProject<Projects.CashFlow_Api>("apis");
//builder.AddProject<Projects.CashFlow_Worker>("workers");

await builder.Build()
    .RunAsync();