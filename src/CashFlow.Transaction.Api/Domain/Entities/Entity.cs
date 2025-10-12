using CashFlow.Transaction.Api.Domain.Events;
using MongoDB.Bson.Serialization.Attributes;

namespace CashFlow.Transaction.Api.Domain.Entities;

public abstract class Entity
{
    [BsonIgnore]
    private IList<IEvent> _events = new List<IEvent>();

    [BsonIgnore] 
    public IReadOnlyCollection<IEvent> Events => _events.AsReadOnly();
    
    public void AddEvent(IEvent @event) => _events.Add(@event);
    
    public void ClearEvents() => _events.Clear();
}