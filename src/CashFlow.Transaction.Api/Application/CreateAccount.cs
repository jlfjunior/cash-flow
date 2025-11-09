using CashFlow.Lib.EventBus;
using CashFlow.Transaction.Api.Domain.Entities;
using CashFlow.Transaction.Api.Domain.Events;
using CashFlow.Transaction.Api.Domain.Repositories;

namespace CashFlow.Transaction.Api.Application;

public interface ICreateAccount
{
    Task ExecuteAsync(Guid customerId, CancellationToken token);
}

public class CreateAccount : ICreateAccount
{
    private readonly ILogger<CreateAccount> _logger;
    private readonly IRepository _accountRepository;
    private readonly IEventBus _eventBus;

    public CreateAccount(ILogger<CreateAccount> logger, IRepository accountRepository, IEventBus eventBus)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _eventBus = eventBus;
    }
    
    public async Task ExecuteAsync(Guid customerId, CancellationToken token)
    {
        var account = new Account(customerId);
        
        await _accountRepository.UpsertAsync(account, token);
        
        var @event = new AccountCreated(account.Id, account.CustomerId);
        
        account.AddEvent(@event);
        
        await _eventBus.PublishAsync(@event, "account.created");
        
        _logger.LogInformation($"Account created for customer: {customerId}");
    }
}