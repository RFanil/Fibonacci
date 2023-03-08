namespace Fibonacci.BuildingBlocks.EventBus.Abstractions;

public class RabbitMQMessageHandlerContext : IRabbitMQMessageHandlerContext {
    public ulong DeliveryTag { get; set; }
    public string Message { get; set; }
}
