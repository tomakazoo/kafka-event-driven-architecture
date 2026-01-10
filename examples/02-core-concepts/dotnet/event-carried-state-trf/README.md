# Event-Carried State Transfer Pattern

This example demonstrates the **Event-Carried State Transfer (ECST)** pattern, where events contain all the data needed by consumers, making services fully autonomous.

## Overview

In Event-Carried State Transfer, producers publish events that include **all necessary data** for consumers to process the event without making additional API calls. This makes consumers fully autonomous and decoupled.

### Key Concepts

- **Full Data in Events**: Events contain denormalized, complete data
- **Autonomous Consumers**: Services don't need to call other services
- **Decoupling**: Services operate independently
- **Resilience**: If one service is down, others continue working
- **Scalability**: Services can scale independently

## Architecture

```
OrderService (Producer)
    ↓
Publishes event with FULL data:
  - Order details
  - Customer info (denormalized)
  - Line items (complete)
  - Shipping address
  - Payment info
    ↓
Kafka Topic: "orders"
    ↓
┌─────────────────┬─────────────────┬─────────────────┐
│ EmailService    │ InventoryService │ AnalyticsService │
│ (Autonomous)    │ (Autonomous)     │ (Autonomous)     │
│                 │                  │                  │
│ No API calls!   │ No API calls!    │ No API calls!    │
│ All data in     │ All data in      │ All data in      │
│ event           │ event            │ event            │
└─────────────────┴─────────────────┴─────────────────┘
```

## Event Structure

The event published by `OrderService` includes:

```json
{
  "eventType": "OrderPlaced",
  "eventId": "...",
  "eventVersion": "1.0",
  "timestamp": "...",
  "source": "order-service",
  "correlationId": "...",
  "data": {
    "orderId": "ORD-001",
    "customerId": "CUST-001",
    "customer": {
      "id": "CUST-001",
      "email": "john.doe@example.com",
      "name": "John Doe",
      "phone": "+1-555-0123"
    },
    "orderDate": "...",
    "totalAmount": 209.97,
    "currency": "USD",
    "status": "PLACED",
    "items": [
      {
        "productId": "PROD-001",
        "productName": "Wireless Mouse",
        "productSku": "WM-001",
        "quantity": 2,
        "unitPrice": 29.99,
        "totalPrice": 59.98,
        "imageUrl": "..."
      }
    ],
    "shippingAddress": {
      "street": "123 Main Street",
      "city": "San Francisco",
      "state": "CA",
      "postalCode": "94102",
      "country": "USA"
    },
    "payment": {
      "method": "credit_card",
      "last4": "4242",
      "status": "completed"
    }
  }
}
```

## Benefits

1. **Autonomy**: Consumers don't depend on other services being available
2. **Performance**: No network calls needed to fetch additional data
3. **Resilience**: Services continue working even if others are down
4. **Simplicity**: Consumers have everything they need in the event
5. **Scalability**: Services can process events independently

## Trade-offs

- **Event Size**: Events are larger (but usually acceptable)
- **Data Duplication**: Data is denormalized across events
- **Consistency**: Data in events may become stale over time
- **Storage**: More storage needed for larger events

## When to Use

✅ **Good for:**
- Read-heavy workloads
- Services that need to operate independently
- Scenarios where eventual consistency is acceptable
- High-throughput systems

❌ **Avoid when:**
- Events would be extremely large
- Strong consistency is required
- Data changes frequently and must be up-to-date

## Components

- **OrderService**: Producer that creates orders and publishes events
- **EmailService**: Consumer that sends order confirmation emails
- **InventoryService**: Consumer that updates inventory levels
- **AnalyticsService**: Consumer that records metrics

## See Also

- [HOW-TO-RUN.md](./HOW-TO-RUN.md) - Step-by-step instructions
- [Event Sourcing Pattern](../06-event-sourcing/dotnet/README.md) - Related pattern
- [Saga Pattern](../06-saga/dotnet/README.md) - Distributed transactions




