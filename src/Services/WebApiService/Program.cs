using Fibonacci.Processing;
using Fibonacci;
using Fibonacci.BuildingBlocks.EventBus;
using Fibonacci.BuildingBlocks.EventBus.Abstractions;
using Fibonacci.BuildingBlocks.EventBusRabbitMQ;
using RabbitMQ.Client;
using WebApiService;
using Microsoft.Extensions.ObjectPool;
using Fibonacci.BuildingBlocks.EventBusRabbitMQ.Connection;
using Fibonacci.BuildingBlocks.EventBusRabbitMQ.Producer;
using System.Reflection.Metadata.Ecma335;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<EventBusConnectionSettings>(builder.Configuration.GetSection(nameof(EventBusConnectionSettings)));
var consumerEventBusSettings = builder.Configuration
   .GetSection(nameof(EventBusConnectionSettings))
   .Get<EventBusConnectionSettings>();

ConfigureDI();
void ConfigureDI() {
    builder.Services.AddSingleton<IRabbitMQPersistentConnection, DefaultRabbitMQPersistentConnection>();
}
RegisterEventBus(builder, consumerEventBusSettings);



/*TODO inject all values from appsettings
list:
1) builder.Services.AddSingleton<TaskScheduler>(sp => new LimitedConcurrencyLevelTaskScheduler(1));

 */


//FibonacciCalculator injection
builder.Services.AddSingleton<FibonacciCalculator>();
builder.Services.AddSingleton<ISender, EventBusSender>();


builder.Services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.Run();

void RegisterEventBus(WebApplicationBuilder builder, EventBusConnectionSettings consumerEventBusSettings) {
    builder.Services.AddSingleton<IProducerEventBus, ProducerEventBusRabbitMQ>();
}
