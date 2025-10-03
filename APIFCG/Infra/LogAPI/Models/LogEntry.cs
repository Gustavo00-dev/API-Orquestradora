using System;

namespace APIFCG.Infra.LogAPI.Models
{
    public class LogEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Level { get; set; }
        public string? Message { get; set; }
        public string? CorrelationId { get; set; }
        public string? Source { get; set; }
    }
}
