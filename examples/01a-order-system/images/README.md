# Screenshots for Order System Example

This folder contains screenshots demonstrating the successful execution of the order system example.

## Required Images

### 1. `all-services-running.png`
**Location in docs:** After Step 7 (Observe Event Flow section)

**What to capture:**
- Four terminal windows showing all services running simultaneously:
  - Top-left: Order Service (producer creating orders)
  - Top-right: Inventory Service (consuming and reserving inventory)
  - Bottom-left: Customer Service (validating customers)
  - Bottom-right: Shipping Service (creating shipments)
- Should show successful order processing with green checkmarks
- Should show error handling (rejected orders, insufficient inventory)

**Example:** Grid layout of 4 terminals showing the complete event flow

---

### 2. `kafka-ui-topics.png`
**Location in docs:** Kafka UI - Topics Overview section

**What to capture:**
- Kafka UI browser window showing Topics list
- Should show:
  - All order system topics visible:
    - orders-created (3 partitions, 4 messages)
    - orders-validated (3 partitions, 3 messages)
    - orders-rejected (1 partition, 1 message)
    - inventory-reserved (3 partitions, 3 messages)
    - inventory-insufficient (1 partition, 1 message)
    - orders-shipped (3 partitions, 2 messages)
  - Internal topics visible (__consumer_offsets, __schemas)
  - Topic details: partitions, replication factor, message count, size

**Example:** Browser screenshot of Kafka UI Topics list with all order system topics

---

### 3. `kafka-ui-orders-created-messages.png`
**Location in docs:** Kafka UI - View Messages section

**What to capture:**
- Kafka UI browser window showing Messages view for `orders-created` topic
- Should show:
  - Message list with all 4 orders (ORD-001, ORD-002, ORD-003, ORD-004)
  - Expanded message showing full JSON structure
  - Order details visible:
    - OrderId, CustomerId
    - Items array with ProductId, ProductName, Quantity, Price
    - Total amount
    - Event metadata (EventId, Timestamp)
  - Messages distributed across partitions (0, 1, 2)
  - Timestamps for each order

**Example:** Browser screenshot of Kafka UI Messages view showing order JSON details

---

## Image Guidelines

- **Format:** PNG preferred (better quality for screenshots)
- **Size:** Full resolution (can be optimized later if needed)
- **Naming:** Use exact filenames as listed above
- **Content:** Capture the full relevant window/terminal
- **Quality:** Clear, readable text and UI elements

---

## Adding Images

1. Take screenshots using your preferred tool
2. Save them with the exact filenames listed above
3. Place them in this `images/` folder
4. The documentation will automatically reference them

---

**Note:** These images help users verify their setup is working correctly and understand what success looks like at each step of the order processing flow.

