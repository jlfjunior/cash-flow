using CashFlow.Lib.Sharable;

namespace CashFlow.Transactions.Domain.Events;

public record AccountCreated(Guid AccountId, Guid CustomerId) : IEvent;