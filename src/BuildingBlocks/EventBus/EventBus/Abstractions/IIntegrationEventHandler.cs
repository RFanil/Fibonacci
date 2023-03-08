namespace Fibonacci.BuildingBlocks.EventBus.Abstractions;
public interface IIntegrationEventHandler { }

public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent {
    Task Handle(IRabbitMQMessageHandlerContext context);
    event BasicAckHandler Handled;
}

public delegate void BasicAckHandler(ulong deliveryTag, bool multiple);