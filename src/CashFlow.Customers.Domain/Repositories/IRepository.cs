using CashFlow.Customers.Domain.Entities;

namespace CashFlow.Customers.Domain.Repositories;

public interface IRepository
{
    Task<Customer> GetByIdAsync(Guid id);
    Task<IEnumerable<Customer>> SearchAsync();
    Task UpsertAsync(Customer customer, CancellationToken token);
}