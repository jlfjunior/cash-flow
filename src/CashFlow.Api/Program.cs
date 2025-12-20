using CashFlow.Api;
using CashFlow.Api.Endpoints;
using CashFlow.Customers.Data;
using CashFlow.Lib.EventBus;
using CashFlow.Lib.Sharable;
using CashFlow.Transactions.Application;
using CashFlow.Transactions.Application.Requests;
using CashFlow.Transactions.Application.Responses;
using CashFlow.Transactions.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

builder.Services.AddDbContext<TransactionContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<CustomerContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRabbitMQ(builder.Configuration);

builder.Services.AddScoped<CashFlow.Transactions.Domain.Repositories.IRepository, CashFlow.Transactions.Data.Repository>();
builder.Services.AddScoped<CashFlow.Customers.Domain.Repositories.IRepository, CashFlow.Customers.Data.Repository>();
builder.Services.AddScoped<ICommand<CreateAccountRequest>, CreateAccount>();
builder.Services.AddScoped<ICommand<CreateTransactionRequest, AccountResponse>, CreateTransaction>();
builder.Services.AddScoped<ICommand<PayBillRequest, AccountResponse>, PayBill>();
builder.Services.AddScoped<ICommand<TransferRequest, AccountResponse>, Transfer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.MapDefaultEndpoints();
//app.MapCustomerEndpoints();
app.MapAccountEndpoints();
app.MapTransactionEndpoints();

app.UseMiddleware<ExceptionHandlingMiddleware>();

await app.RunAsync();