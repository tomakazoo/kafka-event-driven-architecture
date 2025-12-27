using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SagaPattern.Orchestration;

class PersistenceExample
{
    static async Task Main()
    {
        Console.WriteLine("💾 Saga Pattern - State Persistence Example");
        Console.WriteLine();

        var repository = new InMemorySagaRepository();

        try
        {
            // Create a persistent saga
            Console.WriteLine("📝 Creating persistent saga...");
            var saga = new PersistentSaga("SAGA-PERSIST-001", repository);
            Console.WriteLine($"   Saga ID: SAGA-PERSIST-001");
            Console.WriteLine($"   Initial State: {saga.State}");
            Console.WriteLine();

            // Record steps
            Console.WriteLine("📋 Recording saga steps...");
            saga.RecordStep("InventoryReserved", new { ReservationId = "RES-1001" });
            saga.TransitionTo(SagaState.InventoryReserved);
            Console.WriteLine();

            saga.RecordStep("PaymentProcessed", new { PaymentId = "PAY-2001" });
            saga.TransitionTo(SagaState.PaymentProcessed);
            Console.WriteLine();

            saga.RecordStep("ShippingScheduled", new { TrackingNumber = "TRACK-3001" });
            saga.TransitionTo(SagaState.ShippingScheduled);
            Console.WriteLine();

            saga.TransitionTo(SagaState.Completed);
            Console.WriteLine();

            // Recover saga
            Console.WriteLine("🔄 Recovering saga from persistence...");
            var recoveredSaga = PersistentSaga.Recover("SAGA-PERSIST-001", repository);
            Console.WriteLine();

            Console.WriteLine("✅ Recovered Saga State:");
            Console.WriteLine($"   State: {recoveredSaga.State}");
            Console.WriteLine($"   Steps Completed: {string.Join(", ", recoveredSaga.StepsCompleted)}");
            Console.WriteLine($"   Compensation Data Keys: {string.Join(", ", recoveredSaga.CompensationData.Keys)}");
            Console.WriteLine();

            Console.WriteLine("✅ Persistence Demo Complete!");
            Console.WriteLine();
            Console.WriteLine("💡 Key Points:");
            Console.WriteLine("   • Saga state is persisted after each step");
            Console.WriteLine("   • Can recover saga state after crash");
            Console.WriteLine("   • Compensation data stored for rollback");
            Console.WriteLine("   • Enables saga recovery and continuation");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}

