using CashFlow.Customer.Api.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.MapGet("/customers", () =>
{
    var customers = Enumerable.Empty<Customer>();

    return Results.Ok(customers);
})
.WithName("AddCustomer");

app.MapGet("/customers", (string fullName) =>
{
    var customer = new Customer(fullName);
    
    return Results.Ok(customer);
})
.WithName("GetCustomer");

app.Run();

public partial class Program { }
