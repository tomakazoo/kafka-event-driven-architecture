# How to Run Saga Pattern Examples

Complete step-by-step guide for running the Saga pattern examples (Choreography and Orchestration).

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
**If services are not ready after waiting for 30 seconds then you can run:**
```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/fix-kafka.sh
```
**Expected:** All Kafka services running and accessible.

---

### **Step 2: Build All Projects**

```bash
cd examples/06-saga/dotnet
dotnet build SagaPattern.sln
```

**Expected:** All projects build successfully.

---

### **Step 3: Run Choreography Example**

This demonstrates the **Choreography** pattern where services coordinate via events.

```bash
dotnet run --project Examples/ChoreographyExample.csproj
```

**What happens:**

1. **Order Service** creates and publishes `OrderPlacedEvent`
2. **Inventory Service** receives event, reserves inventory, publishes `InventoryReservedEvent`
3. **Payment Service** receives event, processes payment, publishes `PaymentReceivedEvent`
4. If payment fails, `PaymentFailedEvent` triggers inventory release (compensation)

**Expected output:**
```
🎭 Saga Pattern - Choreography Example
📡 Connecting to Kafka: localhost:9092

✅ Connected successfully

📝 Creating order...
📝 Order Service: Placing order ORD-001...
   ✅ Order ORD-001 saved
   📤 Published OrderPlacedEvent for order ORD-001

📦 Inventory Service: Listening for order events...
💳 Payment Service: Listening for order events...

⏳ Waiting for saga to complete...
   (Watch how services react to events)

📦 Inventory Service: Received OrderPlacedEvent for order ORD-001
   📉 Reserved 2x PROD-001. Remaining: 8
   📉 Reserved 1x PROD-002. Remaining: 4
   ✅ Inventory reserved for order ORD-001

💳 Payment Service: Received InventoryReservedEvent for order ORD-001
   💰 Charging customer for order ORD-001
   ✅ Payment processed for order ORD-001

✅ Choreography Saga Demo Complete!
```

**💡 Key Insight:** Services react to events autonomously. No central coordinator needed!

---

### **Step 4: Run Orchestration Example**

This demonstrates the **Orchestration** pattern with a central coordinator.

```bash
dotnet run --project Examples/OrchestrationExample.csproj
```

**What happens:**

1. **Saga Orchestrator** starts the saga
2. **Step 1:** Reserve inventory (stores compensation)
3. **Step 2:** Process payment (stores compensation)
4. **Step 3:** Schedule shipping (no compensation)
5. If any step fails, compensations run in reverse order

**Expected output:**
```
🎭 Saga Pattern - Orchestration Example
📡 Connecting to Kafka: localhost:9092

✅ Connected successfully

📝 Creating order request...
   Order ID: ORD-SAGA-001
   Customer: CUST-789
   Amount: $1059.97
   Items: 2

🎭 Saga ORD-SAGA-001: Starting execution...
   Step 1: Reserving inventory...
   ✅ Inventory reserved: RES-1000
   Step 2: Processing payment...
   ✅ Payment processed: PAY-2000
   Step 3: Scheduling shipping...
   ✅ Shipping scheduled: TRACK-3000
   🎉 Saga ORD-SAGA-001 completed successfully!

✅ Saga completed successfully!
   Tracking Number: TRACK-3000
```

**💡 Key Insight:** Central orchestrator coordinates all steps and manages compensations!

---

### **Step 5: Run Persistence Example**

This demonstrates **Saga State Persistence** for recovery.

```bash
dotnet run --project Examples/PersistenceExample.csproj
```

**What happens:**

1. Create a persistent saga
2. Record steps and state transitions
3. Recover saga from persistence
4. Show recovered state

**Expected output:**
```
💾 Saga Pattern - State Persistence Example

📝 Creating persistent saga...
   Saga ID: SAGA-PERSIST-001
   Initial State: Started

📋 Recording saga steps...
   📝 Saga SAGA-PERSIST-001: Recording step 'InventoryReserved'
   📊 Saga SAGA-PERSIST-001: State transition Started → InventoryReserved

   📝 Saga SAGA-PERSIST-001: Recording step 'PaymentProcessed'
   📊 Saga SAGA-PERSIST-001: State transition InventoryReserved → PaymentProcessed

   📝 Saga SAGA-PERSIST-001: Recording step 'ShippingScheduled'
   📊 Saga SAGA-PERSIST-001: State transition PaymentProcessed → ShippingScheduled

   📊 Saga SAGA-PERSIST-001: State transition ShippingScheduled → Completed

🔄 Recovering saga from persistence...
   🔄 Recovering saga SAGA-PERSIST-001...
   ✅ Saga SAGA-PERSIST-001 recovered: State=Completed, Steps=3

✅ Recovered Saga State:
   State: Completed
   Steps Completed: InventoryReserved, PaymentProcessed, ShippingScheduled
   Compensation Data Keys: InventoryReserved, PaymentProcessed, ShippingScheduled
```

**💡 Key Insight:** Saga state is persisted, enabling recovery after crashes!

---

## 🔍 Understanding the Patterns

### Choreography Pattern

**Flow:**
```
Order Service
    ↓ OrderPlacedEvent
Inventory Service
    ↓ InventoryReservedEvent
Payment Service
    ↓ PaymentReceivedEvent
Order Fulfilled
```

**Characteristics:**
- ✅ Decoupled services
- ✅ No single point of failure
- ✅ Scalable
- ❌ Harder to track overall flow
- ❌ Services need to know about all events

### Orchestration Pattern

**Flow:**
```
Saga Orchestrator
    ├─> Step 1: Reserve Inventory (store compensation)
    ├─> Step 2: Process Payment (store compensation)
    └─> Step 3: Schedule Shipping (no compensation)
```

**Characteristics:**
- ✅ Centralized control
- ✅ Easy to track flow
- ✅ Clear compensation logic
- ❌ Single point of failure
- ❌ Orchestrator needs to know all services

---

## 🎯 Key Concepts

### Compensating Transactions

Each step has a compensating transaction:

| Step | Compensation |
|------|-------------|
| Reserve Inventory | Release Inventory |
| Process Payment | Refund Payment |
| Schedule Shipping | None (can't un-ship) |

### Saga State Persistence

**Why persist?**
- Recover after crashes
- Continue interrupted sagas
- Track saga progress
- Enable manual intervention

**What to persist:**
- Current state
- Completed steps
- Compensation data
- Saga ID

---

## 🛠️ Troubleshooting

### Kafka not running?

```bash
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Build errors?

```bash
cd examples/06-saga/dotnet
dotnet clean
dotnet restore
dotnet build
```

### Events not flowing in Choreography?

1. Check consumers are subscribed: Look for "Listening for order events..."
2. Verify topic exists: `docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092`
3. Check Kafka UI: http://localhost:8080

### Orchestration not completing?

- Check mock services are working
- Verify all steps are executing
- Check for exceptions in output

---

## 📚 Further Reading

- [Saga Pattern](https://microservices.io/patterns/data/saga.html)
- [Choreography vs Orchestration](https://www.enterpriseintegrationpatterns.com/patterns/messaging/ProcessManager.html)
- [Distributed Transactions](https://martinfowler.com/articles/patterns-of-distributed-systems/two-phase-commit.html)

---

**Happy Saga Pattern Implementation! 🚀**




