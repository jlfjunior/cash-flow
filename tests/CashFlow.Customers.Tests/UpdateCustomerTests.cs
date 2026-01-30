using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Features.UpdateCustomer;
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
        
        var request = new UpdateCustomerRequest(Id: customer.Id, FullName: "John Doe");
        
        var logger = Substitute.For<ILogger<UpdateCustomer>>();
        var repository = Substitute.For<ICustomerRepository>();
        var eventBus = Substitute.For<IEventBus>();
        
        repository.GetByIdAsync(customer.Id).Returns(customer);
        
        var updateCustomer = new UpdateCustomer(logger, repository, eventBus);

        var response = await updateCustomer.ExecuteAsync(request, CancellationToken.None);
        
        Assert.Equal("John Doe", response.FullName);

        await repository.Received(1).GetByIdAsync(customer.Id);
        await repository.Received(1).UpsertAsync(customer, Arg.Any<CancellationToken>());
        await repository.Received(1).UpsertAsync(Arg.Any<IEnumerable<OutboxMessage>>(), Arg.Any<CancellationToken>());
        await repository.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCustomer_FailAsync()
    {
        var request = new UpdateCustomerRequest(Id: Guid.NewGuid(), FullName: "John Doe");
        
        var logger = Substitute.For<ILogger<UpdateCustomer>>();
        var repository = Substitute.For<ICustomerRepository>();
        var eventBus = Substitute.For<IEventBus>();
        
        repository.GetByIdAsync(Arg.Any<Guid>()).Returns((Customer?)null);
        
        var updateCustomer = new UpdateCustomer(logger, repository, eventBus);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            updateCustomer.ExecuteAsync(request, CancellationToken.None));

        Assert.Contains("Customer not found", ex.Message);
    }
}
