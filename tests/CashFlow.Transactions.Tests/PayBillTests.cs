using CashFlow.Domain.Entities;
using CashFlow.Domain.Exceptions;
using CashFlow.Domain.Repositories;
using CashFlow.Features.Transactions;
using CashFlow.Features.Transactions.Requests;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CashFlow.Transactions.Tests;

public class PayBillTests
{
    [Fact]
    public async Task PayBill_WithSufficientBalance_ShouldProcessPayment()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var billAmount = 100.50m;
        var initialBalance = 200.00m;

        var account = new Account(customerId);
        account.AddCredit(initialBalance);
        
        var accountId = account.Id;
        var request = new PayBillRequest(accountId, billAmount);

        var logger = Substitute.For<ILogger<PayBill>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(accountId).Returns(account);

        var payBill = new PayBill(logger, repository, eventBus);

        // Act
        var result = await payBill.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(accountId, result.Id);
        Assert.Equal(initialBalance - billAmount, account.Balance);
        Assert.Equal(2, account.Transactions.Count); // One credit + one bill payment
        var billTransaction = account.Transactions.Last();
        Assert.Equal(TransactionType.BillPayment, billTransaction.TransactionType);
        Assert.Equal(billAmount, billTransaction.Value);
        
        await repository.Received(1).GetByIdAsync(accountId);
        await repository.Received(1).UpsertAsync(account, Arg.Any<CancellationToken>());
        await eventBus.Received(1).PublishAsync(Arg.Any<Transaction>(), "accounts.update");
    }

    [Fact]
    public async Task PayBill_WithInsufficientBalance_ShouldThrowException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var billAmount = 200.00m;
        var initialBalance = 100.00m;

        var account = new Account(customerId);
        account.AddCredit(initialBalance);
        
        var accountId = account.Id;
        var request = new PayBillRequest(accountId, billAmount);

        var logger = Substitute.For<ILogger<PayBill>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(accountId).Returns(account);

        var payBill = new PayBill(logger, repository, eventBus);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await payBill.ExecuteAsync(request, CancellationToken.None));

        await repository.Received(1).GetByIdAsync(accountId);
        await repository.DidNotReceive().UpsertAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
        await eventBus.DidNotReceive().PublishAsync(Arg.Any<Transaction>(), Arg.Any<string>());
    }

    [Fact]
    public async Task PayBill_ShouldLogInformation()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var billAmount = 50.00m;
        var initialBalance = 100.00m;

        var account = new Account(customerId);
        account.AddCredit(initialBalance);
        
        var accountId = account.Id;
        var request = new PayBillRequest(accountId, billAmount);

        var logger = Substitute.For<ILogger<PayBill>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(accountId).Returns(account);

        var payBill = new PayBill(logger, repository, eventBus);

        // Act
        await payBill.ExecuteAsync(request, CancellationToken.None);

        // Assert
        logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Bill payment processed")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task PayBill_WithNonExistentAccount_ShouldThrowNotFoundException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var billAmount = 100.00m;
        var request = new PayBillRequest(accountId, billAmount);

        var logger = Substitute.For<ILogger<PayBill>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(accountId).Returns((Account?)null);

        var payBill = new PayBill(logger, repository, eventBus);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await payBill.ExecuteAsync(request, CancellationToken.None));

        Assert.Contains(accountId.ToString(), exception.Message);
        await repository.Received(1).GetByIdAsync(accountId);
        await repository.DidNotReceive().UpsertAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
        await eventBus.DidNotReceive().PublishAsync(Arg.Any<Transaction>(), Arg.Any<string>());
    }
}
