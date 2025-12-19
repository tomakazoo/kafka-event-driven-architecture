# 📊 Monitoring & Data Visualization Guide

This guide shows you how to access all monitoring dashboards and view data/calculations from your event-driven NAV calculator system.

## 🚀 Quick Access URLs

All services are running! Access them at:

| Service | URL | Credentials | Purpose |
|---------|-----|-------------|---------|
| **Demo UI** | http://localhost:3001 | None | Interactive interface to trigger calculations |
| **Grafana** | http://localhost:3000 | admin/admin | Metrics dashboards |
| **Jaeger** | http://localhost:16686 | None | Distributed tracing |
| **Prometheus** | http://localhost:9090 | None | Raw metrics data |
| **Pricing Service** | http://localhost:5001 | None | API health endpoint |
| **NAV Calculator 1** | http://localhost:5002 | None | Service health |
| **NAV Calculator 2** | http://localhost:5003 | None | Service health |
| **Notification Service** | http://localhost:5004 | None | Service health |

---

## 📈 Step-by-Step: Viewing Data & Calculations

### Step 1: Generate Some Data

**Option A: Use the Demo UI (Easiest)**
1. Open http://localhost:3001 in your browser
2. Click on any fund card to trigger a NAV calculation
3. Watch the real-time updates as events flow through the system
4. Click "Simulate Month-End" to process all 28 funds at once

**Option B: Use the Load Tester**
```bash
cd src/Tools/LoadTester
dotnet run -- --funds 28 --concurrent true --iterations 3
```

**Option C: Use curl/API**
```bash
# Trigger a single calculation
curl -X POST http://localhost:5001/api/pricing/trigger \
  -H "Content-Type: application/json" \
  -d '{"fundId":"LUX-001","fundName":"Test Fund"}'
```

### Step 2: View Metrics in Grafana

1. **Login to Grafana**
   - Go to http://localhost:3000
   - Username: `admin`
   - Password: `admin`

2. **Access Pre-configured Dashboard**
   - Click on "Dashboards" (left sidebar) → "Browse"
   - Look for "NAV Calculator Dashboard" or similar
   - This dashboard shows:
     - Events processed per second
     - Processing duration (P50, P95, P99)
     - Success/error rates
     - Load distribution across NAV calculator instances

3. **Explore Metrics**
   - Click "Explore" (left sidebar)
   - Try these PromQL queries:
     ```
     # Events processed per second
     rate(events_processed_total[5m])
     
     # Processing duration (95th percentile)
     histogram_quantile(0.95, rate(event_processing_duration_seconds_bucket[5m]))
     
     # Success rate
     rate(events_processed_total{status="success"}[5m]) / rate(events_processed_total[5m])
     ```

### Step 3: View Distributed Traces in Jaeger

1. **Open Jaeger UI**
   - Go to http://localhost:16686

2. **Find Traces**
   - Select service: `pricing-service` or `nav-calculator-1`
   - Click "Find Traces"
   - You'll see traces for each calculation request

3. **Follow a Complete Flow**
   - Click on any trace to see the full request flow:
     - Pricing Service → publishes event
     - NAV Calculator → processes and calculates NAV
     - Notification Service → sends notification
   - Each span shows timing, tags, and correlation IDs

4. **Search by Correlation ID**
   - If you have a correlation ID from the Demo UI or logs:
     - Use the "Tags" search: `correlation_id=<your-id>`
     - This shows the complete end-to-end trace

### Step 4: View Raw Metrics in Prometheus

1. **Open Prometheus**
   - Go to http://localhost:9090

2. **Check Targets**
   - Click "Status" → "Targets"
   - All services should show as "UP"
   - This confirms metrics are being scraped

3. **Query Metrics**
   - Click "Graph" tab
   - Try these queries:
     ```
     events_processed_total
     event_processing_duration_seconds
     events_processed_total{status="success"}
     events_processed_total{status="error"}
     ```

4. **View Service Metrics Endpoints**
   - Each service exposes metrics at `/metrics`:
     - http://localhost:5001/metrics (Pricing Service)
     - http://localhost:5002/metrics (NAV Calculator 1)
     - http://localhost:5003/metrics (NAV Calculator 2)
     - http://localhost:5004/metrics (Notification Service)

---

## 🎯 What to Look For

### In Grafana Dashboards

**Key Metrics:**
- **Throughput**: Events processed per second (should increase during load)
- **Latency**: P95/P99 processing duration (should be low, <1s typically)
- **Error Rate**: Should be 0% or very low
- **Load Distribution**: Both NAV calculator instances should share load

**During Month-End Simulation:**
- Throughput spikes to ~28 events/second
- Both NAV calculator instances process events (load balancing)
- Latency may increase slightly but should remain stable

### In Jaeger Traces

**What to See:**
- **Service Map**: Visual representation of service interactions
- **Trace Timeline**: See how long each step takes
- **Correlation IDs**: Follow a single request through all services
- **Spans**: Each service operation creates a span with timing

**Example Trace Flow:**
```
pricing-service (trigger_pricing)
  └─> nav-calculator-1 (process_event)
      └─> notification-service (process_event)
```

### In Prometheus

**Available Metrics:**
- `events_processed_total`: Counter of total events
- `event_processing_duration_seconds`: Histogram of processing times
- Labels: `service`, `event_type`, `status`

---

## 🧪 Generating More Data

### Run Load Tests

```bash
# Concurrent processing (simulates month-end)
cd src/Tools/LoadTester
dotnet run -- --funds 28 --concurrent true --iterations 3

# Sequential processing
dotnet run -- --funds 28 --concurrent false

# Heavy load test
dotnet run -- --funds 100 --concurrent true --iterations 5
```

### Monitor in Real-Time

While load tests run:
1. **Grafana**: Watch metrics update in real-time
2. **Jaeger**: See traces appear as events are processed
3. **Demo UI**: See notifications appear

---

## 📊 Service Health Checks

Check if services are healthy:

```bash
# All services
curl http://localhost:5001/health  # Pricing Service
curl http://localhost:5002/health  # NAV Calculator 1
curl http://localhost:5003/health  # NAV Calculator 2
curl http://localhost:5004/health  # Notification Service

# Or use docker compose
docker compose ps
```

---

## 🔍 Troubleshooting

### No Data in Grafana?
1. Check Prometheus targets: http://localhost:9090/targets
2. Verify services are exposing metrics: http://localhost:5001/metrics
3. Check Prometheus config: `monitoring/prometheus.yml`

### No Traces in Jaeger?
1. Verify services are sending traces (check logs)
2. Check Jaeger is running: `docker compose ps jaeger`
3. Look for correlation IDs in service logs

### Demo UI Not Working?
1. Check pricing service: http://localhost:5001/health
2. Check browser console for errors
3. Verify demo-ui container is running: `docker compose ps demo-ui`

---

## 📝 Example Workflow

1. **Start**: Open Demo UI at http://localhost:3001
2. **Trigger**: Click "Simulate Month-End" button
3. **Watch Grafana**: See metrics spike in real-time
4. **Check Jaeger**: Find traces for the correlation IDs
5. **View Prometheus**: Query raw metrics data
6. **Run Load Test**: Generate more data for analysis

---

## 🎓 Learning Tips

- **Compare Metrics**: Run concurrent vs sequential load tests and compare latency
- **Trace Analysis**: Use Jaeger to identify bottlenecks
- **Load Distribution**: Watch how Kafka distributes load across NAV calculator instances
- **Error Scenarios**: Stop a service and see how errors appear in monitoring

---

Happy Monitoring! 🚀

