using CashFlow.Transaction.Api.Application.Responses;

namespace CashFlow.Transaction.Api.Domain.Repositories;

public interface IRepository
{
    Task<List<AccountResponse>> SearchAsync(Guid? accountId = null);
}