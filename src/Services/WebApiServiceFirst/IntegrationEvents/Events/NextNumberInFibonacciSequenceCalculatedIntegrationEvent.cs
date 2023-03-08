using Fibonacci.BuildingBlocks.EventBus.Events;

namespace WebApiServiceFirst.IntegrationEvents.Events
{
    public record NextNumberInFibonacciSequenceCalculatedIntegrationEvent : IntegrationEvent
    {
        public double Number { get; init; }

        public NextNumberInFibonacciSequenceCalculatedIntegrationEvent(double number)
            => Number = number;
    }
}
