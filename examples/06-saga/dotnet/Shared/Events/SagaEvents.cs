using System;
using System.Collections.Generic;
using SagaPattern.Shared.Domain;

namespace SagaPattern.Shared.Events;

// Choreography Events
public record OrderPlacedEvent(Order Order);

public record InventoryReservedEvent(string OrderId);

public record InventoryReservationFailedEvent(string OrderId);

public record PaymentReceivedEvent(string OrderId);

public record PaymentFailedEvent(string OrderId);

// Orchestration Events
public record OrderFulfilledEvent(string OrderId);

public record OrderFailedEvent(string OrderId, string Reason);





