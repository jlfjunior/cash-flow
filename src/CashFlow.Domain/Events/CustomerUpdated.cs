using CashFlow.Domain;

namespace CashFlow.Domain.Events;

public record CustomerUpdated(Guid Id, string FullName) : IEvent;
