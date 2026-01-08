using System.Text.Json;
using CashFlow.Customers.Domain.Entities;
using CashFlow.Customers.Domain.Events;
using CashFlow.Customers.Domain.Repositories;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Customers.Application;

public class PublishCustomerCreated(
    ILogger<PublishCustomerCreated> logger, 
    IRepository repository, 
    IEventBus eventBus) 
    : IPublishCustomerCreated
{
    public async Task ExecuteAsync(CancellationToken token)
    {
        var customerCreatedEvent = await repository.GetPendingOutboxMessageByTypeAsync(
            typeof(CustomerCreated).Name, 
            token);

        if (customerCreatedEvent is null)
        {
            logger.LogInformation("There has not been a pending creation of customer.");
            return;
        }

        try
        {
            var customerCreated = JsonSerializer.Deserialize<CustomerCreated>(customerCreatedEvent.Content);
            await eventBus.PublishAsync(customerCreated, "queuing.customers.created");
            
            customerCreatedEvent.Status = OutboxStatus.Processed;
            customerCreatedEvent.ProcessedAt = DateTime.UtcNow;
            
            await repository.UpdateOutboxMessageAsync(customerCreatedEvent, token);
        }
        catch (Exception e)
        {
            customerCreatedEvent.Status = OutboxStatus.Failed;
            customerCreatedEvent.StatusReason = e.Message;
            
            await repository.UpdateOutboxMessageAsync(customerCreatedEvent, token);
        }
    }
}
