using CashFlow.Transaction.Api.Domain.Services;
using CashFlow.Transaction.Api.Endpoints;
using CashFlow.Transaction.Api.Infrastructure.EventBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register application services
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddRabbitMQ(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.MapTransactionEndpoints();

app.Run();

public partial class Program { }