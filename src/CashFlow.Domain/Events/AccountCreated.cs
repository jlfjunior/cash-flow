using CashFlow.Domain;

namespace CashFlow.Domain.Events;

public record AccountCreated(Guid AccountId, Guid CustomerId) : IEvent;
