using CashFlow.Domain;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Events;
using CashFlow.Domain.Repositories;
using CashFlow.Features.Transactions.Requests;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Features.Transactions;

public class CreateAccount(ILogger<CreateAccount> logger, 
    IAccountRepository accountRepository, 
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
