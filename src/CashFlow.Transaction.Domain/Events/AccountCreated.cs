using CashFlow.Lib.Sharable;

namespace CashFlow.Transaction.Domain.Events;

public record AccountCreated(Guid AccountId, Guid CustomerId) : IEvent;