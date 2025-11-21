using CashFlow.Customer.Application.Requests;
using CashFlow.Customer.Application.Responses;
using CashFlow.Customer.Domain.Events;
using CashFlow.Customer.Domain.Repositories;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Customer.Application;

public interface IUpdateCustomer
{
    Task<UpdateCustomerResponse> ExecuteAsync(UpdateCustomerRequest request,  CancellationToken token);
}

public class UpdateCustomer : IUpdateCustomer
{
    private readonly ILogger<UpdateCustomer> _logger;
    private readonly IRepository _repository;
    private readonly IEventBus _eventBus;

    public UpdateCustomer(ILogger<UpdateCustomer> logger, IRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }
    
    public async Task<UpdateCustomerResponse> ExecuteAsync(UpdateCustomerRequest request, CancellationToken token)
    {
        var customer = await _repository.GetByIdAsync(request.Id);
        
        if (customer is null)
            throw new Exception("Customer not found");
        
        customer.WithFullName(request.FullName);
        
        var customerEvent = new CustomerUpdated(customer.Id, request.FullName);

        await _repository.UpsertAsync(customer, token);
        
        await _eventBus.PublishAsync(customerEvent, "queuing.customers.updated");
        
        _logger.LogInformation("Updated customer. Id: {Id}", customer.Id);
        
        return new UpdateCustomerResponse(customer.Id,  customer.FullName);
    }
}