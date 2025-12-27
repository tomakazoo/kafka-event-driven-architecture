# Event Sourcing with Kafka

A complete event sourcing implementation using Apache Kafka as the event store.

## 🎯 What This Example Demonstrates

- **Event Sourcing Pattern**: Store state changes as a sequence of events
- **Kafka as Event Store**: Use Kafka topics to persist domain events
- **Aggregate Pattern**: Order aggregate with event-driven state changes
- **Event Replay**: Rebuild aggregate state by replaying events
- **Time Travel**: View aggregate state at any point in time
- **Optimistic Locking**: Version-based concurrency control
- **Snapshots**: Performance optimization for large event streams

## 📋 Architecture

```
Order Aggregate
    ↓ (Commands)
Domain Events
    ↓
Kafka Event Store (order-events topic)
    ↓
Event Replay → Rebuild State
```

### Key Components

1. **Domain Events**: Immutable events representing state changes
2. **Order Aggregate**: Business logic and state management
3. **KafkaEventStore**: Kafka-backed event store
4. **OrderRepository**: Repository pattern with event sourcing
5. **Example Programs**: Demonstrations of event sourcing capabilities

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
cd examples/06-advanced-patterns/dotnet
dotnet build EventSourcing.sln
```

### Step 3: Run Examples

**Order Service** - Basic event sourcing:
```bash
dotnet run --project Examples/OrderService.csproj
```

**Event Replay** - Rebuild state from events:
```bash
dotnet run --project Examples/EventReplay.csproj
```

**Time Travel** - View state at different points in time:
```bash
dotnet run --project Examples/TimeTravel.csproj
```

## 📚 Concepts Explained

### Event Sourcing

Instead of storing current state, we store all events that led to that state:

```
Traditional:
  Order Table: { id, status, items, ... }

Event Sourcing:
  Events: [
    OrderCreated,
    ItemAdded,
    ItemAdded,
    ShippingAddressSet,
    OrderSubmitted,
    PaymentReceived,
    OrderShipped
  ]
```

### Rebuilding State

To get current state, replay all events:

```csharp
var order = new Order(orderId);
var events = eventStore.GetEvents(orderId);
order.LoadFromHistory(events);
// Order now has current state
```

### Time Travel

View state at any point in time:

```csharp
var orderYesterday = repository.GetOrderAtTimestamp(
    orderId, 
    DateTime.UtcNow.AddDays(-1)
);
```

### Optimistic Locking

Prevent concurrent modifications:

```csharp
// Save with expected version
eventStore.SaveEvents(aggregateId, events, expectedVersion: 5);

// If version changed, throws ConcurrencyException
```

## 🔍 Example Output

### Order Service

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
```

## 📖 Domain Events

- **OrderCreated**: Order aggregate created
- **ItemAdded**: Item added to order
- **ItemRemoved**: Item removed from order
- **ShippingAddressSet**: Shipping address configured
- **OrderSubmitted**: Order submitted for processing
- **PaymentReceived**: Payment recorded
- **OrderShipped**: Order shipped with tracking

## 🎓 Learning Objectives

✅ Understand event sourcing pattern  
✅ Use Kafka as an event store  
✅ Implement aggregate pattern with events  
✅ Rebuild state from events  
✅ Implement time travel queries  
✅ Handle concurrency with optimistic locking  
✅ Use snapshots for performance  

## 🏗️ Project Structure

```
dotnet/
├── Domain/
│   ├── DomainEvents.cs    # Event definitions
│   ├── Order.cs           # Aggregate root
│   └── Domain.csproj
├── EventStore/
│   ├── KafkaEventStore.cs # Kafka-backed event store
│   └── EventStore.csproj
├── Repository/
│   ├── OrderRepository.cs # Repository with event sourcing
│   └── Repository.csproj
├── Examples/
│   ├── OrderService.cs    # Basic example
│   ├── EventReplay.cs     # Replay demonstration
│   ├── TimeTravel.cs      # Time travel demo
│   └── *.csproj
└── EventSourcing.sln
```

## 🔧 Kafka Topic

Events are stored in: `order-events`

**Message Format:**
- **Key**: Aggregate ID (Order ID)
- **Value**: JSON serialized domain event
- **Headers**: event-type, event-id, version

## 💡 Key Benefits

1. **Complete Audit Trail**: Every change is recorded
2. **Time Travel**: View state at any point in time
3. **Event Replay**: Rebuild state from scratch
4. **Scalability**: Kafka handles high throughput
5. **Durability**: Events persisted in Kafka
6. **Decoupling**: Events can be consumed by multiple services

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

**Events not appearing?**
- Check Kafka topic exists: `docker compose exec kafka kafka-topics --list --bootstrap-server localhost:9092`
- View messages: Use Kafka UI at http://localhost:8080

## 📚 Further Reading

- [Event Sourcing Pattern](https://martinfowler.com/eaaDev/EventSourcing.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)

---

**Happy Event Sourcing! 🎉**

