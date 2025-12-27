# How to Run Event Sourcing Example

Complete step-by-step guide for running the event sourcing example with Kafka.

---

## 🎯 Prerequisites

- Kafka running (see main project setup)
- .NET 8.0 SDK installed
- Terminal windows available

---

## 📋 Step-by-Step Guide

### **Step 1: Start Kafka Infrastructure**

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Expected:** All Kafka services running and accessible.

---

### **Step 2: Build All Projects**

```bash
cd examples/06-event-sourcing/dotnet
dotnet build EventSourcing.sln
```

**Expected:** All projects build successfully.

---

### **Step 3: Run Order Service Example**

This demonstrates basic event sourcing - creating an order, modifying it, and rebuilding state from events.

```bash
dotnet run --project Examples/OrderService.csproj
```

**What happens:**

1. Creates an order with ID `ORD-001`
2. Adds items to the order
3. Sets shipping address
4. Submits the order
5. Records payment
6. Saves all events to Kafka
7. Loads order from event store (rebuilds from events)
8. Shows event history

**Expected output:**
```
🛒 Starting Event Sourcing Order Service...
📝 Creating Order ORD-001...
   Status: CREATED
   Version: 1

➕ Adding items...
   Added: PROD-001 (2x $29.99)
   Added: PROD-002 (1x $49.99)
   Total Items: 2
   Version: 3

📍 Setting shipping address...
   Address set: 123 Main St, Boston
   Version: 4

✅ Submitting order...
   Status: SUBMITTED
   Version: 5

💳 Recording payment...
   Status: PAID
   Version: 6

💾 Saving to event store...
   ✅ Saved 6 uncommitted events

🔄 Loading order from event store...
   Order ID: ORD-001
   Status: PAID
   Items: 2
   Version: 6

📋 Event History:
   [1] OrderCreated @ 14:23:45
   [2] ItemAdded @ 14:23:45
   [3] ItemAdded @ 14:23:45
   [4] ShippingAddressSet @ 14:23:45
   [5] OrderSubmitted @ 14:23:45
   [6] PaymentReceived @ 14:23:45

✅ Event Sourcing Demo Complete!
```

---

### **Step 4: Run Event Replay Example**

This demonstrates how state is rebuilt by replaying events.

```bash
dotnet run --project Examples/EventReplay.csproj
```

**What happens:**

1. Creates an order with multiple changes
2. Shows state after each change
3. Replays all events to rebuild state
4. Shows final state matches original

**Expected output:**
```
🔄 Event Replay Demo - Event Sourcing...
📝 Creating order with multiple changes...
   Step 1: Created - Status: CREATED, Version: 1
   Step 2: Added item - Items: 1, Version: 2
   Step 3: Added another item - Items: 2, Version: 3
   Step 4: Removed item - Items: 1, Version: 4
   Step 5: Set address - Has Address: True, Version: 5
   Step 6: Submitted - Status: SUBMITTED, Version: 6

🔄 Replaying events to rebuild order state...
   Found 6 events to replay:

   Replaying: OrderCreated (v1)
      → Status: CREATED
      → Items: 0
      → Has Address: False

   Replaying: ItemAdded (v2)
      → Status: CREATED
      → Items: 1
      → Has Address: False

   ... (continues for all events)

✅ Final State After Replay:
   Order ID: ORD-REPLAY-001
   Status: SUBMITTED
   Items: 1
      - PROD-Y: 2x $15
   Version: 6
```

---

### **Step 5: Run Time Travel Example**

This demonstrates viewing order state at different points in time.

```bash
dotnet run --project Examples/TimeTravel.csproj
```

**What happens:**

1. Creates an order with history over time
2. Captures timestamps at different points
3. Views order state at each timestamp
4. Shows how state changes over time

**Expected output:**
```
⏰ Time Travel Demo - Event Sourcing...
📝 Creating order with history...
   Order created with 4 events

⏰ Time Travel - Viewing order at different points in time:

📅 Point 1: 14:25:10 (just created)
   Status: CREATED
   Items: 1
   Version: 2

📅 Point 2: 14:25:11 (after adding PROD-B)
   Status: CREATED
   Items: 2
   Version: 3

📅 Point 3: 14:25:12 (after setting address)
   Status: CREATED
   Items: 2
   Has Address: True
   Version: 4

📅 Current: 14:25:13 (submitted)
   Status: SUBMITTED
   Items: 2
   Version: 5

✅ Time Travel Demo Complete!
```

---

## 🔍 View Events in Kafka UI

Open **http://localhost:8080** to see events stored in Kafka.

### **Topics View**

Navigate to: **Topics** → **order-events**

**What you'll see:**
- All domain events stored in Kafka
- Events keyed by Order ID (aggregate ID)
- JSON serialized event data
- Headers with event metadata (event-type, event-id, version)

### **Messages View**

Navigate to: **Topics** → **order-events** → **Messages**

**What you'll see:**
- All events with their keys and values
- Event types in headers
- Versions for each event
- Timestamps

---

## 🎯 Understanding the Flow

### **Event Sourcing Flow**

```
1. Command Received
   ↓
2. Aggregate Validates Command
   ↓
3. Domain Event Created
   ↓
4. Event Applied to Aggregate (state change)
   ↓
5. Event Added to UncommittedEvents
   ↓
6. Repository.Save() Called
   ↓
7. Events Saved to Kafka Event Store
   ↓
8. Events Published to Kafka Topic
```

### **Rebuilding State Flow**

```
1. Repository.Get() Called
   ↓
2. Load Events from Kafka
   ↓
3. Optionally Load Snapshot
   ↓
4. Create New Aggregate Instance
   ↓
5. Replay Events (Apply each event)
   ↓
6. Return Aggregate with Current State
```

---

## 🧪 Try These Scenarios

### **Scenario 1: Concurrent Modification**

Try modifying the same order from two different processes:

```bash
# Terminal 1
dotnet run --project Examples/OrderService.csproj

# Terminal 2 (while Terminal 1 is running)
# Modify OrderService.cs to use same Order ID
# Run again - should see ConcurrencyException
```

### **Scenario 2: View Event History**

After running OrderService, check Kafka UI:
- Navigate to `order-events` topic
- View all messages
- See complete event history

### **Scenario 3: Time Travel Query**

Run TimeTravel example, then modify it to query different timestamps:
- Change the timestamp in code
- See how order state changes

---

## 🛠️ Troubleshooting

### **Kafka not running?**

```bash
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### **Build errors?**

```bash
cd examples/06-event-sourcing/dotnet
dotnet clean
dotnet restore
dotnet build
```

### **Events not appearing in Kafka?**

1. Check Kafka is running: `docker compose ps`
2. Verify topic exists:
   ```bash
   docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092
   ```
3. Check Kafka UI: http://localhost:8080

### **ConcurrencyException?**

This is expected when:
- Two processes try to modify same aggregate
- Expected version doesn't match current version
- Solution: Retry with latest version

### **Order state incorrect after replay?**

- Check events are saved correctly
- Verify events are applied in order (by version)
- Check event handlers in `Order.Apply()` method

---

## 📚 Key Concepts Demonstrated

✅ **Event Sourcing**: Store events instead of current state  
✅ **Aggregate Pattern**: Order aggregate with business logic  
✅ **Event Store**: Kafka as persistent event store  
✅ **Event Replay**: Rebuild state from events  
✅ **Time Travel**: Query state at any point in time  
✅ **Optimistic Locking**: Version-based concurrency control  
✅ **Snapshots**: Performance optimization  

---

## 🎓 Next Steps

- Modify events to add new event types
- Add more aggregates (Customer, Product, etc.)
- Implement CQRS pattern (separate read/write models)
- Add event handlers for side effects
- Implement event versioning/migration

---

**Happy Event Sourcing! 🚀**


