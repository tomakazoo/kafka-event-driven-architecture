# Kafka Troubleshooting Guide

## ⚠️ Issue: Kafka Service Not Starting

Based on your terminal output, I see that **Zookeeper and Kafka UI are running**, but **Kafka broker is NOT running**.

---

## 🔧 Quick Fix

Run this in your terminal (where you have `newgrp docker` active):

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/fix-kafka.sh
```

This will:
1. ✅ Stop all containers
2. ✅ Clean up old data
3. ✅ Pull fresh images
4. ✅ Start all services
5. ✅ Show you the logs

---

## 🔍 Manual Diagnosis

If the quick fix doesn't work, run:

```bash
./scripts/diagnose-kafka.sh
```

This will show you:
- Container status
- Kafka logs
- Zookeeper logs
- Port conflicts
- Disk space

---

## 🐛 Common Issues & Solutions

### Issue 1: Kafka Container Exits Immediately

**Symptom:** Kafka container starts but stops right away

**Solution:**
```bash
# Check logs
docker compose logs kafka

# Restart with fresh data
docker compose down -v
./start-kafka.sh
```

### Issue 2: Connection Refused to Zookeeper

**Symptom:** Kafka can't connect to Zookeeper

**Solution:**
```bash
# Check if Zookeeper is healthy
docker compose logs zookeeper

# Restart Zookeeper first
docker compose restart zookeeper
sleep 10
docker compose restart kafka
```

### Issue 3: Port Already in Use

**Symptom:** Error binding to port 9092

**Solution:**
```bash
# Find what's using the port
sudo lsof -i :9092

# Kill the process or stop conflicting service
docker compose down
# Kill the conflicting process
./start-kafka.sh
```

### Issue 4: Permission Denied on kafka-data Volume

**Symptom:** Permission errors in Kafka logs

**Solution:**
```bash
# Fix permissions
sudo chown -R $USER:$USER ./kafka-data
chmod -R 755 ./kafka-data

# Restart
docker compose restart kafka
```

### Issue 5: Out of Disk Space

**Symptom:** No space left on device

**Solution:**
```bash
# Check disk space
df -h

# Clean up Docker
docker system prune -a

# Remove old Kafka data
docker compose down -v
```

---

## 📊 Checking Kafka Status

### Method 1: Using Docker Compose
```bash
docker compose ps
```

**Good Output:** All services show "Up"
```
NAME                                     STATUS
kafka-event-driven-architecture-kafka-1  Up
kafka-event-driven-architecture-...      Up
```

**Bad Output:** Kafka is missing or "Exited"
```
NAME                                     STATUS
kafka-event-driven-architecture-kafka-1  Exited (1)
```

### Method 2: Check Logs
```bash
# View all logs
docker compose logs

# View only Kafka logs
docker compose logs kafka

# Follow logs in real-time
docker compose logs -f kafka
```

### Method 3: Test Connectivity
```bash
# From inside the network
docker compose exec kafka kafka-broker-api-versions --bootstrap-server localhost:9092

# From host
docker run --rm --network host confluentinc/cp-kafka:7.5.0 kafka-broker-api-versions --bootstrap-server localhost:9092
```

---

## 🔄 Complete Reset

If nothing works, do a complete reset:

```bash
# Stop everything
docker compose down

# Remove ALL data (including volumes)
docker compose down -v
rm -rf ./kafka-data

# Remove Docker cache
docker system prune -a

# Start fresh
./scripts/start-kafka.sh

# Wait 30 seconds for startup
sleep 30

# Verify
./scripts/verify-docker.sh
```

---

## 📝 What to Look for in Logs

### Healthy Kafka Startup:
```
INFO [KafkaServer id=1] started
INFO Kafka version: 7.5.0
INFO Registered broker 1 at path /brokers/ids/1
```

### Problem Indicators:
```
ERROR [KafkaServer id=1] Fatal error during KafkaServer startup
ERROR Connection to zookeeper refused
ERROR Address already in use
ERROR No space left on device
```

---

## 🆘 Still Having Issues?

1. **Check your terminal output has these lines:**
   ```
   ✔ Container kafka-event-driven-architecture-kafka-1 Started
   ```

2. **Run diagnostic:**
   ```bash
   ./scripts/diagnose-kafka.sh | tee kafka-diagnostic.log
   ```

3. **Check if Kafka container exists:**
   ```bash
   docker ps -a | grep kafka
   ```

4. **View real-time logs:**
   ```bash
   docker compose logs -f kafka
   ```

5. **Try starting manually:**
   ```bash
   docker compose up kafka
   ```
   (This shows logs in the foreground)

---

## ✅ Next Steps

Once Kafka is running successfully:

1. ✅ Verify with: `./scripts/verify-docker.sh`
2. ✅ Check Kafka UI: http://localhost:8080
3. ✅ Run examples: `cd examples/01-fundamentals/dotnet && ./run-example.sh`

---

## 📚 Helpful Commands Reference

```bash
# View all containers
docker ps -a

# View running containers only
docker ps

# View logs for specific service
docker compose logs [service-name]

# Restart specific service
docker compose restart [service-name]

# Stop all services
docker compose down

# Stop and remove volumes
docker compose down -v

# Start in foreground (see logs)
docker compose up

# Start in background
docker compose up -d

# Scale a service
docker compose up -d --scale kafka=1
```

