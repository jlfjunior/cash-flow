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
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventBus _eventBus;

    public CreateCustomer(ILogger<CreateCustomer> logger, ICustomerRepository customerRepository, IEventBus eventBus)
    {
        _logger = logger;
        _customerRepository = customerRepository;
        _eventBus = eventBus;
    }
    
    public async Task<CreateCustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken token)
    {
        var customer = new Domain.Entities.Customer(request.FullName);
        
        await _customerRepository.UpsertAsync(customer, token);

        var customerEvent = new CustomerCreated(customer.Id, request.FullName);
        
        await _eventBus.PublishAsync(customerEvent, "queuing.customers.created");
        
        _logger.LogInformation("Created customer. Id: {Id}", customer.Id);
        
        return new CreateCustomerResponse(customer.Id, customer.FullName);
    }
}