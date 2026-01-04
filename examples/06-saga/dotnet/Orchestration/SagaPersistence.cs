using System;
using System.Collections.Generic;
using System.Linq;

namespace SagaPattern.Orchestration;

public enum SagaState
{
    Started,
    InventoryReserved,
    PaymentProcessed,
    ShippingScheduled,
    Completed,
    Failed,
    Compensating
}

public class SagaData
{
    public string State { get; set; } = string.Empty;
    public List<string> StepsCompleted { get; set; } = new();
    public Dictionary<string, object> CompensationData { get; set; } = new();
}

public interface ISagaRepository
{
    SagaData GetSaga(string sagaId);
    void UpdateSaga(string sagaId, SagaData data);
    void SaveSaga(string sagaId, SagaData data);
}

public class InMemorySagaRepository : ISagaRepository
{
    private readonly Dictionary<string, SagaData> _sagas = new();

    public SagaData GetSaga(string sagaId)
    {
        return _sagas.ContainsKey(sagaId) ? _sagas[sagaId] : null;
    }

    public void UpdateSaga(string sagaId, SagaData data)
    {
        if (_sagas.ContainsKey(sagaId))
        {
            _sagas[sagaId] = data;
        }
    }

    public void SaveSaga(string sagaId, SagaData data)
    {
        _sagas[sagaId] = data;
    }
}

public class PersistentSaga
{
    private readonly string _sagaId;
    private readonly ISagaRepository _repository;
    
    public SagaState State { get; private set; }
    public List<string> StepsCompleted { get; private set; }
    public Dictionary<string, object> CompensationData { get; private set; }

    public PersistentSaga(string sagaId, ISagaRepository repository)
    {
        _sagaId = sagaId;
        _repository = repository;
        State = SagaState.Started;
        StepsCompleted = new List<string>();
        CompensationData = new Dictionary<string, object>();
        
        // Save initial state
        SaveState();
    }

    public void TransitionTo(SagaState newState)
    {
        Console.WriteLine($"   📊 Saga {_sagaId}: State transition {State} → {newState}");
        
        // Persist state transition
        State = newState;
        SaveState();
    }

    public void RecordStep(string stepName, object data)
    {
        Console.WriteLine($"   📝 Saga {_sagaId}: Recording step '{stepName}'");
        
        // Record completed step
        StepsCompleted.Add(stepName);
        CompensationData[stepName] = data;
        SaveState();
    }

    private void SaveState()
    {
        _repository.SaveSaga(_sagaId, new SagaData
        {
            State = State.ToString(),
            StepsCompleted = StepsCompleted.ToList(),
            CompensationData = new Dictionary<string, object>(CompensationData)
        });
    }

    public static PersistentSaga Recover(string sagaId, ISagaRepository repository)
    {
        Console.WriteLine($"   🔄 Recovering saga {sagaId}...");
        
        // Recover saga from repository
        var sagaData = repository.GetSaga(sagaId);
        
        if (sagaData == null)
        {
            throw new InvalidOperationException($"Saga {sagaId} not found");
        }
        
        var saga = new PersistentSaga(sagaId, repository);
        saga.State = Enum.Parse<SagaState>(sagaData.State);
        saga.StepsCompleted = sagaData.StepsCompleted.ToList();
        saga.CompensationData = new Dictionary<string, object>(sagaData.CompensationData);
        
        Console.WriteLine($"   ✅ Saga {sagaId} recovered: State={saga.State}, Steps={saga.StepsCompleted.Count}");
        
        return saga;
    }
}




