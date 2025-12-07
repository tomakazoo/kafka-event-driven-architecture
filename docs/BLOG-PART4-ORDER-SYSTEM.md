# Part 4 Blog Content: Building a Complete Order Processing System

**Location in blog:** Replace/enhance the generic "e-commerce flow" description with this complete walkthrough

**Word count:** ~1500 words + 5-6 screenshots

---

## Building a Complete Order Processing System

In Part 1, we built a simple producer-consumer example. Now let's build something real: a complete e-commerce order processing system with multiple microservices communicating through events.

This example demonstrates real-world Event-Driven Architecture patterns you'll use in production.

### System Architecture

We'll build four microservices that work together to process orders:

```
┌─────────────────────────────────────────────────────────┐
│                    Order Service                        │
│              (Event Producer)                           │
│  Creates orders → publishes "orders-created" events     │
└──────────────────────┬──────────────────────────────────┘
                       │
                       ↓
              ┌─────────────────┐
              │  Kafka Broker   │
              │ orders-created  │
              └────────┬─────────┘
                       │
        ┌───────────────┼───────────────┐
        ↓                               ↓
┌───────────────┐            ┌──────────────────┐
│   Customer    │            │    Inventory     │
│   Service     │            │     Service      │
│ (Consumer →   │            │  (Consumer →     │
│  Producer)    │            │   Producer)      │
│               │            │                  │
│ Validates     │            │ Reserves stock   │
│ customers     │            │                  │
│               │            │                  │
│ → orders-     │            │ → inventory-     │
│   validated   │            │   reserved       │
│ → orders-     │            │ → inventory-     │
│   rejected    │            │   insufficient   │
└───────┬───────┘            └────────┬─────────┘
        │                             │
        └───────────────┬─────────────┘
                        ↓
              ┌──────────────────┐
              │  Kafka Broker    │
              │ orders-validated │
              │ inventory-       │
              │ reserved         │
              └────────┬─────────┘
                       │
                       ↓
              ┌─────────────────┐
              │   Shipping      │
              │   Service       │
              │ (Consumer →     │
              │  Producer)      │
              │                 │
              │ Waits for BOTH  │
              │ validation AND  │
              │ inventory       │
              │                 │
              │ → orders-       │
              │   shipped       │
              └─────────────────┘
```

**Event Flow:**

1. **Order Service** creates an order → publishes `orders-created` event
2. **Customer Service** validates customer → publishes `orders-validated` or `orders-rejected`
3. **Inventory Service** reserves stock → publishes `inventory-reserved` or `inventory-insufficient`
4. **Shipping Service** waits for both validation AND inventory → publishes `orders-shipped`

**Key Pattern:** Shipping Service demonstrates **event dependencies** - it waits for multiple events before taking action.

### Topics Overview

Our system uses six topics:

- `orders-created` - New orders (3 partitions)
- `orders-validated` - Validated orders (3 partitions)
- `orders-rejected` - Rejected orders (1 partition)
- `inventory-reserved` - Reserved inventory (3 partitions)
- `inventory-insufficient` - Failed reservations (1 partition)
- `orders-shipped` - Shipped orders (3 partitions)

### Prerequisites

- Kafka running (from Part 1 setup)
- .NET 8.0 SDK installed
- 4 terminal windows available

### Step 1: Start Kafka and Create Topics

```bash
cd kafka-event-driven-architecture

# Start Kafka (if not already running)
./scripts/start-kafka.sh

# Create topics with proper partitions
cd examples/01a-order-system/dotnet
./create-topics.sh
```

**Why create topics manually?**

While Kafka auto-creates topics, manually creating them lets us:
- Set appropriate partition counts (3 for main topics)
- Configure replication factors
- Ensure topics exist before consumers start

### Step 2: Understanding the Services

Let's examine each service:

#### Order Service (Producer)

**Role:** Creates orders and publishes events

```csharp
// Simplified version
var order = new OrderCreatedEvent
{
    OrderId = "ORD-001",
    CustomerId = "CUST-001",
    Products = new List<ProductItem>
    {
        new ProductItem { ProductId = "PROD-001", Quantity = 1, Price = 99.99m }
    },
    TotalAmount = 99.99m
};

await producer.ProduceAsync("orders-created", 
    new Message<string, string> 
    { 
        Key = order.OrderId, 
        Value = JsonSerializer.Serialize(order) 
    });
```

**What it does:**
- Creates sample orders
- Publishes to `orders-created` topic
- Doesn't know about other services

#### Customer Service (Consumer → Producer)

**Role:** Validates customers and publishes results

```csharp
// Consumes orders-created
var orderCreated = Deserialize<OrderCreatedEvent>(message.Value);

// Validates customer
var isValid = ValidateCustomer(orderCreated.CustomerId);

// Publishes result
var event = new OrderValidatedEvent
{
    OrderId = orderCreated.OrderId,
    IsValid = isValid,
    CustomerName = isValid ? GetCustomerName(orderCreated.CustomerId) : null
};

await producer.ProduceAsync(
    isValid ? "orders-validated" : "orders-rejected",
    new Message<string, string> { Key = event.OrderId, Value = Serialize(event) });
```

**What it does:**
- Consumes `orders-created` events
- Validates customer exists and is active
- Publishes to `orders-validated` (success) or `orders-rejected` (failure)
- **Pattern:** Consumer that also produces events

#### Inventory Service (Consumer → Producer)

**Role:** Reserves inventory and publishes results

```csharp
// Consumes orders-created
var orderCreated = Deserialize<OrderCreatedEvent>(message.Value);

// Reserves inventory
var reservation = ReserveInventory(orderCreated.Products);

// Publishes result
var event = new InventoryReservedEvent
{
    OrderId = orderCreated.OrderId,
    IsReserved = reservation.Success,
    ReservedItems = reservation.Items
};

await producer.ProduceAsync(
    reservation.Success ? "inventory-reserved" : "inventory-insufficient",
    new Message<string, string> { Key = event.OrderId, Value = Serialize(event) });
```

**What it does:**
- Consumes `orders-created` events
- Checks product availability
- Reserves inventory if available
- Publishes to `inventory-reserved` (success) or `inventory-insufficient` (failure)

#### Shipping Service (Consumer → Producer)

**Role:** Creates shipments when both conditions are met

```csharp
// Consumes BOTH orders-validated AND inventory-reserved
var topic = message.Topic;

if (topic == "orders-validated")
{
    var validated = Deserialize<OrderValidatedEvent>(message.Value);
    _validatedOrders[validated.OrderId] = validated;
}
else if (topic == "inventory-reserved")
{
    var reserved = Deserialize<InventoryReservedEvent>(message.Value);
    _reservedInventories[reserved.OrderId] = reserved;
}

// Check if BOTH events received
if (_validatedOrders.ContainsKey(orderId) && 
    _reservedInventories.ContainsKey(orderId))
{
    // Create shipment
    var shipped = new OrderShippedEvent
    {
        OrderId = orderId,
        TrackingNumber = GenerateTrackingNumber()
    };
    
    await producer.ProduceAsync("orders-shipped", 
        new Message<string, string> { Key = shipped.OrderId, Value = Serialize(shipped) });
}
```

**What it does:**
- Subscribes to **multiple topics** (`orders-validated` and `inventory-reserved`)
- Tracks orders waiting for both events
- Only ships when **both** validation AND inventory are confirmed
- **Pattern:** Event dependency handling

### Step 3: Running the System

Start each service in a separate terminal:

**Terminal 1 - Customer Service:**
```bash
cd examples/01a-order-system/dotnet
dotnet run --project CustomerService/CustomerService.csproj
```

**Terminal 2 - Inventory Service:**
```bash
cd examples/01a-order-system/dotnet
dotnet run --project InventoryService/InventoryService.csproj
```

**Terminal 3 - Shipping Service:**
```bash
cd examples/01a-order-system/dotnet
dotnet run --project ShippingService/ShippingService.csproj
```

**Terminal 4 - Order Service (creates orders):**
```bash
cd examples/01a-order-system/dotnet
dotnet run --project OrderService/OrderService.csproj
```

![All Services Running](images/all-services-running.png)
*All 4 microservices running simultaneously - Order Service, Customer Service, Inventory Service, and Shipping Service processing events*

### Step 4: Observing the Event Flow

Watch the terminals to see events flow through the system:

**Order Service creates 4 orders:**
- ORD-001: Valid customer, valid inventory → Will be shipped ✅
- ORD-002: Valid customer, valid inventory → Will be shipped ✅
- ORD-003: Invalid customer → Will be rejected ❌
- ORD-004: Valid customer, insufficient inventory → Will fail ❌

**Customer Service validates:**
```
📨 Received Order: ORD-001
✅ Order ORD-001 validated - Customer: John Doe
   Published to orders-validated

📨 Received Order: ORD-003
❌ Order ORD-003 rejected - Customer CUST-999 not found
   Published to orders-rejected
```

**Inventory Service reserves:**
```
📨 Received Order: ORD-001
✅ Order ORD-001 - Inventory reserved
   Published to inventory-reserved

📨 Received Order: ORD-004
❌ Order ORD-004 - Insufficient inventory
   Published to inventory-insufficient
```

**Shipping Service ships:**
```
📨 Received Order Validation: ORD-001
📨 Received Inventory Reservation: ORD-001
🚚 Order ORD-001 shipped! Tracking: TRACK-1000
   Published to orders-shipped
```

### Understanding the Flow

#### Successful Order Flow (ORD-001, ORD-002)

1. Order Service → `orders-created`
2. Customer Service → `orders-validated`
3. Inventory Service → `inventory-reserved`
4. Shipping Service → Receives both → `orders-shipped` ✅

#### Rejected Order Flow (ORD-003)

1. Order Service → `orders-created`
2. Customer Service → `orders-rejected` ❌
3. Inventory Service → Still reserves (doesn't know about rejection)
4. Shipping Service → Never ships (missing validation)

**Key Insight:** Services are decoupled. Inventory Service doesn't know about customer validation failures.

#### Failed Order Flow (ORD-004)

1. Order Service → `orders-created`
2. Customer Service → `orders-validated` ✅
3. Inventory Service → `inventory-insufficient` ❌
4. Shipping Service → Never ships (missing inventory reservation)

**Key Insight:** Shipping Service waits for **both** conditions. If either fails, no shipment.

### Viewing Events in Kafka UI

Open **http://localhost:8080** to see the complete event flow.

#### Topics Overview

Navigate to: **Topics**

![Kafka UI Topics](images/kafka-ui-topics.png)
*Kafka UI showing all order system topics with message counts, partitions, and sizes*

**What you'll see:**
- `orders-created` - 4 messages (all orders)
- `orders-validated` - 3 messages (ORD-001, ORD-002, ORD-004)
- `orders-rejected` - 1 message (ORD-003)
- `inventory-reserved` - 3 messages (ORD-001, ORD-002, ORD-003)
- `inventory-insufficient` - 1 message (ORD-004)
- `orders-shipped` - 2 messages (ORD-001, ORD-002)

#### Messages View

Navigate to: **Topics** → **orders-created** → **Messages**

![Kafka UI Orders Created Messages](images/kafka-ui-orders-created-messages.png)
*Viewing messages in orders-created topic - showing order details with JSON structure*

**What you'll see:**
- Full JSON structure of each order
- Order ID, Customer ID, Products, Total
- Event metadata (EventId, Timestamp)
- Messages distributed across partitions

**Try exploring:**
- `orders-validated` - See validated orders with customer names
- `orders-rejected` - See rejection reasons
- `inventory-reserved` - See reserved items
- `orders-shipped` - See tracking numbers

### Key Patterns Demonstrated

This example demonstrates several important EDA patterns:

#### 1. **Event-Driven Workflow**

Services communicate through events, not direct calls. Each service reacts to events independently.

#### 2. **Service Decoupling**

- Order Service doesn't know about Customer Service
- Customer Service doesn't know about Inventory Service
- Services only know about Kafka topics

#### 3. **Error Handling**

- Invalid customers → `orders-rejected` topic
- Insufficient inventory → `inventory-insufficient` topic
- Services handle errors gracefully without affecting others

#### 4. **Event Dependencies**

Shipping Service demonstrates waiting for multiple events:
- Subscribes to multiple topics
- Tracks state until both conditions met
- Only then publishes `orders-shipped`

#### 5. **Consumer → Producer Pattern**

Customer, Inventory, and Shipping Services all:
- Consume events from one topic
- Process them
- Produce events to other topics

This is a common pattern in event-driven systems.

#### 6. **Topic-Based Routing**

Different event types go to different topics:
- Success → `orders-validated`, `inventory-reserved`
- Failure → `orders-rejected`, `inventory-insufficient`

Consumers subscribe to relevant topics.

### Production Considerations

While this example is simplified, here are production considerations:

**Schema Evolution:**
- Use Schema Registry for event schemas
- Version events for backward compatibility
- Handle schema evolution gracefully

**Error Handling:**
- Implement retry logic with exponential backoff
- Use dead-letter queues for failed messages
- Monitor consumer lag and errors

**Idempotency:**
- Ensure operations can be safely retried
- Use event IDs for deduplication
- Handle duplicate events gracefully

**Monitoring:**
- Track consumer lag
- Monitor event throughput
- Alert on failures
- Use distributed tracing

**Scaling:**
- Scale consumers horizontally (add more instances)
- Use consumer groups for load distribution
- Partition topics appropriately

### What You've Built

Congratulations! You've built a complete event-driven microservices system that:

✅ Processes orders through multiple services  
✅ Handles errors gracefully  
✅ Demonstrates event dependencies  
✅ Shows real-world EDA patterns  
✅ Scales independently  

This is the foundation for production event-driven systems.

### Next Steps

- **Part 5:** Advanced Kafka concepts (exactly-once semantics, Kafka Streams, performance tuning)
- **Part 6:** Advanced patterns (CQRS, Event Sourcing, Sagas, Outbox/Inbox patterns)
- **Part 7:** Operating in production (monitoring, debugging, deployment)

**Full code and detailed guide:**  
[GitHub Repository](https://github.com/your-username/kafka-event-driven-architecture) → `examples/01a-order-system/`

**Complete walkthrough:**  
[HOW-TO-RUN.md](https://github.com/your-username/kafka-event-driven-architecture/blob/main/examples/01a-order-system/HOW-TO-RUN.md)

---

**Key Takeaway:** Event-Driven Architecture enables building complex, scalable systems where services communicate through events. This order system demonstrates real-world patterns: decoupling, error handling, event dependencies, and consumer-producer patterns. In the next parts, we'll explore advanced Kafka features and sophisticated EDA patterns used in production systems.


