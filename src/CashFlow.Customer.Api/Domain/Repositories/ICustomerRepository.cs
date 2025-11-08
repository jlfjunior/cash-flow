using CashFlow.Customer.Api.Application.Responses;

namespace CashFlow.Customer.Api.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Entities.Customer> GetByIdAsync(Guid id);
    Task<IEnumerable<CreateCustomerResponse>> SearchAsync();
    Task UpsertAsync(Entities.Customer customer, CancellationToken token);
}