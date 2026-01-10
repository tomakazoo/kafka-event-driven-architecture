using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaStreams.Domain;

namespace KafkaStreams.Streams
{
    /// <summary>
    /// Simulates Kafka Streams processing topology
    /// In production, use Kafka Streams library (Java) or ksqlDB
    /// This demonstrates stream processing concepts:
    /// - Filtering
    /// - Aggregation
    /// - Windowing
    /// - Stateful operations
    /// </summary>
    public class OrderProcessingTopology
    {
        private readonly IConsumer<string, string> _sourceConsumer;
        private readonly IProducer<string, string> _sinkProducer;
        private readonly Dictionary<string, OrderStats> _customerStats;
        private readonly Dictionary<string, OrderStats> _productStats;
        private readonly object _lock = new object();

        public OrderProcessingTopology(
            IConsumer<string, string> sourceConsumer,
            IProducer<string, string> sinkProducer)
        {
            _sourceConsumer = sourceConsumer;
            _sinkProducer = sinkProducer;
            _customerStats = new Dictionary<string, OrderStats>();
            _productStats = new Dictionary<string, OrderStats>();
        }

        /// <summary>
        /// Process orders in real-time
        /// Simulates: filter -> map -> aggregate -> sink
        /// </summary>
        public async Task ProcessStreamAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("🔄 Starting stream processing topology...");
            Console.WriteLine();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = _sourceConsumer.Consume(TimeSpan.FromSeconds(1));

                    if (result == null)
                        continue;

                    var order = Order.FromJson(result.Message.Value);

                    // Step 1: Filter (only process completed orders)
                    if (order.Status != "completed")
                    {
                        Console.WriteLine($"⏭️  Filtered out: Order {order.OrderId} (status: {order.Status})");
                        continue;
                    }

                    // Step 2: Process (aggregate statistics)
                    await ProcessOrderAsync(order);

                    // Step 3: Emit aggregated results
                    await EmitStatsAsync();
                }
                catch (ConsumeException e)
                {
                    if (!e.Error.IsFatal)
                        continue;
                    throw;
                }
            }
        }

        private async Task ProcessOrderAsync(Order order)
        {
            lock (_lock)
            {
                // Aggregate by customer
                if (!_customerStats.ContainsKey(order.CustomerId))
                {
                    _customerStats[order.CustomerId] = new OrderStats
                    {
                        Key = order.CustomerId,
                        Count = 0,
                        TotalAmount = 0,
                        AverageAmount = 0,
                        LastUpdated = DateTime.UtcNow
                    };
                }

                var customerStat = _customerStats[order.CustomerId];
                customerStat.Count++;
                customerStat.TotalAmount += order.Amount;
                customerStat.AverageAmount = customerStat.TotalAmount / customerStat.Count;
                customerStat.LastUpdated = DateTime.UtcNow;

                // Aggregate by product
                if (!_productStats.ContainsKey(order.ProductId))
                {
                    _productStats[order.ProductId] = new OrderStats
                    {
                        Key = order.ProductId,
                        Count = 0,
                        TotalAmount = 0,
                        AverageAmount = 0,
                        LastUpdated = DateTime.UtcNow
                    };
                }

                var productStat = _productStats[order.ProductId];
                productStat.Count++;
                productStat.TotalAmount += order.Amount;
                productStat.AverageAmount = productStat.TotalAmount / productStat.Count;
                productStat.LastUpdated = DateTime.UtcNow;
            }

            Console.WriteLine($"✅ Processed: Order {order.OrderId}");
            Console.WriteLine($"   Customer: {order.CustomerId}, Amount: ${order.Amount:F2}");
        }

        private async Task EmitStatsAsync()
        {
            lock (_lock)
            {
                // Emit customer statistics
                foreach (var stat in _customerStats.Values)
                {
                    _sinkProducer.Produce("customer-stats",
                        new Message<string, string>
                        {
                            Key = stat.Key,
                            Value = stat.ToJson()
                        },
                        (report) =>
                        {
                            if (!report.Error.IsError)
                            {
                                Console.WriteLine($"📊 Emitted customer stats: {stat.Key} (Count: {stat.Count}, Total: ${stat.TotalAmount:F2})");
                            }
                        });
                }

                // Emit product statistics
                foreach (var stat in _productStats.Values)
                {
                    _sinkProducer.Produce("product-stats",
                        new Message<string, string>
                        {
                            Key = stat.Key,
                            Value = stat.ToJson()
                        },
                        (report) =>
                        {
                            if (!report.Error.IsError)
                            {
                                Console.WriteLine($"📊 Emitted product stats: {stat.Key} (Count: {stat.Count}, Total: ${stat.TotalAmount:F2})");
                            }
                        });
                }
            }

            await Task.CompletedTask;
        }

        public void PrintStats()
        {
            lock (_lock)
            {
                Console.WriteLine();
                Console.WriteLine("═══════════════════════════════════════════════════════");
                Console.WriteLine("📊 Current Statistics");
                Console.WriteLine("═══════════════════════════════════════════════════════");
                Console.WriteLine();

                Console.WriteLine("👥 Customer Statistics:");
                foreach (var stat in _customerStats.Values.OrderByDescending(s => s.TotalAmount))
                {
                    Console.WriteLine($"   {stat.Key}:");
                    Console.WriteLine($"      Orders: {stat.Count}");
                    Console.WriteLine($"      Total: ${stat.TotalAmount:F2}");
                    Console.WriteLine($"      Average: ${stat.AverageAmount:F2}");
                }

                Console.WriteLine();
                Console.WriteLine("📦 Product Statistics:");
                foreach (var stat in _productStats.Values.OrderByDescending(s => s.TotalAmount))
                {
                    Console.WriteLine($"   {stat.Key}:");
                    Console.WriteLine($"      Orders: {stat.Count}");
                    Console.WriteLine($"      Total: ${stat.TotalAmount:F2}");
                    Console.WriteLine($"      Average: ${stat.AverageAmount:F2}");
                }
            }
        }
    }
}



