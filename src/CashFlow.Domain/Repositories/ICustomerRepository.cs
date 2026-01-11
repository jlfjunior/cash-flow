using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer> GetByIdAsync(Guid id);
    Task<IEnumerable<Customer>> SearchAsync();
    Task UpsertAsync(Customer customer, CancellationToken token);
    Task UpsertAsync(IEnumerable<OutboxMessage> events, CancellationToken token);
    Task<OutboxMessage?> GetPendingOutboxMessageByTypeAsync(string type, CancellationToken token);
    Task UpdateOutboxMessageAsync(OutboxMessage message, CancellationToken token);
    Task CommitAsync(CancellationToken token);
}
