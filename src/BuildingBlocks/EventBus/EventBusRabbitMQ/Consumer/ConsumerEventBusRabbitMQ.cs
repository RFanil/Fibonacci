using Fibonacci.BuildingBlocks.EventBus;
using Fibonacci.BuildingBlocks.EventBusRabbitMQ.Connection;
using Microsoft.Extensions.Options;
using System.Text;

namespace Fibonacci.BuildingBlocks.EventBusRabbitMQ.Consumer;

public class ConsumerEventBusRabbitMQ<TEventType> : EventBusRabbitMQ, IDisposable where TEventType : IntegrationEvent {

    private IModel _consumerChannel;
    private readonly IIntegrationEventHandler<TEventType> _handler;
    protected readonly ILogger<ConsumerEventBusRabbitMQ<TEventType>> _logger;
    private readonly EventBusChannelSettings _eventBusChannelSettings;
    private readonly EventBusConnectionSettings _eventBusConnectionSettings;

    public ConsumerEventBusRabbitMQ(IOptions<EventBusChannelSettings> eventBusChannelSettings, IOptions<EventBusConnectionSettings> eventBusConnectionSettings, IRabbitMQPersistentConnection persistentConnection, ILogger<ConsumerEventBusRabbitMQ<TEventType>> logger,
         IIntegrationEventHandler<TEventType> handler) : base(persistentConnection)
    {
        _eventBusConnectionSettings = eventBusConnectionSettings.Value;
        _eventBusChannelSettings = eventBusChannelSettings.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _handler = handler;
        _handler.Handled += BasicAck;
        _persistentConnection.TryConnect();
        CreateConsumerChannelAndStartToConsume();
    }
    protected internal void CreateConsumerChannelAndStartToConsume()
    {
        _logger.LogTrace("Creating RabbitMQ consumer channel");
        _consumerChannel = _persistentConnection.CreateModel();
        _consumerChannel.ExchangeDeclare(exchange: _eventBusConnectionSettings.BrokerName, type: _eventBusConnectionSettings.Type);

        _consumerChannel.QueueDeclare(queue: _eventBusChannelSettings.SubscriptionClientName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _consumerChannel.QueueBind(queue: _eventBusChannelSettings.SubscriptionClientName, exchange: _eventBusConnectionSettings.BrokerName, routingKey: string.Empty);
        _consumerChannel.BasicQos(prefetchSize: 0, prefetchCount: _eventBusChannelSettings.PrefetchCount, global: false);
        _consumerChannel.CallbackException += Channel_CallbackException;

        var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
        consumer.Received += OnEventReceived<TEventType>;
        
        _consumerChannel.BasicConsume(queue: _eventBusChannelSettings.SubscriptionClientName, autoAck: false,consumer: consumer);
    }

    private void Channel_CallbackException(object sender, CallbackExceptionEventArgs e)
    {
        _logger.LogWarning(e.Exception, "Recreating RabbitMQ consumer channel");
        _consumerChannel.Dispose();
        CreateConsumerChannelAndStartToConsume();
    }

    protected virtual async Task OnEventReceived<TEventType>(object sender, BasicDeliverEventArgs @event) {
        var context = new RabbitMQMessageHandlerContext { DeliveryTag = @event.DeliveryTag, Message = Encoding.UTF8.GetString(@event.Body.Span) };
        _handler.Handle(context);
    }

    public new void Dispose()
    {
        if (_consumerChannel != null)
        {
            _consumerChannel.Dispose();
        }
    }

    // Even on exception we take the message off the queue.
    // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
    // For more information see: https://www.rabbitmq.com/dlx.html
    public void BasicAck(ulong deliveryTag, bool multiple = false)
    {
        _consumerChannel?.BasicAck(deliveryTag, multiple);
    }
}
