namespace Fibonacci.BuildingBlocks.EventBus.Abstractions;

public interface IProducerEventBus {
    void Publish(IntegrationEvent @event);
}
