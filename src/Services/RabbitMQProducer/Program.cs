using Fibonacci.Processing;
using Fibonacci;
using Fibonacci.BuildingBlocks.EventBus;
using Fibonacci.BuildingBlocks.EventBus.Abstractions;
using Fibonacci.BuildingBlocks.EventBusRabbitMQ;
using RabbitMQ.Client;
using RabbitMQProducer;
using Microsoft.Extensions.ObjectPool;
using Fibonacci.BuildingBlocks.EventBusRabbitMQ.Connection;
using Fibonacci.BuildingBlocks.EventBusRabbitMQ.Producer;

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
builder.Services.AddSingleton<FibonacciCalculator>();

builder.Services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();

builder.Services.AddSingleton<IProducerEventBus, ProducerEventBusRabbitMQ>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.Run();


void T() {

    var policy = new Microsoft.Extensions.ObjectPool.StringBuilderPooledObjectPolicy();
}