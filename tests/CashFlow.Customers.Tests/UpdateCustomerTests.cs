using CashFlow.Customers.Application;
using CashFlow.Customers.Application.Requests;
using CashFlow.Customers.Domain.Entities;
using CashFlow.Customers.Domain.Repositories;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CashFlow.Customers.Tests;

public class UpdateCustomerTests
{
    [Fact]
    public async Task UpdateCustomer_SuccessAsync()
    {
        var customer = new Customer("Doe");
        
        var request = new UpdateCustomerRequest(customer.Id)
        {
            FullName = "John Doe",
        };
        
        var logger = Substitute.For<ILogger<UpdateCustomer>>();
        var repository = Substitute.For<IRepository>();
        var eventBus = Substitute.For<IEventBus>();
        
        repository.GetByIdAsync(customer.Id).Returns(customer);
        
        var updateCustomer = new UpdateCustomer(logger, repository, eventBus);

        var response = await updateCustomer.ExecuteAsync(request, CancellationToken.None);
        
        Assert.Equal("John Doe", response.FullName);

        repository.Received(2);
    }

    [Fact]
    public async Task UpdateCustomer_FailAsync()
    {
        
        var request = new UpdateCustomerRequest(Guid.NewGuid())
        {
            FullName = "John Doe",
        };
        
        var logger = Substitute.For<ILogger<UpdateCustomer>>();
        var repository = Substitute.For<IRepository>();
        var eventBus = Substitute.For<IEventBus>();
        
        var updateCustomer = new UpdateCustomer(logger, repository, eventBus);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            updateCustomer.ExecuteAsync(request, CancellationToken.None));

        Assert.Contains("Customer not found", ex.Message);
    }
}