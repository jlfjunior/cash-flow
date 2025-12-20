using CashFlow.Lib.EventBus;
using CashFlow.Lib.Sharable;
using CashFlow.Transactions.Application.Requests;
using CashFlow.Transactions.Domain.Entities;
using CashFlow.Transactions.Domain.Events;
using CashFlow.Transactions.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CashFlow.Transactions.Application;

public class CreateAccount(ILogger<CreateAccount> logger, 
    IRepository accountRepository, 
    IEventBus eventBus) : ICommand<CreateAccountRequest>
{
    public async Task ExecuteAsync(CreateAccountRequest request,  CancellationToken token)
    {
        var account = new Account(request.CustomerId);
        
        await accountRepository.UpsertAsync(account, token);
        
        var @event = new AccountCreated(account.Id, account.CustomerId);
        
        account.AddEvent(@event);
        
        await eventBus.PublishAsync(@event, "account.created");
        
        logger.LogInformation($"Account created for customer: {request.CustomerId}");
    }
}