using CashFlow.Lib.EventBus;
using CashFlow.Transactions.Domain.Entities;
using CashFlow.Transactions.Domain.Events;
using CashFlow.Transactions.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CashFlow.Transactions.Application;

public class CreateAccount(ILogger<CreateAccount> logger, IRepository accountRepository, IEventBus eventBus)
    : ICreateAccount
{
    public async Task ExecuteAsync(Guid customerId, CancellationToken token)
    {
        var account = new Account(customerId);
        
        await accountRepository.UpsertAsync(account, token);
        
        var @event = new AccountCreated(account.Id, account.CustomerId);
        
        account.AddEvent(@event);
        
        await eventBus.PublishAsync(@event, "account.created");
        
        logger.LogInformation($"Account created for customer: {customerId}");
    }
}