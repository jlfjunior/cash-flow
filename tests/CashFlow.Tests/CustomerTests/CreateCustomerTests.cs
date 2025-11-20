using CashFlow.Customer.Application;
using CashFlow.Customer.Application.Requests;
using CashFlow.Customer.Domain.Repositories;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CashFlow.Tests.CustomerTests;

public class CreateCustomerTests
{
    [Fact]
    public async Task CreateNewCustomer()
    {
        var request = new CreateCustomerRequest
        {
            FullName = "John Doe",
        };
        
        var logger = Substitute.For<ILogger<CreateCustomer>>();
        var repository = Substitute.For<ICustomerRepository>();
        var eventBus = Substitute.For<IEventBus>();
        
        var createCustomer = new CreateCustomer(logger, repository, eventBus);

        var customer = await createCustomer.ExecuteAsync(request, CancellationToken.None);
        
        Assert.Equal("John Doe", customer.FullName);
        
        repository.Received(1);
    }
}