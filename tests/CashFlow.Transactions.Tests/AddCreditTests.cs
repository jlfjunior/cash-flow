using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Features.Deposit;
using CashFlow.Features.Transactions.Requests;
using CashFlow.Features.Withdraw;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CashFlow.Transactions.Tests;

public class AddCreditTests
{
    [Fact]
    public async Task CreateTransaction_WithCreditDirection_ShouldProcessCredit()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var creditAmount = 150.75m;
        var initialBalance = 50.00m;

        var account = new Account(customerId);
        account.AddCredit(initialBalance);
        
        var accountId = account.Id;
        var request = new DepositRequest(accountId, "Credit", creditAmount);

        var logger = Substitute.For<ILogger<Withdraw>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(accountId).Returns(account);

        var createTransaction = new Deposit(logger, repository, eventBus);

        // Act
        var result = await createTransaction.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(accountId, result.Id);
        Assert.Equal(initialBalance + creditAmount, account.Balance);
        Assert.Equal(2, account.Transactions?.Count); // One initial credit + one new credit
        var creditTransaction = account.Transactions?.Last();
        Assert.Equal(TransactionType.Deposit, creditTransaction?.TransactionType);
        Assert.Equal(Direction.Credit, creditTransaction?.Direction);
        Assert.Equal(creditAmount, creditTransaction?.Value);
        
        await repository.Received(1).GetByIdAsync(accountId);
        await repository.Received(1).UpsertAsync(account, Arg.Any<CancellationToken>());
        await eventBus.Received(1).PublishAsync(Arg.Any<Transaction>(), "accounts.update");
    }

    [Fact]
    public async Task CreateTransaction_WithCreditDirectionAndZeroBalance_ShouldProcessCredit()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var creditAmount = 100.00m;

        var account = new Account(customerId);
        var accountId = account.Id;
        var request = new DepositRequest(accountId, "Credit", creditAmount);

        var logger = Substitute.For<ILogger<Withdraw>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(accountId).Returns(account);

        var createTransaction = new Deposit(logger, repository, eventBus);

        // Act
        var result = await createTransaction.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(accountId, result.Id);
        Assert.Equal(creditAmount, account.Balance);
        Assert.Equal(1, account.Transactions?.Count);
        var creditTransaction = account.Transactions?.First();
        Assert.Equal(TransactionType.Deposit, creditTransaction?.TransactionType);
        Assert.Equal(creditAmount, creditTransaction?.Value);
    }

    [Fact]
    public async Task CreateTransaction_WithCreditDirection_ShouldLogInformation()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var creditAmount = 75.50m;

        var account = new Account(customerId);
        var accountId = account.Id;
        var request = new DepositRequest(accountId, "Credit", creditAmount);

        var logger = Substitute.For<ILogger<Withdraw>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(accountId).Returns(account);

        var createTransaction = new Deposit(logger, repository, eventBus);

        // Act
        await createTransaction.ExecuteAsync(request, CancellationToken.None);

        // Assert
        logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Transaction created")),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
