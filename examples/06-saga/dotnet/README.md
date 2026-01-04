# Saga Pattern with Kafka

Complete implementation of the Saga pattern using Apache Kafka, demonstrating both Choreography and Orchestration approaches.

## 🎯 What This Example Demonstrates

- **Saga Pattern**: Distributed transaction management across multiple services
- **Choreography**: Services coordinate via events (no central orchestrator)
- **Orchestration**: Central orchestrator coordinates all steps
- **Saga State Persistence**: Recover sagas after failures
- **Compensating Transactions**: Rollback mechanism for failed sagas

## 📋 Architecture

### Choreography Pattern
```
Order Service
    ↓ (OrderPlacedEvent)
Inventory Service → (InventoryReservedEvent)
    ↓
Payment Service → (PaymentReceivedEvent)
    ↓
Order Fulfilled
```

**Key Points:**
- Services react to events autonomously
- No central coordinator
- Each service knows what to do based on event type
- Compensating transactions handle failures

### Orchestration Pattern
```
Saga Orchestrator
    ↓
Step 1: Reserve Inventory
    ↓
Step 2: Process Payment
    ↓
Step 3: Schedule Shipping
    ↓
Order Fulfilled
```

**Key Points:**
- Central orchestrator coordinates all steps
- Compensations stored and executed in reverse order
- If any step fails, compensations run automatically

## 🚀 Quick Start

### Prerequisites

- Kafka running (see main project setup)
- .NET 8.0 SDK

### Step 1: Start Kafka

```bash
cd /home/babicto/projects/kafka-event-driven-architecture
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

### Step 2: Build Projects

```bash
cd examples/06-saga/dotnet
dotnet build SagaPattern.sln
```

### Step 3: Run Examples

**Choreography Example:**
```bash
dotnet run --project Examples/ChoreographyExample.csproj
```

**Orchestration Example:**
```bash
dotnet run --project Examples/OrchestrationExample.csproj
```

**Persistence Example:**
```bash
dotnet run --project Examples/PersistenceExample.csproj
```

## 📚 Concepts Explained

### Saga Pattern

A saga is a sequence of local transactions that together form a distributed transaction. If any step fails, compensating transactions undo previous steps.

**Example Flow:**
1. Reserve Inventory → If fails, nothing to undo ✅
2. Process Payment → If fails, release inventory 🔄
3. Schedule Shipping → If fails, refund payment + release inventory 🔄

### Choreography vs Orchestration

**Choreography:**
- Services publish events
- Other services react to events
- No central coordinator
- Decoupled and scalable
- Harder to track overall flow

**Orchestration:**
- Central orchestrator coordinates steps
- Easier to track and debug
- Single point of failure
- More control over flow

### Compensating Transactions

Each step has a compensating transaction that undoes its effects:

- **Reserve Inventory** → Compensate: Release Inventory
- **Process Payment** → Compensate: Refund Payment
- **Schedule Shipping** → No compensation (can't un-ship)

## 🏗️ Project Structure

```
dotnet/
├── Shared/
│   ├── Domain/          # Order, OrderItem, ShippingAddress
│   ├── Events/           # Saga events
│   └── Shared.csproj
├── Choreography/
│   ├── OrderService.cs   # Publishes OrderPlacedEvent
│   ├── InventoryService.cs  # Reacts to OrderPlacedEvent
│   ├── PaymentService.cs     # Reacts to InventoryReservedEvent
│   └── Choreography.csproj
├── Orchestration/
│   ├── SagaOrchestration.cs  # Central orchestrator
│   ├── SagaPersistence.cs    # State persistence
│   ├── MockServices.cs       # Mock implementations
│   └── Orchestration.csproj
├── Examples/
│   ├── ChoreographyExample.cs
│   ├── OrchestrationExample.cs
│   ├── PersistenceExample.cs
│   └── *.csproj
└── SagaPattern.sln
```

## 🔍 What to Observe

### Choreography Example

1. Order Service publishes `OrderPlacedEvent`
2. Inventory Service receives event and reserves inventory
3. Inventory Service publishes `InventoryReservedEvent`
4. Payment Service receives event and processes payment
5. Payment Service publishes `PaymentReceivedEvent`
6. If payment fails, `PaymentFailedEvent` triggers inventory release

### Orchestration Example

1. Orchestrator starts saga
2. Step 1: Reserve inventory (stores compensation)
3. Step 2: Process payment (stores compensation)
4. Step 3: Schedule shipping (no compensation)
5. If any step fails, compensations run in reverse order

### Persistence Example

1. Saga state persisted after each step
2. Can recover saga after crash
3. Compensation data stored for rollback
4. Enables saga recovery and continuation

## 🎓 Learning Objectives

✅ Understand Saga pattern for distributed transactions  
✅ Compare Choreography vs Orchestration  
✅ Implement compensating transactions  
✅ Handle saga failures gracefully  
✅ Persist saga state for recovery  

## 🛠️ Troubleshooting

**Kafka not running?**
```bash
./scripts/start-kafka.sh
./scripts/verify-docker.sh
```

**Build errors?**
```bash
dotnet clean
dotnet restore
dotnet build
```

**Events not flowing?**
- Check Kafka topic: `docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092`
- View in Kafka UI: http://localhost:8080

---

**Happy Saga Pattern Implementation! 🎉**




