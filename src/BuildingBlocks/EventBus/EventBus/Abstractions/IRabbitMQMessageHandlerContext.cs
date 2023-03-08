namespace Fibonacci.BuildingBlocks.EventBus.Abstractions;

public interface IRabbitMQMessageHandlerContext: IMessageHandlerContext {
    ///<summary>The delivery tag for this delivery. See
    ///IModel.BasicAck.</summary>
    public ulong DeliveryTag { get; set; }
    ///<summary>The message body.</summary>
    public string Message { get; set; }
}
