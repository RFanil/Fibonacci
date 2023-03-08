namespace Fibonacci.BuildingBlocks.EventBus;
public class EventBusConnectionSettings {
    public string EventBusConnection { get; set; }
    public string EventBusUserName { get; set; }
    public string EventBusPassword { get; set; }
    public int EventBusRetryCount { get; set; }
    public string BrokerName { get; set; }
    public string Type { get; set; }
}
