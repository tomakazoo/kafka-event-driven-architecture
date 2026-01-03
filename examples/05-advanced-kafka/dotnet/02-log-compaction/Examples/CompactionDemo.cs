using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace LogCompaction.Examples
{
    /// <summary>
    /// Demo showing log compaction in action
    /// </summary>
    class CompactionDemo
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("   Log Compaction Demo");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();

            Console.WriteLine("📚 What is Log Compaction?");
            Console.WriteLine();
            Console.WriteLine("Log compaction keeps only the latest value for each key.");
            Console.WriteLine("Older updates are removed, but the latest state is preserved.");
            Console.WriteLine();
            Console.WriteLine("Use cases:");
            Console.WriteLine("  • User profiles (latest state per user)");
            Console.WriteLine("  • Configuration (latest config per key)");
            Console.WriteLine("  • State stores (latest state per entity)");
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("💡 Run the producer first to send updates");
            Console.WriteLine("💡 Then run the consumer to see the compacted log");
            Console.WriteLine();
        }
    }
}

