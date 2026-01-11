using System.Text.Json;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Events;
using CashFlow.Domain.Repositories;
using CashFlow.Features.Customers.Requests;
using CashFlow.Features.Customers.Responses;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Features.Customers;

public class CreateCustomer(ILogger<CreateCustomer> logger, ICustomerRepository repository)
    : ICreateCustomer
{
    public async Task<CreateCustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken token)
    {
        var customer = new Customer(request.FullName);
        
        await repository.UpsertAsync(customer, token);

        var customerEvent = new CustomerCreated(customer.Id, request.FullName);

        var messages = new List<OutboxMessage>()
        {
            new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = customerEvent.GetType().Name,
                Content = JsonSerializer.Serialize(customerEvent),
                CreatedAt = DateTime.UtcNow,
                Status = OutboxStatus.Pending
            }
        };
        
        await repository.UpsertAsync(messages, token);
        
        logger.LogInformation("Created customer. Id: {Id}", customer.Id);

        await repository.CommitAsync(token);
        
        return new CreateCustomerResponse(customer.Id, customer.FullName);
    }
}
