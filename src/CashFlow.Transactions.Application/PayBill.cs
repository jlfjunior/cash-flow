using CashFlow.Lib.EventBus;
using CashFlow.Lib.Sharable;
using CashFlow.Transactions.Domain.Repositories;
using CashFlow.Transactions.Domain.Exceptions;
using CashFlow.Transactions.Application.Requests;
using CashFlow.Transactions.Application.Responses;
using Microsoft.Extensions.Logging;

namespace CashFlow.Transactions.Application;

public class PayBill(ILogger<PayBill> logger, 
    IRepository accountRepository, 
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
    
        logger.LogInformation("Bill payment processed. AccountId {AccountId}.", account.Id);

        return new AccountResponse(account.Id);
    }
}

