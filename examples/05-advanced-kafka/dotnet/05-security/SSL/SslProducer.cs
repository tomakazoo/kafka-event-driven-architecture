using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Security.SSL
{
    /// <summary>
    /// SSL/TLS Producer Example
    /// 
    /// NOTE: This requires Kafka broker to be configured with SSL.
    /// For local development, SSL is typically not enabled.
    /// This example shows the configuration needed.
    /// 
    /// To enable SSL on Kafka:
    /// 1. Generate certificates (see SSL/README.md)
    /// 2. Configure broker with SSL listeners
    /// 3. Update client configuration below
    /// </summary>
    class SslProducer
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🔒 SSL/TLS Producer Example");
            Console.WriteLine("⚠️  NOTE: Requires SSL-enabled Kafka broker");
            Console.WriteLine();

            // SSL Configuration
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9093", // SSL port (default: 9093)
                
                // SSL/TLS Settings
                SecurityProtocol = SecurityProtocol.Ssl,
                SslCaLocation = "/path/to/ca-cert",           // CA certificate
                SslCertificateLocation = "/path/to/client-cert", // Client certificate
                SslKeyLocation = "/path/to/client-key",        // Client private key
                SslKeyPassword = "password",                  // Key password (if needed)
                
                // Standard producer settings
                Acks = Acks.All,
                RequestTimeoutMs = 5000,
                MessageTimeoutMs = 5000,
                SocketTimeoutMs = 5000
            };

            Console.WriteLine("📋 SSL Configuration:");
            Console.WriteLine($"   Security Protocol: {config.SecurityProtocol}");
            Console.WriteLine($"   CA Location: {config.SslCaLocation}");
            Console.WriteLine($"   Certificate Location: {config.SslCertificateLocation}");
            Console.WriteLine($"   Key Location: {config.SslKeyLocation}");
            Console.WriteLine();

            try
            {
                using var producer = new ProducerBuilder<string, string>(config)
                    .SetErrorHandler((p, e) =>
                    {
                        Console.WriteLine($"❌ Producer Error: {e.Reason}");
                    })
                    .Build();

                Console.WriteLine("✅ SSL Producer created");
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

