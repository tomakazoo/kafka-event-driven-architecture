using System;
using System.Text.Json;

namespace MultiDCReplication.Domain
{
    /// <summary>
    /// Event model for multi-datacenter replication
    /// </summary>
    public class ReplicationEvent
    {
        public string EventId { get; set; }
        public string SourceDC { get; set; }
        public string TargetDC { get; set; }
        public string Topic { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
        public long Offset { get; set; }
        public int Partition { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static ReplicationEvent FromJson(string json)
        {
            return JsonSerializer.Deserialize<ReplicationEvent>(json) ?? new ReplicationEvent();
        }
    }
}


