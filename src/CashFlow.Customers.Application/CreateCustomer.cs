using System.Text.Json;
using CashFlow.Customers.Domain.Events;
using CashFlow.Customers.Application.Requests;
using CashFlow.Customers.Application.Responses;
using CashFlow.Customers.Domain.Entities;
using CashFlow.Customers.Domain.Repositories;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Customers.Application;

public class CreateCustomer(ILogger<CreateCustomer> logger, IRepository repository)
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