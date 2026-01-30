using System.Text.Json;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Events;
using CashFlow.Domain.Repositories;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Features.UpdateCustomer;

public class UpdateCustomer(ILogger<UpdateCustomer> logger, ICustomerRepository repository, IEventBus eventBus) : IUpdateCustomer
{
    public async Task<UpdateCustomerResponse> ExecuteAsync(UpdateCustomerRequest request, CancellationToken token)
    {
        var customer = await repository.GetByIdAsync(request.Id);
        
        if (customer is null)
            throw new Exception("Customer not found");
        
        customer.WithFullName(request.FullName);
        
        await repository.UpsertAsync(customer, token);
        
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
        
        return new UpdateCustomerResponse(customer.Id, customer.FullName);
    }
}
