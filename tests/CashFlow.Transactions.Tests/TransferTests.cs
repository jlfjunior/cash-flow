using CashFlow.Domain.Entities;
using CashFlow.Domain.Exceptions;
using CashFlow.Domain.Repositories;
using CashFlow.Features.Transactions;
using CashFlow.Features.Transactions.Requests;
using CashFlow.Lib.EventBus;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CashFlow.Transactions.Tests;

public class TransferTests
{
    [Fact]
    public async Task Transfer_WithSufficientBalance_ShouldProcessTransfer()
    {
        // Arrange
        var sourceCustomerId = Guid.NewGuid();
        var destinationCustomerId = Guid.NewGuid();
        var transferAmount = 100.50m;
        var sourceInitialBalance = 200.00m;
        var destinationInitialBalance = 50.00m;

        var sourceAccount = new Account(sourceCustomerId);
        sourceAccount.AddCredit(sourceInitialBalance);
        
        var destinationAccount = new Account(destinationCustomerId);
        destinationAccount.AddCredit(destinationInitialBalance);
        
        var sourceAccountId = sourceAccount.Id;
        var destinationAccountId = destinationAccount.Id;
        var request = new TransferRequest(sourceAccountId, destinationAccountId, transferAmount);

        var logger = Substitute.For<ILogger<Transfer>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(sourceAccountId).Returns(sourceAccount);
        repository.GetByIdAsync(destinationAccountId).Returns(destinationAccount);

        var transfer = new Transfer(logger, repository, eventBus);

        // Act
        var result = await transfer.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(sourceAccountId, result.Id);
        Assert.Equal(sourceInitialBalance - transferAmount, sourceAccount.Balance);
        Assert.Equal(destinationInitialBalance + transferAmount, destinationAccount.Balance);
        
        // Source account should have 2 transactions (initial credit + transfer out)
        Assert.Equal(2, sourceAccount.Transactions?.Count);
        var sourceTransferTransaction = sourceAccount.Transactions?.Last();
        Assert.Equal(TransactionType.Transfer, sourceTransferTransaction?.TransactionType);
        Assert.Equal(Direction.Debit, sourceTransferTransaction?.Direction);
        Assert.Equal(transferAmount, sourceTransferTransaction?.Value);
        
        // Destination account should have 2 transactions (initial credit + transfer in)
        Assert.Equal(2, destinationAccount.Transactions?.Count);
        var destinationTransferTransaction = destinationAccount.Transactions?.Last();
        Assert.Equal(TransactionType.Deposit, destinationTransferTransaction?.TransactionType);
        Assert.Equal(Direction.Credit, destinationTransferTransaction?.Direction);
        Assert.Equal(transferAmount, destinationTransferTransaction?.Value);
        
        await repository.Received(1).GetByIdAsync(sourceAccountId);
        await repository.Received(1).GetByIdAsync(destinationAccountId);
        await repository.Received(1).UpsertAsync(sourceAccount, Arg.Any<CancellationToken>());
        await repository.Received(1).UpsertAsync(destinationAccount, Arg.Any<CancellationToken>());
        await eventBus.Received(2).PublishAsync(Arg.Any<Transaction>(), "accounts.update");
    }

    [Fact]
    public async Task Transfer_WithInsufficientBalance_ShouldThrowException()
    {
        // Arrange
        var sourceCustomerId = Guid.NewGuid();
        var destinationCustomerId = Guid.NewGuid();
        var transferAmount = 200.00m;
        var sourceInitialBalance = 100.00m;

        var sourceAccount = new Account(sourceCustomerId);
        sourceAccount.AddCredit(sourceInitialBalance);
        
        var destinationAccount = new Account(destinationCustomerId);
        
        var sourceAccountId = sourceAccount.Id;
        var destinationAccountId = destinationAccount.Id;
        var request = new TransferRequest(sourceAccountId, destinationAccountId, transferAmount);

        var logger = Substitute.For<ILogger<Transfer>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(sourceAccountId).Returns(sourceAccount);
        repository.GetByIdAsync(destinationAccountId).Returns(destinationAccount);

        var transfer = new Transfer(logger, repository, eventBus);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await transfer.ExecuteAsync(request, CancellationToken.None));

        await repository.Received(1).GetByIdAsync(sourceAccountId);
        await repository.Received(1).GetByIdAsync(destinationAccountId);
        await repository.DidNotReceive().UpsertAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
        await eventBus.DidNotReceive().PublishAsync(Arg.Any<Transaction>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Transfer_WithExactBalance_ShouldProcessTransfer()
    {
        // Arrange
        var sourceCustomerId = Guid.NewGuid();
        var destinationCustomerId = Guid.NewGuid();
        var transferAmount = 100.00m;
        var sourceInitialBalance = 100.00m;

        var sourceAccount = new Account(sourceCustomerId);
        sourceAccount.AddCredit(sourceInitialBalance);
        
        var destinationAccount = new Account(destinationCustomerId);
        
        var sourceAccountId = sourceAccount.Id;
        var destinationAccountId = destinationAccount.Id;
        var request = new TransferRequest(sourceAccountId, destinationAccountId, transferAmount);

        var logger = Substitute.For<ILogger<Transfer>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(sourceAccountId).Returns(sourceAccount);
        repository.GetByIdAsync(destinationAccountId).Returns(destinationAccount);

        var transfer = new Transfer(logger, repository, eventBus);

        // Act
        var result = await transfer.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(sourceAccountId, result.Id);
        Assert.Equal(0, sourceAccount.Balance);
        Assert.Equal(transferAmount, destinationAccount.Balance);
    }

    [Fact]
    public async Task Transfer_ToSameAccount_ShouldThrowException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var transferAmount = 100.00m;
        var initialBalance = 200.00m;

        var account = new Account(customerId);
        account.AddCredit(initialBalance);
        
        var accountId = account.Id;
        var request = new TransferRequest(accountId, accountId, transferAmount);

        var logger = Substitute.For<ILogger<Transfer>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(accountId).Returns(account);

        var transfer = new Transfer(logger, repository, eventBus);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await transfer.ExecuteAsync(request, CancellationToken.None));

        await repository.Received(2).GetByIdAsync(accountId);
        await repository.DidNotReceive().UpsertAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
        await eventBus.DidNotReceive().PublishAsync(Arg.Any<Transaction>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Transfer_WithNonExistentSourceAccount_ShouldThrowNotFoundException()
    {
        // Arrange
        var sourceAccountId = Guid.NewGuid();
        var destinationAccountId = Guid.NewGuid();
        var transferAmount = 100.00m;
        var request = new TransferRequest(sourceAccountId, destinationAccountId, transferAmount);

        var logger = Substitute.For<ILogger<Transfer>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(sourceAccountId).Returns((Account?)null);

        var transfer = new Transfer(logger, repository, eventBus);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await transfer.ExecuteAsync(request, CancellationToken.None));

        Assert.Contains(sourceAccountId.ToString(), exception.Message);
        await repository.Received(1).GetByIdAsync(sourceAccountId);
        await repository.DidNotReceive().UpsertAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
        await eventBus.DidNotReceive().PublishAsync(Arg.Any<Transaction>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Transfer_WithNonExistentDestinationAccount_ShouldThrowNotFoundException()
    {
        // Arrange
        var sourceCustomerId = Guid.NewGuid();
        var sourceAccount = new Account(sourceCustomerId);
        sourceAccount.AddCredit(100.00m);
        
        var sourceAccountId = sourceAccount.Id;
        var destinationAccountId = Guid.NewGuid();
        var transferAmount = 50.00m;
        var request = new TransferRequest(sourceAccountId, destinationAccountId, transferAmount);

        var logger = Substitute.For<ILogger<Transfer>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(sourceAccountId).Returns(sourceAccount);
        repository.GetByIdAsync(destinationAccountId).Returns((Account?)null);

        var transfer = new Transfer(logger, repository, eventBus);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await transfer.ExecuteAsync(request, CancellationToken.None));

        Assert.Contains(destinationAccountId.ToString(), exception.Message);
        await repository.Received(1).GetByIdAsync(sourceAccountId);
        await repository.Received(1).GetByIdAsync(destinationAccountId);
        await repository.DidNotReceive().UpsertAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
        await eventBus.DidNotReceive().PublishAsync(Arg.Any<Transaction>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Transfer_ShouldLogInformation()
    {
        // Arrange
        var sourceCustomerId = Guid.NewGuid();
        var destinationCustomerId = Guid.NewGuid();
        var transferAmount = 75.50m;
        var sourceInitialBalance = 150.00m;

        var sourceAccount = new Account(sourceCustomerId);
        sourceAccount.AddCredit(sourceInitialBalance);
        
        var destinationAccount = new Account(destinationCustomerId);
        
        var sourceAccountId = sourceAccount.Id;
        var destinationAccountId = destinationAccount.Id;
        var request = new TransferRequest(sourceAccountId, destinationAccountId, transferAmount);

        var logger = Substitute.For<ILogger<Transfer>>();
        var repository = Substitute.For<IAccountRepository>();
        var eventBus = Substitute.For<IEventBus>();

        repository.GetByIdAsync(sourceAccountId).Returns(sourceAccount);
        repository.GetByIdAsync(destinationAccountId).Returns(destinationAccount);

        var transfer = new Transfer(logger, repository, eventBus);

        // Act
        await transfer.ExecuteAsync(request, CancellationToken.None);

        // Assert
        logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o != null && o.ToString() != null && o.ToString().Contains("Transfer processed")),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
