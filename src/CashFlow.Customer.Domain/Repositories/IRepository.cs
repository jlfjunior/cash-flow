namespace CashFlow.Customer.Domain.Repositories;

public interface IRepository
{
    Task<Entities.Customer> GetByIdAsync(Guid id);
    Task<IEnumerable<Entities.Customer>> SearchAsync();
    Task UpsertAsync(Entities.Customer customer, CancellationToken token);
}