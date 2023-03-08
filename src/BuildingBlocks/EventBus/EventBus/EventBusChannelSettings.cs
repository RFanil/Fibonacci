namespace Fibonacci.BuildingBlocks.EventBus;

public class EventBusChannelSettings {
    public string SubscriptionClientName { get; set; }
    public ushort PrefetchCount { get; set; }
}
