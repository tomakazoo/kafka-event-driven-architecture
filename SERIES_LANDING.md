# Event-Driven Architecture & Apache Kafka – Series Landing Page

This repository accompanies the **Complete Event-Driven Architecture & Apache Kafka** 7‑part blog series. The series takes you from foundational concepts to building and operating production‑grade event‑driven systems with Apache Kafka.

> Online series home:  
> https://team-ai-lifehacks.vercel.app/blog/master-event-driven-architecture-kafka-series

---

## Series Overview

The series combines **theory**, **hands‑on implementation**, and **production operations**. You will design events, build real services, learn advanced patterns (CQRS, Event Sourcing, Sagas, Outbox/Inbox), and see how to run event‑driven systems reliably in production.[attached_file:1]

The code in this repository is organized to mirror the seven parts of the series and provide runnable examples and reference implementations.

---

## Parts 1–7

### Part 1 – Introduction to Event-Driven Architecture

**Blog post:**  
https://team-ai-lifehacks.vercel.app/blog/part-01-introduction-to-eda

**What this part covers (short):**

- Core EDA concepts: events, producers, brokers, consumers.  
- When event‑driven architecture fits (and when it does not).  
- Simple end‑to‑end examples to build intuition.[attached_file:1]

---

### Part 2 – Event Patterns and Design

**Blog post:**  
https://team-ai-lifehacks.vercel.app/blog/part-02-event-patterns-and-design

**What this part covers (short):**

- Key event patterns: notification, event‑carried state, event sourcing.  
- Practical event modeling, schema evolution, and common design pitfalls.  
- A workshop‑style event design for a ride‑sharing–style domain.[attached_file:1]

---

### Part 3 – Introduction to Apache Kafka

**Blog post:**  
https://team-ai-lifehacks.vercel.app/blog/part-03-introduction-to-apache-kafka

**What this part covers (short):**

- Kafka fundamentals: topics, partitions, offsets, consumer groups.  
- Why Kafka’s architecture enables high‑throughput, durable event streaming.  
- When Kafka is the right tool in an event‑driven system.[attached_file:1]

---

### Part 4 – Hands-On Kafka: Your First Event-Driven Application

**Blog post:**  
https://team-ai-lifehacks.vercel.app/blog/part-04-hands-on-kafka

**What this part covers (short):**

- Local Kafka cluster with Docker Compose (multi‑broker setup).  
- A complete event‑driven e‑commerce flow (order API, email, inventory, analytics).  
- Consumer groups, parallel processing, failure handling, and event replay.[attached_file:1]

---

### Part 5 – Advanced Kafka Concepts

**Blog post:**  
https://team-ai-lifehacks.vercel.app/blog/part-05-advanced-kafka-concepts

**What this part covers (short):**

- Exactly‑once semantics, idempotent producers, and log compaction.  
- Kafka Streams for real‑time processing and schema registry for data contracts.  
- Performance tuning, security (SSL/SASL/ACLs), and multi‑datacenter setups.[attached_file:1]

---

### Part 6 – Advanced Event-Driven Patterns

**Blog post:**  
https://team-ai-lifehacks.vercel.app/blog/part-06-advanced-event-driven-patterns

**What this part covers (short):**

- CQRS, Event Sourcing, and Sagas (choreography and orchestration).  
- Outbox and Inbox patterns for reliable messaging and idempotent consumers.  
- A cohesive system combining these patterns in an e‑commerce domain.[attached_file:1]

---

### Part 7 – Operating Event-Driven Systems in Production

**Blog post:**  
https://team-ai-lifehacks.vercel.app/blog/part-07-operating-event-driven-systems

**What this part covers (short):**

- Observability with Prometheus, Grafana, OpenTelemetry, and related tools.  
- Debugging issues like lost events, consumer lag, and stuck workflows.  
- Deployment, testing, incident response, and capacity planning practices.[attached_file:1]

---

## How to Use This Repository

- Use this file as the **entry point** for the series inside the codebase.  
- Start with Part 1 and follow the parts in order, or jump directly to the parts that match your current level (for example, Part 4 for hands‑on Kafka, Part 6 for patterns, Part 7 for operations).[attached_file:1]

For setup and commands to run the examples, see `README.md`.
