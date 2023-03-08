using Fibonacci;
using System.Text.Json;
using RabbitMQProducerFirst.IntegrationEvents.Events;
using Fibonacci.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Options;
using RabbitMQConsumer;
using RabbitMQProducer.Utilities;

namespace RabbitMQProducerFirst.IntegrationEvents.EventHandling;
public class NextNumberInFibonacciSequenceCalculatedIntegrationEventHandlerParallel : IIntegrationEventHandler<NextNumberInFibonacciSequenceCalculatedIntegrationEvent> {

    private readonly FibonacciCalculator _fibonacciCalculator;
    private readonly ISender _sender;
    private TaskFactory _factory;
    private CancellationTokenSource cts;
    private readonly ILogger<NextNumberInFibonacciSequenceCalculatedIntegrationEventHandlerParallel> _logger;
    private readonly Type eventType = typeof(NextNumberInFibonacciSequenceCalculatedIntegrationEvent);

    public event BasicAckHandler? Handled;

    public NextNumberInFibonacciSequenceCalculatedIntegrationEventHandlerParallel(IOptions<AppSettings> appSettings, FibonacciCalculator fibonacciCalculator, ISender sender, ILogger<NextNumberInFibonacciSequenceCalculatedIntegrationEventHandlerParallel> logger) {
        var taskSchedulerPair = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Current, appSettings.Value.MaxDegreeOfParallelism);

        _factory = new TaskFactory(taskSchedulerPair.ConcurrentScheduler);
        cts = new CancellationTokenSource();
        _fibonacciCalculator = fibonacciCalculator;
        _sender = sender;
        _logger = logger;
    }

    event BasicAckHandler IIntegrationEventHandler<NextNumberInFibonacciSequenceCalculatedIntegrationEvent>.Handled {
        add {
            Handled += value;
        }

        remove {
            Handled -= value;
        }
    }

    public Task Handle(IRabbitMQMessageHandlerContext context) {
        var task = _factory.StartNew(() =>
        {
            try {
                //TODO: use object pooling
                var newEvent = new NextNumberInFibonacciSequenceCalculatedIntegrationEvent(0);
                NextNumberInFibonacciSequenceCalculatedIntegrationEventDeserializer.Parse(context.Message, newEvent);
                var nextNumber = _fibonacciCalculator.GetNextNumberFromFibonacciSequence(newEvent.Number);
                Handled?.Invoke(context.DeliveryTag, multiple: false);
                _ = _sender.Send(newEvent);

            }
            catch (Exception ex) {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", context.Message);
            }
        }, cts.Token);

        return Task.CompletedTask;
    }
}
