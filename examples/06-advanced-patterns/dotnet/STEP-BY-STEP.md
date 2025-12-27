# 🎯 Step-by-Step: Event Sourcing Pattern Demonstration

Complete walkthrough to see exactly how event sourcing works with Kafka.

---

## 🛑 Step 0: Clean Start

**Stop all services:**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
docker compose down
```

**Verify everything is stopped:**
```bash
docker compose ps
```

Should show no running containers.

---

## 📋 Step 1: Start Kafka Infrastructure

**Start Kafka and related services:**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
```

**Wait for services to be ready:**
```bash
./scripts/verify-docker.sh
```

**If services are not ready after waiting for 30 seconds then you can run:**
```bash
./scripts/fix-kafka.sh
```

**Expected output:**
```
✅ All services are running!
Access Kafka UI: http://localhost:8080
```

**Verify Kafka is accessible:**
```bash
docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092
```

---

## 📋 Step 2: Open Kafka UI (In Browser)

**Open Kafka UI:**
- URL: http://localhost:8080
- Keep this open to watch events appear in real-time

**Navigate to Topics:**
- Click "Topics" in the left sidebar
- You should see internal topics (we'll create `order-events` when we run the example)

---

## 📋 Step 3: Build the Event Sourcing Example

**Navigate to the example directory:**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture/examples/06-advanced-patterns/dotnet
```

**Build all projects:**
```bash
dotnet build EventSourcing.sln
```

**Expected output:**
```
Build succeeded.
```

---

## 📋 Step 4: Run OrderService - See Event Sourcing in Action

**Run the OrderService example:**
```bash
dotnet run --project Examples/OrderService.csproj
```

### **What You'll See:**

**1. Service Starting:**
```
🛒 Starting Event Sourcing Order Service...
📡 Connecting to Kafka: localhost:9092
✅ Connected successfully
```

**2. Creating Order:**
```
📝 Creating Order ORD-001...
   Status: CREATED
   Version: 1
```
**💡 What happened:** `OrderCreated` event was created and applied to the aggregate.

**3. Adding Items:**
```
➕ Adding items...
   Added: PROD-001 (2x $29.99)
   Added: PROD-002 (1x $49.99)
   Total Items: 2
   Version: 3
```
**💡 What happened:** Two `ItemAdded` events were created (versions 2 and 3).

**4. Setting Address:**
```
📍 Setting shipping address...
   Address set: 123 Main St, Boston
   Version: 4
```
**💡 What happened:** `ShippingAddressSet` event created (version 4).

**5. Submitting Order:**
```
✅ Submitting order...
   Status: SUBMITTED
   Version: 5
```
**💡 What happened:** `OrderSubmitted` event created (version 5).

**6. Recording Payment:**
```
💳 Recording payment...
   Status: PAID
   Version: 6
```
**💡 What happened:** `PaymentReceived` event created (version 6).

**7. Saving to Event Store:**
```
💾 Saving to event store...
   ✅ Saved 6 uncommitted events
   Total events: 6
```
**💡 What happened:** All 6 events are now being saved to Kafka topic `order-events`.

**8. Loading from Event Store:**
```
🔄 Loading order from event store...
   Order ID: ORD-001
   Customer: CUST-456
   Status: PAID
   Items: 2
   Version: 6
```
**💡 What happened:** Order state was **rebuilt** by replaying all 6 events from Kafka!

**9. Event History:**
```
📋 Event History:
   [1] OrderCreated @ 20:26:04
   [2] ItemAdded @ 20:26:04
   [3] ItemAdded @ 20:26:04
   [4] ShippingAddressSet @ 20:26:04
   [5] OrderSubmitted @ 20:26:04
   [6] PaymentReceived @ 20:26:04
```
**💡 What happened:** Shows complete audit trail of all events.

---

## 📋 Step 5: View Events in Kafka UI

**In Kafka UI (http://localhost:8080):**

1. **Refresh the Topics page**
   - You should now see `order-events` topic

2. **Click on `order-events` topic**

3. **Go to "Messages" tab**

4. **What You'll See:**
   - **6 messages** (one for each event)
   - **Key:** `ORD-001` (the aggregate ID)
   - **Value:** JSON serialized event
   - **Headers:** event-type, event-id, version

5. **Click on any message** to see:
   - Full event JSON
   - Event type (OrderCreated, ItemAdded, etc.)
   - Version number
   - Timestamp

**💡 Key Insight:** Notice that Kafka stores **events**, not the current state. The order state is rebuilt by replaying these events!

---

## 📋 Step 6: Run EventReplay - See State Rebuilding

**In a new terminal, run:**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture/examples/06-advanced-patterns/dotnet
dotnet run --project Examples/EventReplay.csproj
```

### **What You'll See:**

**1. Creating Order with Changes:**
```
📝 Creating order with multiple changes...
   Step 1: Created - Status: CREATED, Version: 1
   Step 2: Added item - Items: 1, Version: 2
   Step 3: Added another item - Items: 2, Version: 3
   Step 4: Removed item - Items: 1, Version: 4
   Step 5: Set address - Has Address: True, Version: 5
   Step 6: Submitted - Status: SUBMITTED, Version: 6
```

**2. Replaying Events:**
```
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

   Replaying: ItemAdded (v3)
      → Status: CREATED
      → Items: 2
      → Has Address: False

   Replaying: ItemRemoved (v4)
      → Status: CREATED
      → Items: 1
      → Has Address: False

   Replaying: ShippingAddressSet (v5)
      → Status: CREATED
      → Items: 1
      → Has Address: True

   Replaying: OrderSubmitted (v6)
      → Status: SUBMITTED
      → Items: 1
      → Has Address: True
```

**💡 Key Insight:** Watch how the state **transforms** with each event:
- Start: Empty order
- After v1: Order created
- After v2: 1 item
- After v3: 2 items
- After v4: Back to 1 item (removed one)
- After v5: Address added
- After v6: Order submitted

**This is event sourcing!** State is rebuilt by replaying events in order.

---

## 📋 Step 7: Run TimeTravel - See Historical State

**In a new terminal, run:**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture/examples/06-advanced-patterns/dotnet
dotnet run --project Examples/TimeTravel.csproj
```

### **What You'll See:**

**1. Creating Order Over Time:**
```
📝 Creating order with history...
   Order created with 5 events
```

**2. Time Travel Queries:**
```
⏰ Time Travel - Viewing order at different points in time:

📅 Point 1: 20:24:30 (just created)
   Status: CREATED
   Items: 1
   Version: 2

📅 Point 2: 20:24:31 (after adding PROD-B)
   Status: CREATED
   Items: 2
   Version: 3

📅 Point 3: 20:24:32 (after setting address)
   Status: CREATED
   Items: 2
   Has Address: True
   Version: 4

📅 Current: 20:24:32 (submitted)
   Status: SUBMITTED
   Items: 2
   Version: 5
```

**💡 Key Insight:** You can query the order state at **any point in time** by replaying events up to that timestamp!

---

## 🎯 Key Concepts Demonstrated

### **1. Event Sourcing Pattern**

**Traditional Approach:**
```
Database Table:
  Order: { id, status, items, address }
  
Update: UPDATE Order SET status = 'PAID' WHERE id = 'ORD-001'
```

**Event Sourcing Approach:**
```
Events:
  [1] OrderCreated
  [2] ItemAdded
  [3] ItemAdded
  [4] ShippingAddressSet
  [5] OrderSubmitted
  [6] PaymentReceived
  
Current State = Replay all events
```

### **2. State Rebuilding**

**How it works:**
1. Start with empty aggregate
2. Load all events from Kafka
3. Apply each event in order (by version)
4. Result: Current state

**Code:**
```csharp
var order = new Order(orderId);
var events = eventStore.GetEvents(orderId);
order.LoadFromHistory(events);  // Rebuild state
```

### **3. Immutable Events**

- Events are **never modified** once created
- Events are **append-only** in Kafka
- Complete audit trail

### **4. Version Tracking**

- Each event has a version number
- Prevents concurrent modifications (optimistic locking)
- Ensures events are applied in correct order

### **5. Time Travel**

- Query state at any timestamp
- Filter events by timestamp
- Replay events up to that point

---

## 🔍 What to Observe

### **In OrderService:**
1. ✅ Events are created for each state change
2. ✅ Events are stored in Kafka
3. ✅ State is rebuilt from events
4. ✅ Complete event history is available

### **In EventReplay:**
1. ✅ State transforms with each event
2. ✅ Events are applied in order
3. ✅ Final state matches what was saved

### **In TimeTravel:**
1. ✅ Can view state at any point in time
2. ✅ Events are filtered by timestamp
3. ✅ Historical state is rebuilt correctly

### **In Kafka UI:**
1. ✅ Events are stored in `order-events` topic
2. ✅ Each event has version, type, timestamp
3. ✅ Events are keyed by aggregate ID (Order ID)
4. ✅ Complete audit trail visible

---

## 🎓 Understanding the Flow

```
┌─────────────────────────────────────────────────────────┐
│ 1. Command Received (e.g., "Add Item")                  │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│ 2. Aggregate Validates & Creates Event                  │
│    Event: ItemAdded(ProductId, Quantity, Price)         │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│ 3. Event Applied to Aggregate (State Changes)           │
│    order.Items.Add(new Item(...))                       │
│    order.Version++                                      │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│ 4. Event Added to UncommittedEvents                     │
│    order.UncommittedEvents.Add(event)                   │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│ 5. Repository.Save() Called                             │
│    repository.Save(order)                               │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│ 6. Events Saved to Kafka                                │
│    Kafka Topic: order-events                            │
│    Key: Order ID                                        │
│    Value: JSON(event)                                   │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│ 7. Later: Load Order                                    │
│    repository.Get(orderId)                              │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│ 8. Load Events from Kafka                               │
│    events = eventStore.GetEvents(orderId)               │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│ 9. Replay Events to Rebuild State                       │
│    order.LoadFromHistory(events)                       │
│    → Apply each event in order                          │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│ 10. Return Order with Current State                     │
│     Order: { Status: PAID, Items: 2, ... }              │
└─────────────────────────────────────────────────────────┘
```

---

## ✅ Summary

**What You've Learned:**

1. ✅ **Event Sourcing** stores events, not current state
2. ✅ **State is rebuilt** by replaying events
3. ✅ **Kafka** serves as the event store
4. ✅ **Version tracking** prevents concurrency issues
5. ✅ **Time travel** queries are possible
6. ✅ **Complete audit trail** of all changes

**Key Takeaway:**
> In event sourcing, the **events are the source of truth**. Current state is derived by replaying events. This provides a complete audit trail and enables powerful features like time travel queries.

---

**Happy Event Sourcing! 🎉**

