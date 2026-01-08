var builder = DistributedApplication.CreateBuilder(args);

// Add MongoDB with persistence
var mongodb = builder.AddMongoDB("mongodb")
    .WithMongoExpress()
    .WithDataVolume("cashflow-mongodb-data");

var cashflowDb = mongodb.AddDatabase("cashflow");

// Add RabbitMQ for messaging
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

// Add API with MongoDB and RabbitMQ references
builder.AddProject<Projects.CashFlow_Api>("apis")
    .WithReference(cashflowDb)
    .WithReference(rabbitmq);

// Add Worker with MongoDB and RabbitMQ references
builder.AddProject<Projects.CashFlow_Worker>("workers")
    .WithReference(cashflowDb)
    .WithReference(rabbitmq);

await builder.Build()
    .RunAsync();