using CashFlow.Domain;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Events;
using CashFlow.Domain.Exceptions;
using CashFlow.Domain.Repositories;
using CashFlow.Features.CreateAccount;
using CashFlow.Features.Transactions.Requests;
using CashFlow.Features.Transfer;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Features.Transactions;

public class Transfer(
    ILogger<Transfer> logger, 
    IAccountRepository accountRepository, 
    IEventBus eventBus)
    : ICommand<TransferRequest, AccountResponse>
{
    public async Task<AccountResponse> ExecuteAsync(TransferRequest request, CancellationToken token)
    {
        var sourceAccount = await accountRepository.GetByIdAsync(request.SourceAccountId);
        
        if (sourceAccount == null)
            throw new NotFoundException($"Source account with id {request.SourceAccountId} was not found.");

        var destinationAccount = await accountRepository.GetByIdAsync(request.DestinationAccountId);
        
        if (destinationAccount == null)
            throw new NotFoundException($"Destination account with id {request.DestinationAccountId} was not found.");

        if (sourceAccount.Id == destinationAccount.Id)
            throw new InvalidOperationException("Cannot transfer to the same account");

        var transferTransaction = new TransferTransaction(sourceAccount.Id, destinationAccount.Id, request.Value);
        sourceAccount.ProcessDebit(transferTransaction);
        
        destinationAccount.ProcessCredit(new DepositTransaction(destinationAccount.Id, request.Value));
    
        await accountRepository.UpsertAsync(sourceAccount, token);
        await accountRepository.UpsertAsync(destinationAccount, token);
    
        var sourceLastTransaction = sourceAccount.Transactions?.LastOrDefault();
        if (sourceLastTransaction != null)
        {
            await eventBus.PublishAsync(sourceLastTransaction, "accounts.update");
            
            // Publish TransactionCreated event for consolidation (source account)
            var sourceTransactionEvent = sourceAccount.Events.OfType<CashFlow.Domain.Events.TransactionCreated>().LastOrDefault();
            if (sourceTransactionEvent != null)
            {
                var consolidationEvent = new TransactionCreated(
                    sourceTransactionEvent.Id,
                    sourceAccount.CustomerId,
                    sourceTransactionEvent.Direction,
                    sourceTransactionEvent.TransactionType,
                    sourceTransactionEvent.ReferenceDate,
                    sourceTransactionEvent.Value);
                
                await eventBus.PublishAsync(consolidationEvent, "transaction.created");
            }
        }
        
        var destinationLastTransaction = destinationAccount.Transactions?.LastOrDefault();
        if (destinationLastTransaction != null)
        {
            await eventBus.PublishAsync(destinationLastTransaction, "accounts.update");
            
            // Publish TransactionCreated event for consolidation (destination account)
            var destinationTransactionEvent = destinationAccount.Events.OfType<CashFlow.Domain.Events.TransactionCreated>().LastOrDefault();
            if (destinationTransactionEvent != null)
            {
                var consolidationEvent = new TransactionCreated(
                    destinationTransactionEvent.Id,
                    destinationAccount.CustomerId,
                    destinationTransactionEvent.Direction,
                    destinationTransactionEvent.TransactionType,
                    destinationTransactionEvent.ReferenceDate,
                    destinationTransactionEvent.Value);
                
                await eventBus.PublishAsync(consolidationEvent, "transaction.created");
            }
        }
    
        logger.LogInformation("Transfer processed. SourceAccountId {SourceAccountId}, DestinationAccountId {DestinationAccountId}, Value {Value}.", 
            sourceAccount.Id, destinationAccount.Id, request.Value);

        return new AccountResponse(sourceAccount.Id);
    }
}
