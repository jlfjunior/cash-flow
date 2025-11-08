using CashFlow.Customer.Api.Application.Requests;
using CashFlow.Customer.Api.Application.Responses;
using CashFlow.Customer.Api.Domain.Repositories;
using CashFlow.Lib.EventBus;

namespace CashFlow.Customer.Api.Application;

public interface IUpdateCustomer
{
    Task<UpdateCustomerResponse> ExecuteAsync(UpdateCustomerRequest request,  CancellationToken token);
}

public class UpdateCustomer : IUpdateCustomer
{
    private readonly ILogger<UpdateCustomer> _logger;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventBus _eventBus;

    public UpdateCustomer(ILogger<UpdateCustomer> logger, ICustomerRepository customerRepository, IEventBus eventBus)
    {
        _logger = logger;
        _customerRepository = customerRepository;
        _eventBus = eventBus;
    }
    
    public async Task<UpdateCustomerResponse> ExecuteAsync(UpdateCustomerRequest request, CancellationToken token)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);
        
        if (customer is null)
            throw new Exception("Customer not found");
        
        customer.WithFullName(request.FullName);

        await _customerRepository.UpsertAsync(customer, token);
        
        await _eventBus.PublishAsync(customer, "queuing.customers.updated");
        
        _logger.LogInformation("Updated customer. Id: {Id}", customer.Id);
        
        return new UpdateCustomerResponse(customer.Id,  customer.FullName);
    }
}