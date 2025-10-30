using CashFlow.Customer.Api.Application.Responses;

namespace CashFlow.Customer.Api.Domain.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<CreateCustomerResponse>> SearchAsync();
}