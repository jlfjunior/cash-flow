using CashFlow.Customer.Api.Application.Requests;
using CashFlow.Customer.Api.Application.Responses;
using CashFlow.Customer.Api.Infrastructure;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CashFlow.Customer.Api.Application;

public interface IUpdateCustomer
{
    Task<UpdateCustomerResponse> ExecuteAsync(UpdateCustomerRequest request,  CancellationToken ct);
}

public class UpdateCustomer : IUpdateCustomer
{
    private readonly ILogger<UpdateCustomer> _logger;
    private readonly IMongoCollection<Domain.Customer> _customers;
    private readonly IEventBus _eventBus;

    public UpdateCustomer(ILogger<UpdateCustomer> logger, IOptions<MongoDbConfiguration> mongoOptions, IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
        
        var config = mongoOptions.Value;
        var connectionString =
            $"mongodb://{config.Username}:{config.Password}@{config.Host}:{config.Port}/{config.Database}?authSource={config.Username}";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(config.Database);
        _customers = database.GetCollection<Domain.Customer>("customers");
    }
    
    public async Task<UpdateCustomerResponse> ExecuteAsync(UpdateCustomerRequest request, CancellationToken ct)
    {
        var customer = await _customers.Find(x => x.Id == request.Id)
            .SingleAsync(ct);

        customer.WithFullName(request.FullName);

        _customers.UpdateOne(x => x.Id == request.Id, 
            Builders<Domain.Customer>.Update.Set(x => x.FullName, customer.FullName));
        
        await _eventBus.PublishAsync(customer, "queuing.customers.updated");
        
        _logger.LogInformation("Updated customer. Id: {Id}", customer.Id);
        
        return new UpdateCustomerResponse(customer.Id,  customer.FullName);
    }
}