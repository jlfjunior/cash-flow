using CashFlow.Lib.EventBus;
using CashFlow.Transactions.Domain.Repositories;
using CashFlow.Transactions.Domain.Exceptions;
using CashFlow.Transactions.Domain.Entities;
using CashFlow.Transactions.Application.Requests;
using CashFlow.Transactions.Application.Responses;
using Microsoft.Extensions.Logging;

namespace CashFlow.Transactions.Application;

public class Transfer(ILogger<Transfer> logger, IRepository accountRepository, IEventBus eventBus)
    : ITransfer
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
        }
        
        var destinationLastTransaction = destinationAccount.Transactions?.LastOrDefault();
        if (destinationLastTransaction != null)
        {
            await eventBus.PublishAsync(destinationLastTransaction, "accounts.update");
        }
    
        logger.LogInformation("Transfer processed. SourceAccountId {SourceAccountId}, DestinationAccountId {DestinationAccountId}, Value {Value}.", 
            sourceAccount.Id, destinationAccount.Id, request.Value);

        return new AccountResponse(sourceAccount.Id);
    }
}

