using CashFlow.Lib.EventBus;
using CashFlow.Transaction.Application.Requests;
using CashFlow.Transaction.Application.Responses;
using CashFlow.Transaction.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CashFlow.Transaction.Application
{
    public interface ICreateTransaction
    {
        Task<AccountResponse> ExecuteAsync(CreateTransactionRequest request, CancellationToken token);
    }

    public class CreateTransaction : ICreateTransaction
    {
        private readonly ILogger<CreateTransaction> _logger;
        private readonly IRepository _accountRepository;
        private readonly IEventBus _eventBus;
    
        public CreateTransaction(ILogger<CreateTransaction> logger, IRepository accountRepository,  IEventBus eventBus)
        {
            _logger = logger;
            _accountRepository = accountRepository;
            _eventBus = eventBus;
        }
    
        public async Task<AccountResponse> ExecuteAsync(CreateTransactionRequest request, CancellationToken token)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId);

            account.AddTransaction(request.Direction, request.Value);
        
            await _accountRepository.UpsertAsync(account, token);
        
            await _eventBus.PublishAsync(account.Transactions.First(), "accounts.update");
        
            _logger.LogInformation("Transaction created.  Id {AccountId}.", account.Id);

            return new AccountResponse(account.Id);
        }
    }
}