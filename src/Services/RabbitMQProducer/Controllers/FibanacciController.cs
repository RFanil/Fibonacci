using Fibonacci;
using Fibonacci.Processing;
using Microsoft.AspNetCore.Mvc;
using Fibonacci.BuildingBlocks.EventBus.Abstractions;
using Fibonacci.BuildingBlocks.EventBus.Events;
using System.Net;
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
        public void PostNextFibonacciNumber(double payload) {
            var next = fibonacciCalculator.GetNextNumberFromFibonacciSequence(payload);
            _eventBus.Publish(new NextNumberInFibonacciSequenceCalculatedIntegrationEvent(next));
        }
    }
}