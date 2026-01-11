using CashFlow.Domain;
using CashFlow.Domain.Exceptions;
using CashFlow.Domain.Repositories;
using CashFlow.Features.Consolidation.Responses;
using CashFlow.Features.Transactions.Requests;
using CashFlow.Features.Transactions.Responses;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Features.Transactions;

public class PayBill(
    ILogger<PayBill> logger, 
    IAccountRepository accountRepository, 
    IEventBus eventBus)
    : ICommand<PayBillRequest, AccountResponse>
{
    public async Task<AccountResponse> ExecuteAsync(PayBillRequest request, CancellationToken token)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId);
        
        if (account == null)
            throw new NotFoundException($"Account with id {request.AccountId} was not found.");

        account.PayBill(request.Value);
    
        await accountRepository.UpsertAsync(account, token);
    
        var lastTransaction = account.Transactions.LastOrDefault();
        if (lastTransaction == null)
        {
            logger.LogWarning("No transaction found after PayBill operation. AccountId: {AccountId}", account.Id);
            return new AccountResponse(account.Id);
        }
        
        await eventBus.PublishAsync(lastTransaction, "accounts.update");
        
        // Publish TransactionCreated event for consolidation
        var transactionCreatedEvent = account.Events.OfType<CashFlow.Domain.Events.TransactionCreated>().LastOrDefault();
        if (transactionCreatedEvent != null)
        {
            var consolidationEvent = new CashFlow.Features.Consolidation.Responses.TransactionCreated(
                transactionCreatedEvent.Id,
                account.CustomerId,
                transactionCreatedEvent.ReferenceDate,
                transactionCreatedEvent.Direction,
                transactionCreatedEvent.TransactionType,
                transactionCreatedEvent.Value);
            
            await eventBus.PublishAsync(consolidationEvent, "transaction.created");
        }
    
        logger.LogInformation("Bill payment processed. AccountId {AccountId}.", account.Id);

        return new AccountResponse(account.Id);
    }
}
