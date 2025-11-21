using CashFlow.Lib.EventBus;
using CashFlow.Transactions.Api;
using CashFlow.Transactions.Application;
using CashFlow.Transactions.Api.Endpoints;
using CashFlow.Transactions.Data;
using CashFlow.Transactions.Domain.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register application services
builder.Services.AddHostedService<CustomerCreatedConsumer>();

builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<ICreateAccount, CreateAccount>();
builder.Services.AddScoped<ICreateTransaction, CreateTransaction>();

builder.Services.AddRabbitMQ(builder.Configuration);
builder.Services.Configure<MongoDbConfiguration>(builder.Configuration.GetSection("MongoDB"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();

app.MapTransactionEndpoints();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.Run();

public partial class Program { }