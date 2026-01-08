using CashFlow.Customers.Application;
using CashFlow.Customers.Application.Requests;
using CashFlow.Customers.Domain.Repositories;
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
        var repository = Substitute.For<IRepository>();

        var createCustomer = new CreateCustomer(logger, repository);

        var customer = await createCustomer.ExecuteAsync(request, CancellationToken.None);

        Assert.Equal("John Doe", customer.FullName);

        await repository.Received(1).UpsertAsync(Arg.Is<CashFlow.Customers.Domain.Entities.Customer>(c => c.FullName == "John Doe"), Arg.Any<CancellationToken>());
        await repository.Received(1).UpsertAsync(Arg.Any<IEnumerable<CashFlow.Customers.Domain.Entities.OutboxMessage>>(), Arg.Any<CancellationToken>());
        await repository.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
