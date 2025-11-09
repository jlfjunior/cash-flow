using CashFlow.Transaction.Api.Application.Responses;
using CashFlow.Transaction.Api.Domain.Entities;

namespace CashFlow.Transaction.Api.Domain.Repositories;

public interface IRepository
{
    Task<Account> GetByIdAsync(Guid id);
    Task UpsertAsync(Account account, CancellationToken token);
    Task<List<AccountResponse>> SearchAsync(Guid? accountId = null);
}