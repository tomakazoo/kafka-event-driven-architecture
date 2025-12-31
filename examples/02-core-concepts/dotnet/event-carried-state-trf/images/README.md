# Images

This folder contains screenshots and diagrams for the Event-Carried State Transfer pattern example.

## Files

### `example-output.png`
**Location in docs:** After Step 3 - Expected Output section

**What to capture:**
- Terminal window showing successful execution of the example
- Should show:
  - "🚀 Event-Carried State Transfer Pattern Example"
  - "📡 Connecting to Kafka: localhost:9092"
  - "📋 Creating topic 'orders' if it doesn't exist..."
  - "✅ Topic 'orders' created"
  - "📝 Placing order..." with order details
  - "💾 Order ORD-001 saved to database"
  - "📤 Event published to topic 'orders'"
  - "📥 Event received by consumers:"
  - Email Service output with order confirmation email
  - Inventory Service output showing stock reduction
  - Analytics Service output showing metrics recorded
  - Success message and key benefits

**Example:** Terminal output showing the complete flow from order placement through autonomous service processing

---

### `kafka-ui-orders-message.png`
**Location in docs:** Step 4 - View Events in Kafka UI section

**What to capture:**
- Kafka UI browser window showing the "orders" topic messages
- Should show:
  - Left navigation: Topics → orders selected
  - Main content: "Topics / orders" header
  - Messages tab selected
  - Message list showing at least one message with:
    - Offset: 0
    - Partition: 0
    - Timestamp visible
  - Message details panel showing:
    - Key Preview: UUID (e.g., "942ac704-24de-4689-81be-f4a68b8355ce")
    - Value tab selected showing full JSON event
    - JSON should display the OrderPlaced event with:
      - `eventType: "OrderPlaced"`
      - `eventId`, `eventVersion`, `timestamp`, `source`, `correlationId`
      - `data` object containing:
        - `orderId`, `customerId`
        - `customer` object with `id`, `email`, `name`, `phone`
        - `orderDate`, `totalAmount`, `currency`, `status`
        - `items` array with product details
        - `shippingAddress` object
        - `payment` object
  - Right panel showing message metadata (timestamp, serde info, size)

**Example:** Browser screenshot of Kafka UI showing the full OrderPlaced event message with all event-carried state transfer data visible

## Image Guidelines

- **Format:** PNG preferred (better quality for screenshots)
- **Size:** Full resolution (can be optimized later if needed)
- **Naming:** Use exact filename as listed above
- **Content:** Capture the full relevant terminal window
- **Quality:** Clear, readable text and output

---

## Adding Images

1. Take screenshot using your preferred tool
2. Save it with the exact filename listed above
3. Place it in this `images/` folder
4. The documentation will automatically reference it

---

**Note:** This image helps users verify their setup is working correctly and understand what successful execution looks like.
