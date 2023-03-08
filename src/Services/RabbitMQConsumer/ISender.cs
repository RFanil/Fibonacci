using RabbitMQProducerFirst.IntegrationEvents.Events;

namespace RabbitMQConsumer;
public interface ISender {
    Task Send(NextNumberInFibonacciSequenceCalculatedIntegrationEvent @event);
}