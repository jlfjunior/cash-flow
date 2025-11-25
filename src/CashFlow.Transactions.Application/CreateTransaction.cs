using CashFlow.Lib.EventBus;
using CashFlow.Transactions.Domain.Repositories;
using CashFlow.Transactions.Application.Requests;
using CashFlow.Transactions.Application.Responses;
using Microsoft.Extensions.Logging;

namespace CashFlow.Transactions.Application
{
    public class CreateTransaction(ILogger<CreateTransaction> logger, IRepository accountRepository, IEventBus eventBus)
        : ICreateTransaction
    {
        public async Task<AccountResponse> ExecuteAsync(CreateTransactionRequest request, CancellationToken token)
        {
            var account = await accountRepository.GetByIdAsync(request.AccountId);

            account.AddTransaction(request.Direction, request.Value);
        
            await accountRepository.UpsertAsync(account, token);
        
            await eventBus.PublishAsync(account.Transactions.First(), "accounts.update");
        
            logger.LogInformation("Transaction created.  Id {AccountId}.", account.Id);

            return new AccountResponse(account.Id);
        }
    }
}