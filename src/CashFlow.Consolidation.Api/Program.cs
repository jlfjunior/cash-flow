using CashFlow.Consolidation.Api.Domain;
using CashFlow.Consolidation.Api.Domain.Services;
using CashFlow.Consolidation.Api.Infrastructure;
using CashFlow.Lib.EventBus;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddRabbitMQ(builder.Configuration);
builder.Services.Configure<MongoDbConfiguration>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddScoped<IDailyClosureService, DailyClosureService>();

builder.Services.AddHostedService<TransactionConsumer>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/daily-closures", () =>
    {
        return Results.Ok();
    })
    .WithName("GetDailyClosures");

app.Run();

public partial class Program { }
