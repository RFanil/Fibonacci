using Fibonacci;
using Microsoft.AspNetCore.Mvc;
using Fibonacci.BuildingBlocks.EventBus.Abstractions;
using RabbitMQProducer.IntegrationEvents.Events;

namespace RabbitMQProducer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FibanacciController : ControllerBase {
        private readonly IProducerEventBus _eventBus;
        private readonly ILogger<FibanacciController> _logger;
        private readonly FibonacciCalculator fibonacciCalculator;

        public FibanacciController(IProducerEventBus eventBus, ILogger<FibanacciController> logger, FibonacciCalculator fibonacciCalculator) {
            _eventBus = eventBus;
            _logger = logger;
            this.fibonacciCalculator = fibonacciCalculator;
        }

        [HttpPost(nameof(PostNextFibonacciNumber), Name = nameof(PostNextFibonacciNumber))]
        public void PostNextFibonacciNumber(NextNumberInFibonacciSequenceCalculatedIntegrationEvent @event) {
            var next = fibonacciCalculator.GetNextNumberFromFibonacciSequence(@event.Number);
            _eventBus.Publish(new NextNumberInFibonacciSequenceCalculatedIntegrationEvent(next));
        }
    }
}