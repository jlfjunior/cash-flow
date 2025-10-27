using CashFlow.Customer.Api.Domain.Services;

namespace CashFlow.Customer.Api.Domain.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<CreateCustomerResponse>> SearchAsync();
}