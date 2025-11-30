using System.Text.Json;
using CashFlow.Customers.Application.Requests;
using CashFlow.Customers.Application.Responses;
using CashFlow.Customers.Domain.Entities;
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

        var messages = new List<OutboxMessage>()
        {
            new OutboxMessage()
            {
                Id = Guid.NewGuid(),
                Type = customerEvent.GetType().Name,
                Content = JsonSerializer.Serialize(customerEvent),
                CreatedAt = DateTime.UtcNow,
                Status = OutboxStatus.Pending
            }
        }; 
        
        await repository.UpsertAsync(messages, token);
        
        logger.LogInformation("Updated customer. Id: {Id}", customer.Id);
        
        await repository.CommitAsync(token);
        
        return new UpdateCustomerResponse(customer.Id,  customer.FullName);
    }
}