using CashFlow.Customers.Domain.Entities;
using CashFlow.Lib.Sharable;

namespace CashFlow.Customers.Domain.Repositories;

public interface IRepository
{
    Task<Customer> GetByIdAsync(Guid id);
    Task<IEnumerable<Customer>> SearchAsync();
    Task UpsertAsync(Customer customer, CancellationToken token);
    Task UpsertAsync(IEnumerable<OutboxMessage> events, CancellationToken token);
    Task CommitAsync(CancellationToken token);
}