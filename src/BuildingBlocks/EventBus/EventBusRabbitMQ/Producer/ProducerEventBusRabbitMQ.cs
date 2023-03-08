using Fibonacci.BuildingBlocks.EventBus;
using Fibonacci.BuildingBlocks.EventBusRabbitMQ.Connection;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Fibonacci.BuildingBlocks.EventBusRabbitMQ.Producer;
public class ProducerEventBusRabbitMQ : EventBusRabbitMQ, IProducerEventBus {
    protected readonly ILogger<ProducerEventBusRabbitMQ> _logger;
    private readonly EventBusConnectionSettings _eventBusConnectionSettings;
    private readonly DefaultObjectPool<IModel> _objectPool;
    private RetryPolicy _policy;
    private Guid eventId = Guid.Empty;

    public ProducerEventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, IPooledObjectPolicy<IModel> objectPolicy, ILogger<ProducerEventBusRabbitMQ> logger, IOptions<EventBusConnectionSettings> eventBusConnectionSettings) : base(persistentConnection) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _eventBusConnectionSettings = eventBusConnectionSettings.Value;
        _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
        _persistentConnection.TryConnect();
        _policy = Policy.Handle<BrokerUnreachableException>()
                    .Or<SocketException>()
                    .WaitAndRetry(_eventBusConnectionSettings.EventBusRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) => {
                        _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", eventId, $"{time.TotalSeconds:n1}", ex.Message);
                    });
    }

    public void Publish(IntegrationEvent @event) {
        var eventName = @event.GetType().Name;
        eventId = @event.Id;
        _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

        var channel = _objectPool.Get();
        try {
            _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
            channel.ExchangeDeclare(exchange: _eventBusConnectionSettings.BrokerName, type: _eventBusConnectionSettings.Type);

            var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions {
                WriteIndented = true
            });

            _policy.Execute(() => {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 1; // not persistent

                _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                channel.BasicPublish(exchange: _eventBusConnectionSettings.BrokerName, routingKey: string.Empty, mandatory: true, basicProperties: properties, body: body);
            });
        }
        catch (Exception) {
            throw;
        }
        finally {
            _objectPool.Return(channel);
        }

    }
}
