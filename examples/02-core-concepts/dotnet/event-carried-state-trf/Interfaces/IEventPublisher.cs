using System.Threading.Tasks;

namespace EventCarriedStateTransfer.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync(string topic, object eventData);
    void Publish(string topic, object eventData);
}

