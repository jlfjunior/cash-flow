using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Features.Customers;
using CashFlow.Features.Customers.Requests;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CashFlow.Customers.Tests;

public class CreateCustomerTests
{
    [Fact]
    public async Task CreateNewCustomer()
    {
        var request = new CreateCustomerRequest
        {
            FullName = "John Doe"
        };

        var logger = Substitute.For<ILogger<CreateCustomer>>();
        var repository = Substitute.For<ICustomerRepository>();

        var createCustomer = new CreateCustomer(logger, repository);

        var customer = await createCustomer.ExecuteAsync(request, CancellationToken.None);

        Assert.Equal("John Doe", customer.FullName);

        await repository.Received(1).UpsertAsync(Arg.Is<Customer>(c => c.FullName == "John Doe"), Arg.Any<CancellationToken>());
        await repository.Received(1).UpsertAsync(Arg.Any<IEnumerable<OutboxMessage>>(), Arg.Any<CancellationToken>());
        await repository.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
