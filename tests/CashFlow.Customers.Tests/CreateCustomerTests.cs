using CashFlow.Customers.Application;
using CashFlow.Customers.Application.Requests;
using CashFlow.Customers.Domain.Repositories;
using CashFlow.Lib.EventBus;
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
        var eventBus = Substitute.For<IEventBus>();

        var createCustomer = new CreateCustomer(logger, repository, eventBus);

        var customer = await createCustomer.ExecuteAsync(request, CancellationToken.None);

        Assert.Equal("John Doe", customer.FullName);

        repository.Received(1);
    }
}