using CashFlow.Customers.Domain.Events;
using CashFlow.Customers.Application.Requests;
using CashFlow.Customers.Application.Responses;
using CashFlow.Customers.Domain.Repositories;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Customers.Application;

public interface ICreateCustomer
{
    Task<CreateCustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken ct);
}

public class CreateCustomer : ICreateCustomer
{
    private readonly ILogger<CreateCustomer> _logger;
    private readonly IRepository _repository;
    private readonly IEventBus _eventBus;

    public CreateCustomer(ILogger<CreateCustomer> logger, IRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }
    
    public async Task<CreateCustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken token)
    {
        var customer = new Customers.Domain.Entities.Customer(request.FullName);
        
        await _repository.UpsertAsync(customer, token);

        var customerEvent = new CustomerCreated(customer.Id, request.FullName);
        
        await _eventBus.PublishAsync(customerEvent, "queuing.customers.created");
        
        _logger.LogInformation("Created customer. Id: {Id}", customer.Id);
        
        return new CreateCustomerResponse(customer.Id, customer.FullName);
    }
}