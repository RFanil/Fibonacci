using Fibonacci.BuildingBlocks.EventBusRabbitMQ.Connection;

namespace Fibonacci.BuildingBlocks.EventBusRabbitMQ;

public class EventBusRabbitMQ
{
    protected readonly IRabbitMQPersistentConnection _persistentConnection;

    public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection) {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
    }
}
