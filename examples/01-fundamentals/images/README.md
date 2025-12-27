# Screenshots for HOW-TO-RUN.md

This folder contains screenshots demonstrating successful execution of the Kafka fundamentals example.

## Required Images

### 1. `producer-success.png`
**Location in docs:** After Step 4 (Producer section)

**What to capture:**
- Terminal window showing successful producer execution
- Should show:
  - "🚀 Starting Kafka Producer..."
  - "✅ Producer created successfully"
  - "📨 Sending message X..." for each message
  - "✅ Delivered to my-topic [[0]] @X" for each message
  - "✅ All messages delivered successfully!"

**Example:** Terminal output with producer sending 5 messages successfully

---

### 2. `consumer-success.png`
**Location in docs:** After Step 5 (Consumer section)

**What to capture:**
- Terminal window showing successful consumer execution
- Should show:
  - "Received: {"id":0,"value":"Message 0"}" for all 5 messages
  - Consumer running and waiting for more messages

**Example:** Terminal output with consumer receiving messages

---

### 3. `kafka-ui-messages.png`
**Location in docs:** Step 7 - Messages View section

**What to capture:**
- Kafka UI browser window showing Messages tab
- Should show:
  - Topic: my-topic
  - List of messages with offsets, partitions, timestamps
  - Expanded message showing JSON value
  - Message details (Key, Value, Headers tabs)

**Example:** Browser screenshot of Kafka UI Messages view with messages visible

---

### 4. `kafka-ui-topics.png`
**Location in docs:** Step 7 - Topics View section

**What to capture:**
- Kafka UI browser window showing Topics list
- Should show:
  - my-topic in the list
  - Partitions: 1
  - Replication Factor: 1
  - Number of messages: 20
  - Size: 2 KB
  - Internal topics visible (__consumer_offsets, __schemas)

**Example:** Browser screenshot of Kafka UI Topics list

---

### 5. `kafka-ui-brokers.png`
**Location in docs:** Step 7 - Brokers View section

**What to capture:**
- Kafka UI browser window showing Brokers tab
- Should show:
  - Broker ID: 1 with green checkmark
  - Disk usage: 4.37 KB
  - Online partitions: 52 of 52
  - Leaders: 52
  - Port: 29092
  - Host: kafka

**Example:** Browser screenshot of Kafka UI Brokers view

---

### 6. `kafka-ui-consumers.png`
**Location in docs:** Step 7 - Consumers View section

**What to capture:**
- Kafka UI browser window showing Consumers tab
- Should show:
  - Consumer Group: dotnet-consumer-group
  - State: STABLE (green badge)
  - Num Of Members: 1
  - Num Of Topics: 1
  - Coordinator: 1

**Example:** Browser screenshot of Kafka UI Consumers view

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

**Note:** These images help users verify their setup is working correctly and understand what success looks like at each step.







