using Fibonacci.BuildingBlocks.EventBusRabbitMQ.Connection;
using Microsoft.Extensions.ObjectPool;

namespace Fibonacci.BuildingBlocks.EventBusRabbitMQ;
public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel> {
    private readonly IRabbitMQPersistentConnection _rabbitMQPersistentConnection;

    public RabbitModelPooledObjectPolicy(IRabbitMQPersistentConnection rabbitMQPersistentConnection) {
        _rabbitMQPersistentConnection = rabbitMQPersistentConnection;

    }
    public IModel Create() => _rabbitMQPersistentConnection.CreateModel();

    public bool Return(IModel obj) {
        if (obj.IsOpen)
            return true;
        else {
            obj?.Dispose();
            return false;
        }
    }
}