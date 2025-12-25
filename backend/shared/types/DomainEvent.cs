using System;

namespace B2Connect.Shared.Events
{
    /// <summary>
    /// Base record for all domain events
    /// Used in event-driven architecture for communication between services
    /// </summary>
    public abstract record DomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public abstract string EventType { get; }
        public abstract Guid AggregateId { get; }
        public abstract string AggregateType { get; }
        public virtual int Version { get; } = 1;
        public Dictionary<string, object> Metadata { get; } = new();
    }
}
