using CashFlow.Domain;
using CashFlow.Domain.Events;
using CashFlow.Domain.Repositories;
using CashFlow.Features.Transactions.Requests;
using CashFlow.Features.Transactions.Responses;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;

namespace CashFlow.Features.Transactions
{
    public class CreateTransaction(ILogger<CreateTransaction> logger, 
        IAccountRepository accountRepository, 
        IEventBus eventBus)
        : ICommand<CreateTransactionRequest, AccountResponse>
    {
        public async Task<AccountResponse> ExecuteAsync(CreateTransactionRequest request, CancellationToken token)
        {
            var account = await accountRepository.GetByIdAsync(request.AccountId);

            account.AddTransaction(request.Direction, request.Value);
        
            await accountRepository.UpsertAsync(account, token);
        
            // Publish transaction entity for accounts.update
            await eventBus.PublishAsync(account.Transactions.First(), "accounts.update");
            
            // Publish TransactionCreated event for consolidation
            var transactionCreatedEvent = account.Events.OfType<CashFlow.Domain.Events.TransactionCreated>().FirstOrDefault();
            if (transactionCreatedEvent != null)
            {
                // Create event with CustomerId for consolidation
                var consolidationEvent = new TransactionCreated(
                    transactionCreatedEvent.Id,
                    account.CustomerId,
                    transactionCreatedEvent.Direction,
                    transactionCreatedEvent.TransactionType,
                    transactionCreatedEvent.ReferenceDate,
                    transactionCreatedEvent.Value);
                
                await eventBus.PublishAsync(consolidationEvent, "transaction.created");
            }
        
            logger.LogInformation("Transaction created. Id {AccountId}.", account.Id);

            return new AccountResponse(account.Id);
        }
    }
}
