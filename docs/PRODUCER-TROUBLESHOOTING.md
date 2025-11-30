# Producer Hanging - Troubleshooting Guide

## 🔍 Quick Diagnostic Steps

If your producer is hanging and not producing messages, follow these steps in order:

---

## Step 1: Check Kafka is Running

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
docker compose ps
```

**Expected:** All services show "Up"
```
NAME                                    STATUS
kafka-event-driven-architecture-kafka-1 Up
```

**If Kafka is down:**
```bash
./scripts/start-kafka.sh
# Wait 30 seconds
./scripts/verify-docker.sh
```

---

## Step 2: Test Kafka Connectivity

```bash
# Test from host
docker run --rm --network host confluentinc/cp-kafka:7.5.0 \
  kafka-broker-api-versions --bootstrap-server localhost:9092
```

**Expected:** Shows broker API versions

**If connection fails:**
- Kafka might not be fully started (wait 30 seconds)
- Port 9092 might be blocked
- Check firewall settings

---

## Step 3: Check Kafka Logs

```bash
docker compose logs kafka --tail 50
```

**Look for:**
- ✅ `INFO [KafkaServer id=1] started` - Kafka is healthy
- ❌ `ERROR` messages - Indicates problems
- ❌ `Connection refused` - Network issues
- ❌ `Address already in use` - Port conflict

---

## Step 4: Verify Producer Can Connect

Run the producer with verbose logging:

```bash
cd examples/01-fundamentals/dotnet
dotnet run --project BasicProducer.csproj
```

**If it hangs:**
- Wait 5 seconds (timeout should trigger)
- Check for error messages
- Press Ctrl+C to cancel

---

## Step 5: Check Network Connectivity

```bash
# Test if port 9092 is accessible
telnet localhost 9092
# OR
nc -zv localhost 9092
```

**Expected:** Connection successful

**If connection refused:**
- Kafka container might not be running
- Port mapping might be incorrect
- Check `docker compose ps`

---

## 🔧 Common Causes & Solutions

### Issue 1: Kafka Broker Not Responding

**Symptoms:**
- Producer hangs indefinitely
- No error messages
- Timeout doesn't trigger

**Solution:**
```bash
# Restart Kafka
docker compose restart kafka

# Wait 30 seconds for full startup
sleep 30

# Verify
./scripts/verify-docker.sh

# Try producer again
cd examples/01-fundamentals/dotnet
dotnet run --project BasicProducer.csproj
```

---

### Issue 2: Kafka Container Crashed

**Symptoms:**
- `docker compose ps` shows Kafka as "Exited"
- Producer can't connect

**Solution:**
```bash
# Check why it crashed
docker compose logs kafka --tail 100

# Restart with fresh data
docker compose down
docker compose up -d kafka

# Wait and verify
sleep 30
./scripts/verify-docker.sh
```

---

### Issue 3: Zookeeper Connection Issues

**Symptoms:**
- Kafka logs show "Connection to zookeeper refused"
- Producer hangs

**Solution:**
```bash
# Check Zookeeper
docker compose ps zookeeper
docker compose logs zookeeper --tail 50

# Restart Zookeeper first
docker compose restart zookeeper
sleep 10

# Then restart Kafka
docker compose restart kafka
sleep 30
```

---

### Issue 4: Topic Metadata Not Available

**Symptoms:**
- Producer hangs on first message
- No error, just waiting

**Solution:**
```bash
# Create topic manually
docker compose exec kafka kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic my-topic \
  --partitions 1 \
  --replication-factor 1

# Or let auto-create work (wait longer)
# Producer might hang while Kafka creates topic metadata
```

---

### Issue 5: Timeout Settings Not Working

**Symptoms:**
- Producer hangs even with timeout settings
- No error after 5 seconds

**Solution:**
The producer should fail after 5 seconds. If it doesn't:

1. **Check if you're using the latest code:**
   ```bash
   cd examples/01-fundamentals/dotnet
   cat BasicProducer.cs | grep -A 5 "RequestTimeoutMs"
   ```

2. **Rebuild the project:**
   ```bash
   dotnet clean
   dotnet build BasicProducer.csproj
   dotnet run --project BasicProducer.csproj
   ```

---

### Issue 6: Producer Waiting for Acknowledgment

**Symptoms:**
- Producer sends first message successfully
- Hangs on second message
- Kafka might be slow to respond

**Solution:**
```bash
# Check Kafka performance
docker stats kafka-event-driven-architecture-kafka-1

# Check disk I/O
docker compose exec kafka df -h

# Increase timeout if needed (edit BasicProducer.cs)
# Change RequestTimeoutMs to 10000 (10 seconds)
```

---

## 🛠️ Enhanced Producer with Diagnostics

If the producer keeps hanging, use this enhanced version with better diagnostics:

```csharp
using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Text.Json;

class BasicProducer
{
    static async Task Main()
    {
        Console.WriteLine("🚀 Starting Kafka Producer...");
        Console.WriteLine($"Connecting to: localhost:9092");
        
        var config = new ProducerConfig 
        { 
            BootstrapServers = "localhost:9092",
            RequestTimeoutMs = 5000,
            MessageTimeoutMs = 5000,
            SocketTimeoutMs = 5000,
            // Enable debug logging
            Debug = "broker,protocol"
        };
        
        try
        {
            using (var producer = new ProducerBuilder<string, string>(config)
                .SetErrorHandler((p, e) => 
                {
                    Console.WriteLine($"❌ Producer Error: {e.Reason}");
                })
                .SetLogHandler((p, m) => 
                {
                    if (m.Level >= SyslogLevel.Warning)
                    {
                        Console.WriteLine($"⚠️  Kafka Log: {m.Message}");
                    }
                })
                .Build())
            {
                Console.WriteLine("✅ Producer created successfully");
                
                var topic = "my-topic";
                Console.WriteLine($"📤 Producing to topic: {topic}");
                
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"Sending message {i}...");
                    var message = new { id = i, value = $"Message {i}" };
                    
                    try
                    {
                        var report = await producer.ProduceAsync(
                            topic,
                            new Message<string, string>
                            {
                                Key = $"key-{i}",
                                Value = JsonSerializer.Serialize(message)
                            });
                        Console.WriteLine($"✅ Delivered to {report.TopicPartitionOffset}");
                    }
                    catch (ProduceException<string, string> e)
                    {
                        Console.WriteLine($"❌ Failed to deliver message {i}: {e.Error.Reason}");
                        Console.WriteLine($"   Error Code: {e.Error.Code}");
                        throw;
                    }
                }
                
                Console.WriteLine("🔄 Flushing producer...");
                producer.Flush(TimeSpan.FromSeconds(5));
                Console.WriteLine("✅ All messages delivered successfully!");
            }
        }
        catch (KafkaException e)
        {
            Console.WriteLine($"❌ Kafka Exception: {e.Message}");
            Console.WriteLine($"   Inner Exception: {e.InnerException?.Message}");
            Environment.Exit(1);
        }
        catch (Exception e)
        {
            Console.WriteLine($"❌ Unexpected Error: {e.Message}");
            Console.WriteLine($"   Stack Trace: {e.StackTrace}");
            Environment.Exit(1);
        }
    }
}
```

---

## 🔍 Diagnostic Commands Reference

```bash
# Check all services
docker compose ps

# Check Kafka specifically
docker compose ps kafka

# View Kafka logs
docker compose logs kafka --tail 100

# Follow Kafka logs in real-time
docker compose logs -f kafka

# Test Kafka connectivity
docker compose exec kafka kafka-broker-api-versions \
  --bootstrap-server localhost:9092

# List topics
docker compose exec kafka kafka-topics --list \
  --bootstrap-server localhost:9092

# Describe topic
docker compose exec kafka kafka-topics --describe \
  --bootstrap-server localhost:9092 \
  --topic my-topic

# Check consumer groups
docker compose exec kafka kafka-consumer-groups --list \
  --bootstrap-server localhost:9092

# Check port
sudo lsof -i :9092

# Test network
nc -zv localhost 9092
```

---

## 📊 What to Check in Order

1. ✅ **Kafka is running** (`docker compose ps`)
2. ✅ **Kafka is healthy** (`docker compose logs kafka`)
3. ✅ **Port 9092 is accessible** (`nc -zv localhost 9092`)
4. ✅ **Producer code has timeouts** (check BasicProducer.cs)
5. ✅ **No port conflicts** (`sudo lsof -i :9092`)
6. ✅ **Sufficient disk space** (`df -h`)
7. ✅ **Docker has resources** (`docker stats`)

---

## 🆘 Still Hanging?

If producer still hangs after all checks:

1. **Use the enhanced producer** (above) to see detailed error messages
2. **Check Kafka UI** at http://localhost:8080 - see if topics exist
3. **Restart everything:**
   ```bash
   docker compose down
   docker compose up -d
   sleep 30
   ./scripts/verify-docker.sh
   ```
4. **Try Python producer** to isolate if it's a .NET-specific issue:
   ```bash
   cd examples/01-fundamentals/python
   python basic_producer.py
   ```

---

## 💡 Pro Tips

- **Always wait 30 seconds** after starting Kafka before running producer
- **Check logs first** - they usually tell you what's wrong
- **Use `docker compose logs -f kafka`** in a separate terminal while running producer
- **Producer should fail fast** with timeout - if it hangs longer, Kafka might be completely unresponsive

---

**Related Documentation:**
- [TROUBLESHOOTING.md](TROUBLESHOOTING.md) - General Kafka troubleshooting
- [QUICKSTART.md](QUICKSTART.md) - Setup guide
- [HOW-TO-RUN.md](../examples/01-fundamentals/HOW-TO-RUN.md) - Running examples guide

