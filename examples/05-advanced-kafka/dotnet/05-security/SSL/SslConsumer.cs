using System;
using System.Threading;
using Confluent.Kafka;

namespace Security.SSL
{
    /// <summary>
    /// SSL/TLS Consumer Example
    /// 
    /// NOTE: This requires Kafka broker to be configured with SSL.
    /// For local development, SSL is typically not enabled.
    /// This example shows the configuration needed.
    /// </summary>
    class SslConsumer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🔒 SSL/TLS Consumer Example");
            Console.WriteLine("⚠️  NOTE: Requires SSL-enabled Kafka broker");
            Console.WriteLine();

            // SSL Configuration
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9093", // SSL port (default: 9093)
                
                // SSL/TLS Settings
                SecurityProtocol = SecurityProtocol.Ssl,
                SslCaLocation = "/path/to/ca-cert",           // CA certificate
                SslCertificateLocation = "/path/to/client-cert", // Client certificate
                SslKeyLocation = "/path/to/client-key",        // Client private key
                SslKeyPassword = "password",                  // Key password (if needed)
                
                // Standard consumer settings
                GroupId = "ssl-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            Console.WriteLine("📋 SSL Configuration:");
            Console.WriteLine($"   Security Protocol: {config.SecurityProtocol}");
            Console.WriteLine($"   CA Location: {config.SslCaLocation}");
            Console.WriteLine($"   Certificate Location: {config.SslCertificateLocation}");
            Console.WriteLine($"   Key Location: {config.SslKeyLocation}");
            Console.WriteLine();

            try
            {
                using var consumer = new ConsumerBuilder<string, string>(config).Build();
                consumer.Subscribe("secure-topic");

                Console.WriteLine("✅ SSL Consumer created");
                Console.WriteLine("💡 This example shows SSL configuration");
                Console.WriteLine("💡 For local development, SSL is typically not enabled");
                Console.WriteLine();
                Console.WriteLine("📚 See SSL/README.md for setup instructions");
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error: {e.Message}");
                Console.WriteLine();
                Console.WriteLine("💡 SSL requires broker configuration:");
                Console.WriteLine("   1. Generate certificates");
                Console.WriteLine("   2. Configure broker with SSL listeners");
                Console.WriteLine("   3. Update client configuration");
            }
        }
    }
}

