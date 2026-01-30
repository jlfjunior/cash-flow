using CashFlow.Api;
using CashFlow.Api.Endpoints;
using CashFlow.Data;
using CashFlow.Data.Repositories;
using CashFlow.Domain;
using CashFlow.Domain.Repositories;
using CashFlow.Features.CreateAccount;
using CashFlow.Features.CreateCustomer;
using CashFlow.Features.Deposit;
using CashFlow.Features.Transactions;
using CashFlow.Features.Transactions.Requests;
using CashFlow.Features.Transfer;
using CashFlow.Features.UpdateCustomer;
using CashFlow.Features.Withdraw;
using CashFlow.Lib.EventBus;
using MongoDB.Driver;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

// Add MongoDB using Aspire integration
builder.AddMongoDBClient("mongodb");

// Configure MongoDB context
var mongoDatabaseName = builder.Configuration.GetValue<string>("MongoDB:DatabaseName") ?? "cashflow";

builder.Services.AddScoped<CashFlowMongoContext>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return new CashFlowMongoContext(client, mongoDatabaseName);
});

builder.Services.AddRabbitMQ(builder.Configuration);

// Register repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Register Features (Application services)
builder.Services.AddScoped<ICreateCustomer, CreateCustomer>();
builder.Services.AddScoped<IUpdateCustomer, UpdateCustomer>();
builder.Services.AddScoped<ICommand<CreateAccountRequest>, CreateAccount>();
builder.Services.AddScoped<ICommand<DepositRequest, AccountResponse>, Deposit>();
builder.Services.AddScoped<ICommand<WithdrawRequest, AccountResponse>, Withdraw>();
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
app.MapCustomerEndpoints();
app.MapAccountEndpoints();
app.MapTransactionEndpoints();

app.UseMiddleware<ExceptionHandlingMiddleware>();

await app.RunAsync();
