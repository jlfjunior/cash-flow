using CashFlow.Api;
using CashFlow.Api.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapDefaultEndpoints();
app.MapCustomerEndpoints();
app.MapAccountEndpoints();
app.MapTransactionEndpoints();
app.MapConsolidationEndpoints();

app.UseMiddleware<ExceptionHandlingMiddleware>();

await app.RunAsync();