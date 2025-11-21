using CashFlow.Customer.Domain.Events;
using CashFlow.Customer.Application.Requests;
using CashFlow.Customer.Application.Responses;
using CashFlow.Customer.Domain.Repositories;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Customer.Application;

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
        var customer = new Domain.Entities.Customer(request.FullName);
        
        await _repository.UpsertAsync(customer, token);

        var customerEvent = new CustomerCreated(customer.Id, request.FullName);
        
        await _eventBus.PublishAsync(customerEvent, "queuing.customers.created");
        
        _logger.LogInformation("Created customer. Id: {Id}", customer.Id);
        
        return new CreateCustomerResponse(customer.Id, customer.FullName);
    }
}