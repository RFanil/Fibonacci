using RabbitMQProducerFirst.IntegrationEvents.Events;

namespace RabbitMQProducer.Utilities {
    public class NextNumberInFibonacciSequenceCalculatedIntegrationEventDeserializer {
        public static void Parse(string json, NextNumberInFibonacciSequenceCalculatedIntegrationEvent @event) {
            var startIndex = 14; // where number starts
            var ind = json.IndexOf(",", startIndex);
            @event.Number = int.Parse(json.AsSpan(startIndex, ind - startIndex));

            Guid guid = Guid.Empty;
            startIndex = ind + 11; // where guid starts
            int guidLength = 32 + 4;
            var ttt = json.IndexOf('3');
            var testNumber = json.AsSpan(startIndex, guidLength).ToString();
            @event.Id = Guid.Parse(json.AsSpan(startIndex, guidLength));

            DateTime dateTime = DateTime.MinValue;
            startIndex = startIndex + guidLength + 23; // where date starts
            int dateLength = 27;
            @event.CreationDate = DateTime.Parse(json.AsSpan(startIndex, dateLength));
        }
    }
}
