using Fibonacci.Processing;
using Fibonacci.BuildingBlocks.EventBus.Abstractions;
using WebApiService.IntegrationEvents.Events;

namespace WebApiService {
    internal class EventBusSender : ISender {
        private readonly IProducerEventBus _produceEventBus;

        public EventBusSender(IProducerEventBus produceEventBus) {
            _produceEventBus = produceEventBus;
        }
        public async Task Send(double number) {
            await Task.Run(() => {
                var @event = new NextNumberInFibonacciSequenceCalculatedIntegrationEvent(number);
                _produceEventBus.Publish(@event);
            });
        }
    }
}
