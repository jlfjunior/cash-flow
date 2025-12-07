using System.Collections.Generic;

namespace CashFlow.Lib.Sharable
{
    public abstract class Entity
    {
        private readonly List<IEvent> _events = new();

        public IReadOnlyCollection<IEvent> Events => _events.AsReadOnly();
    
        public void AddEvent(IEvent @event) => _events.Add(@event);
    
        public void ClearEvents() => _events.Clear();
    }
}