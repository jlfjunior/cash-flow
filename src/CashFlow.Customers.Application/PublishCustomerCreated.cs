using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using CashFlow.Customers.Data;
using CashFlow.Customers.Domain.Entities;
using CashFlow.Customers.Domain.Events;
using CashFlow.Lib.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CashFlow.Customers.Application;

public class PublishCustomerCreated(ILogger<PublishCustomerCreated> logger, CustomerContext context, IEventBus  eventBus) 
    : IPublishCustomerCreated
{
    public async Task ExecuteAsync(CancellationToken token)
    {
        var customerCreatedEvent = await context.OutboxMessages
            .Where(o => o.Status == OutboxStatus.Pending)
            .Where(o => o.Type == typeof(CustomerCreated).Name)
            .FirstOrDefaultAsync(token);

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
        }
        catch (Exception e)
        {
            customerCreatedEvent.Status = OutboxStatus.Failed;
        }
        
        await context.SaveChangesAsync(token);
    }
}