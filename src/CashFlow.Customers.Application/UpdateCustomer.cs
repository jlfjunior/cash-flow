using CashFlow.Customers.Application.Requests;
using CashFlow.Customers.Application.Responses;
using CashFlow.Customers.Domain.Events;
using CashFlow.Customers.Domain.Repositories;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Customers.Application;

public class UpdateCustomer(ILogger<UpdateCustomer> logger, IRepository repository, IEventBus eventBus) : IUpdateCustomer
{
    public async Task<UpdateCustomerResponse> ExecuteAsync(UpdateCustomerRequest request, CancellationToken token)
    {
        var customer = await repository.GetByIdAsync(request.Id);
        
        if (customer is null)
            throw new Exception("Customer not found");
        
        customer.WithFullName(request.FullName);
        
        var customerEvent = new CustomerUpdated(customer.Id, request.FullName);

        await repository.UpsertAsync(customer, token);
        
        await eventBus.PublishAsync(customerEvent, "queuing.customers.updated");
        
        logger.LogInformation("Updated customer. Id: {Id}", customer.Id);
        
        return new UpdateCustomerResponse(customer.Id,  customer.FullName);
    }
}