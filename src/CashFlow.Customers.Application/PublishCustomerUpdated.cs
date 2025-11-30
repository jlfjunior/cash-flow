using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using CashFlow.Customers.Data;
using CashFlow.Customers.Domain.Entities;
using CashFlow.Customers.Domain.Events;
using CashFlow.Lib.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CashFlow.Customers.Application;

public class PublishCustomerUpdated(ILogger<PublishCustomerUpdated> logger, CustomerContext context, IEventBus  eventBus) 
    : IPublishCustomerUpdated
{
    public async Task ExecuteAsync(CancellationToken token)
    {
        var customerUpdatedEvent = await context.OutboxMessages
            .Where(o => o.Status == OutboxStatus.Pending)
            .Where(o => o.Type == typeof(CustomerUpdated).Name)
            .FirstOrDefaultAsync(token);

        if (customerUpdatedEvent is null)
        {
            logger.LogInformation("There has not been a pending creation of customer.");
            return;
        }

        try
        {
            var customerUpdated = JsonSerializer.Deserialize<CustomerCreated>(customerUpdatedEvent.Content);
            await eventBus.PublishAsync(customerUpdated, "queuing.customers.updated");
            customerUpdatedEvent.Status = OutboxStatus.Processed;
        }
        catch (Exception e)
        {
            customerUpdatedEvent.Status = OutboxStatus.Failed;
        }
        
        await context.SaveChangesAsync(token);
    }
}