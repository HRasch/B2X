using System;

namespace B2Connect.Shared.Events
{
    /// <summary>
    /// Base record for all domain events
    /// Used in event-driven architecture for communication between services
    /// </summary>
    public abstract record DomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string EventType { get; set; }
        public Guid AggregateId { get; set; }
        public string AggregateType { get; set; }
        public int Version { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
