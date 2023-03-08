using RabbitMQProducerFirst.IntegrationEvents.Events;

namespace RabbitMQProducer.Utilities {
    public class NextNumberInFibonacciSequenceCalculatedIntegrationEventDeserializer {
        public static void Parse(string json, NextNumberInFibonacciSequenceCalculatedIntegrationEvent @event) {
            var startIndex = 14; // where number starts
            var ind = json.IndexOf(",", startIndex);
            @event.Number = int.Parse(json.AsSpan(startIndex, ind - startIndex));

            startIndex = ind + 11; // where guid starts
            int guidLength = 32 + 4;
            @event.Id = Guid.Parse(json.AsSpan(startIndex, guidLength));

            startIndex = startIndex + guidLength + 23; // where date starts
            int dateLength = 27;
            @event.CreationDate = DateTime.Parse(json.AsSpan(startIndex, dateLength));
        }
    }
}
