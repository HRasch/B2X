namespace B2X.Store.API.Models
{
    public class AnalyticsEvent
    {
        public required string EventType { get; set; }
        public required string UserId { get; set; }
        public required string SessionId { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new(StringComparer.Ordinal);
    }
}
