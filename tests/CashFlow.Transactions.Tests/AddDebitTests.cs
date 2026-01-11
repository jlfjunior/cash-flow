using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Features.Transactions;
using CashFlow.Features.Transactions.Requests;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CashFlow.Transactions.Tests;

public class AddDebitTests
{
    [Fact]
    public async Task CreateTransaction_WithDebitDirectionAndSufficientBalance_ShouldProcessDebit()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var debitAmount = 80.25m;
        var initialBalance = 200.00m;

        var account = new Account(customerId);
        account.AddCredit(initialBalance);
        
        var accountId = account.Id;
        var request = new CreateTransactionRequest(accountId, "Debit", debitAmount);

        var logger = Substitute.For<ILogger<CreateTransaction>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(accountId).Returns(account);

        var createTransaction = new CreateTransaction(logger, repository, eventBus);

        // Act
        var result = await createTransaction.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(accountId, result.Id);
        Assert.Equal(initialBalance - debitAmount, account.Balance);
        Assert.Equal(2, account.Transactions?.Count); // One credit + one debit
        var debitTransaction = account.Transactions?.Last();
        Assert.Equal(TransactionType.Withdraw, debitTransaction?.TransactionType);
        Assert.Equal(Direction.Debit, debitTransaction?.Direction);
        Assert.Equal(debitAmount, debitTransaction?.Value);
        
        await repository.Received(1).GetByIdAsync(accountId);
        await repository.Received(1).UpsertAsync(account, Arg.Any<CancellationToken>());
        await eventBus.Received(1).PublishAsync(Arg.Any<Transaction>(), "accounts.update");
    }

    [Fact]
    public async Task CreateTransaction_WithDebitDirectionAndInsufficientBalance_ShouldThrowException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var debitAmount = 150.00m;
        var initialBalance = 100.00m;

        var account = new Account(customerId);
        account.AddCredit(initialBalance);
        
        var accountId = account.Id;
        var request = new CreateTransactionRequest(accountId, "Debit", debitAmount);

        var logger = Substitute.For<ILogger<CreateTransaction>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(accountId).Returns(account);

        var createTransaction = new CreateTransaction(logger, repository, eventBus);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await createTransaction.ExecuteAsync(request, CancellationToken.None));

        await repository.Received(1).GetByIdAsync(accountId);
        await repository.DidNotReceive().UpsertAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
        await eventBus.DidNotReceive().PublishAsync(Arg.Any<Transaction>(), Arg.Any<string>());
    }

    [Fact]
    public async Task CreateTransaction_WithDebitDirectionAndExactBalance_ShouldProcessDebit()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var debitAmount = 100.00m;
        var initialBalance = 100.00m;

        var account = new Account(customerId);
        account.AddCredit(initialBalance);
        
        var accountId = account.Id;
        var request = new CreateTransactionRequest(accountId, "Debit", debitAmount);

        var logger = Substitute.For<ILogger<CreateTransaction>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(accountId).Returns(account);

        var createTransaction = new CreateTransaction(logger, repository, eventBus);

        // Act
        var result = await createTransaction.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(accountId, result.Id);
        Assert.Equal(0, account.Balance);
        Assert.Equal(2, account.Transactions?.Count);
        var debitTransaction = account.Transactions?.Last();
        Assert.Equal(TransactionType.Withdraw, debitTransaction?.TransactionType);
        Assert.Equal(debitAmount, debitTransaction?.Value);
    }

    [Fact]
    public async Task CreateTransaction_WithDebitDirection_ShouldLogInformation()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var debitAmount = 50.00m;
        var initialBalance = 100.00m;

        var account = new Account(customerId);
        account.AddCredit(initialBalance);
        
        var accountId = account.Id;
        var request = new CreateTransactionRequest(accountId, "Debit", debitAmount);

        var logger = Substitute.For<ILogger<CreateTransaction>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(accountId).Returns(account);

        var createTransaction = new CreateTransaction(logger, repository, eventBus);

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
