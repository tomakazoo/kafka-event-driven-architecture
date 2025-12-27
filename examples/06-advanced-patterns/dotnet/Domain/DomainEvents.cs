using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EventSourcing.Domain;

// Base Event
public abstract record DomainEvent(
    string EventId,
    string AggregateId,
    string EventType,
    DateTime Timestamp,
    int Version
);

// Order Events
public record OrderCreated(
    string EventId,
    string AggregateId,
    string EventType,
    DateTime Timestamp,
    int Version,
    string CustomerId
) : DomainEvent(EventId, AggregateId, EventType, Timestamp, Version);

public record ItemAdded(
    string EventId,
    string AggregateId,
    string EventType,
    DateTime Timestamp,
    int Version,
    string ProductId,
    int Quantity,
    decimal Price
) : DomainEvent(EventId, AggregateId, EventType, Timestamp, Version);

public record ItemRemoved(
    string EventId,
    string AggregateId,
    string EventType,
    DateTime Timestamp,
    int Version,
    string ProductId
) : DomainEvent(EventId, AggregateId, EventType, Timestamp, Version);

public record ShippingAddressSet(
    string EventId,
    string AggregateId,
    string EventType,
    DateTime Timestamp,
    int Version,
    Dictionary<string, string> Address
) : DomainEvent(EventId, AggregateId, EventType, Timestamp, Version);

public record OrderSubmitted(
    string EventId,
    string AggregateId,
    string EventType,
    DateTime Timestamp,
    int Version
) : DomainEvent(EventId, AggregateId, EventType, Timestamp, Version);

public record PaymentReceived(
    string EventId,
    string AggregateId,
    string EventType,
    DateTime Timestamp,
    int Version,
    string PaymentId,
    decimal Amount
) : DomainEvent(EventId, AggregateId, EventType, Timestamp, Version);

public record OrderShipped(
    string EventId,
    string AggregateId,
    string EventType,
    DateTime Timestamp,
    int Version,
    string TrackingNumber
) : DomainEvent(EventId, AggregateId, EventType, Timestamp, Version);

