using CashFlow.Lib.EventBus;
using CashFlow.Transactions.Api;
using CashFlow.Transactions.Application;
using CashFlow.Transactions.Api.Endpoints;
using CashFlow.Transactions.Data;
using CashFlow.Transactions.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TransactionContext>(options 
    => options.UseNpgsql(builder.Configuration.GetConnectionString("TransactionContext")));

// Register application services
builder.Services.AddHostedService<CustomerCreatedConsumer>();

builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<ICreateAccount, CreateAccount>();
builder.Services.AddScoped<ICreateTransaction, CreateTransaction>();
builder.Services.AddScoped<IPayBill, PayBill>();

builder.Services.AddRabbitMQ(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();

app.MapTransactionEndpoints();
app.MapAccountEndpoints();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.Run();

public partial class Program { }