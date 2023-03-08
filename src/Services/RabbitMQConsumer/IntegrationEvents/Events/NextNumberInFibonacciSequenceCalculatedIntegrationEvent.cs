using Fibonacci.BuildingBlocks.EventBus.Events;

namespace RabbitMQProducerFirst.IntegrationEvents.Events
{
    public record NextNumberInFibonacciSequenceCalculatedIntegrationEvent : IntegrationEvent
    {
        public double Number { get; set; }

        public NextNumberInFibonacciSequenceCalculatedIntegrationEvent(double number)
            => Number = number;
    }
}
