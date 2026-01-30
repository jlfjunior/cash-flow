using System.Text.Json;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Events;
using CashFlow.Domain.Repositories;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Features.UpdateCustomer;

public class PublishCustomerUpdated(
    ILogger<PublishCustomerUpdated> logger, 
    ICustomerRepository repository, 
    IEventBus eventBus) 
    : IPublishCustomerUpdated
{
    public async Task ExecuteAsync(CancellationToken token)
    {
        var customerUpdatedEvent = await repository.GetPendingOutboxMessageByTypeAsync(
            typeof(CustomerUpdated).Name, 
            token);

        if (customerUpdatedEvent is null)
        {
            logger.LogInformation("There has not been a pending update of customer.");
            return;
        }

        try
        {
            var customerUpdated = JsonSerializer.Deserialize<CustomerUpdated>(customerUpdatedEvent.Content);
            await eventBus.PublishAsync(customerUpdated, "queuing.customers.updated");
            
            customerUpdatedEvent.Status = OutboxStatus.Processed;
            customerUpdatedEvent.ProcessedAt = DateTime.UtcNow;
            
            await repository.UpdateOutboxMessageAsync(customerUpdatedEvent, token);
        }
        catch (Exception e)
        {
            customerUpdatedEvent.Status = OutboxStatus.Failed;
            customerUpdatedEvent.StatusReason = e.Message;
            
            await repository.UpdateOutboxMessageAsync(customerUpdatedEvent, token);
        }
    }
}
