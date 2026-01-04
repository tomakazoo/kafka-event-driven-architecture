using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Security.SASL
{
    /// <summary>
    /// SASL Producer Example (PLAIN authentication)
    /// 
    /// NOTE: This requires Kafka broker to be configured with SASL.
    /// For local development, SASL is typically not enabled.
    /// This example shows the configuration needed.
    /// 
    /// To enable SASL on Kafka:
    /// 1. Configure broker with SASL listeners
    /// 2. Create JAAS configuration file
    /// 3. Update client configuration below
    /// </summary>
    class SaslProducer
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🔐 SASL Producer Example (PLAIN)");
            Console.WriteLine("⚠️  NOTE: Requires SASL-enabled Kafka broker");
            Console.WriteLine();

            // SASL Configuration
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9093", // SASL port
                
                // SASL Settings
                SecurityProtocol = SecurityProtocol.SaslSsl, // or SaslPlaintext for non-SSL
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "alice",              // Username
                SaslPassword = "password",          // Password
                
                // SSL Settings (if using SaslSsl)
                SslCaLocation = "/path/to/ca-cert", // CA certificate for SSL
                
                // Standard producer settings
                Acks = Acks.All,
                RequestTimeoutMs = 5000,
                MessageTimeoutMs = 5000,
                SocketTimeoutMs = 5000
            };

            Console.WriteLine("📋 SASL Configuration:");
            Console.WriteLine($"   Security Protocol: {config.SecurityProtocol}");
            Console.WriteLine($"   SASL Mechanism: {config.SaslMechanism}");
            Console.WriteLine($"   Username: {config.SaslUsername}");
            Console.WriteLine($"   Password: {'*'.PadRight(config.SaslPassword?.Length ?? 0, '*')}");
            Console.WriteLine();

            try
            {
                using var producer = new ProducerBuilder<string, string>(config)
                    .SetErrorHandler((p, e) =>
                    {
                        Console.WriteLine($"❌ Producer Error: {e.Reason}");
                    })
                    .Build();

                Console.WriteLine("✅ SASL Producer created");
                Console.WriteLine("💡 This example shows SASL configuration");
                Console.WriteLine("💡 For local development, SASL is typically not enabled");
                Console.WriteLine();
                Console.WriteLine("📚 See SASL/README.md for setup instructions");
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error: {e.Message}");
                Console.WriteLine();
                Console.WriteLine("💡 SASL requires broker configuration:");
                Console.WriteLine("   1. Configure broker with SASL listeners");
                Console.WriteLine("   2. Create JAAS configuration");
                Console.WriteLine("   3. Update client configuration");
            }
        }
    }
}


