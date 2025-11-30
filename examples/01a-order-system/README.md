# Order System Example

A complete event-driven order processing system demonstrating how multiple microservices communicate through Kafka events.

## 🎯 Overview

This example implements a simplified e-commerce order system with four microservices:

1. **Order Service** - Creates orders and publishes `orders-created` events
2. **Customer Service** - Validates customers and publishes `orders-validated` or `orders-rejected` events
3. **Inventory Service** - Reserves inventory and publishes `inventory-reserved` or `inventory-insufficient` events
4. **Shipping Service** - Creates shipments when orders are validated AND inventory is reserved, publishes `orders-shipped` events

## 🔄 Event Flow

```
Order Created
    ↓
orders-created topic
    ↓
┌─────────────────────────────────────┐
│  Customer Service (validates)       │ → orders-validated / orders-rejected
│  Inventory Service (reserves stock) │ → inventory-reserved / inventory-insufficient
└─────────────────────────────────────┘
    ↓
Shipping Service (waits for both)
    ↓
orders-shipped topic
```

## 📋 Topics

**Note:** Topics are **auto-created** when producers first publish to them (configured in `docker-compose.yml`). You can also create them manually if you need specific partition/replication settings.

- `orders-created` - New orders created
- `orders-validated` - Orders with valid customers
- `orders-rejected` - Orders with invalid/inactive customers
- `inventory-reserved` - Orders with reserved inventory
- `inventory-insufficient` - Orders that couldn't reserve inventory
- `orders-shipped` - Orders that have been shipped

**Default settings:** Auto-created topics use 1 partition and replication factor 1. See [HOW-TO-RUN.md](HOW-TO-RUN.md) for manual topic creation with custom settings.

## 🏗️ Architecture

### Services

**Order Service (Producer)**
- Creates sample orders
- Publishes to `orders-created` topic
- Demonstrates basic event production

**Customer Service (Consumer → Producer)**
- Consumes `orders-created` events
- Validates customer exists and is active
- Publishes validation results

**Inventory Service (Consumer → Producer)**
- Consumes `orders-created` events
- Checks product availability
- Reserves inventory
- Publishes reservation results

**Shipping Service (Consumer → Producer)**
- Consumes `orders-validated` and `inventory-reserved` events
- Waits for both conditions to be met
- Creates shipping label
- Publishes `orders-shipped` events

## 📦 Models

Simple POCOs for:
- **Order** - Order details with items
- **Customer** - Customer information
- **Product** - Product inventory
- **Shipping** - Shipping details

## 🚀 Quick Start

1. **Start Kafka:**
   ```bash
   cd /home/babicto/projects/kafka-event-driven-architecture
   ./scripts/start-kafka.sh
   ```

2. **Create Topics** (Recommended):
   ```bash
   cd examples/01a-order-system/dotnet
   ./create-topics.sh
   ```
   
   **Note:** Consumers need topics to exist before subscribing. This script creates all required topics.

3. **Start Services** (in separate terminals):
   ```bash
   cd examples/01a-order-system/dotnet
   
   # Terminal 1: Customer Service
   dotnet run --project CustomerService/CustomerService.csproj
   
   # Terminal 2: Inventory Service
   dotnet run --project InventoryService/InventoryService.csproj
   
   # Terminal 3: Shipping Service
   dotnet run --project ShippingService/ShippingService.csproj
   
   # Terminal 4: Order Service (creates orders)
   dotnet run --project OrderService/OrderService.csproj
   ```

3. **Watch the event flow** across all terminals!

## 📚 Learning Objectives

- ✅ Multiple producers and consumers
- ✅ Event-driven workflows
- ✅ Service decoupling
- ✅ Topic-based routing
- ✅ Error handling and rejection flows
- ✅ Event dependencies (Shipping waits for multiple events)

## 🔍 What to Observe

- **Order Service** creates orders and publishes events
- **Customer Service** validates customers (some orders will be rejected)
- **Inventory Service** reserves inventory (some orders will fail due to insufficient stock)
- **Shipping Service** only ships orders that are BOTH validated AND have reserved inventory

## 📖 Detailed Guide

See [HOW-TO-RUN.md](HOW-TO-RUN.md) for step-by-step instructions.

