using CashFlow.Domain;

namespace CashFlow.Domain.Events;

public record CustomerCreated(Guid Id, string FullName) : IEvent;
