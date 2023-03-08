using Fibonacci;
using Fibonacci.Processing;
using Fibonacci.BuildingBlocks.EventBus;
using Fibonacci.BuildingBlocks.EventBus.Abstractions;
using Fibonacci.BuildingBlocks.EventBusRabbitMQ.Connection;
using Fibonacci.BuildingBlocks.EventBusRabbitMQ.Consumer;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Reflection;
using RabbitMQProducerFirst;
using RabbitMQProducerFirst.IntegrationEvents.EventHandling;
using RabbitMQProducerFirst.IntegrationEvents.Events;
using RabbitMQProducerFirst.Services;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
InitializeConfigVariables();
ConfigureIOptions();
ConfigureDI();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<FibonacciCalculator>();
builder.Services.AddSingleton<IIntegrationEventHandler<NextNumberInFibonacciSequenceCalculatedIntegrationEvent>, NextNumberInFibonacciSequenceCalculatedIntegrationEventHandlerParallel>();
builder.Services.AddSingleton<ISender, RESTSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

StartCalculation();
app.Run();

void InitializeConfigVariables() {
    var enviroment = builder.Environment.EnvironmentName;
    builder.Configuration
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{enviroment}.json", true, true)
            .AddEnvironmentVariables();
}
void ConfigureIOptions() {
    builder.Services.Configure<EventBusConnectionSettings>(builder.Configuration.GetSection(nameof(EventBusConnectionSettings)));
    builder.Services.Configure<EventBusChannelSettings>(builder.Configuration.GetSection(nameof(EventBusChannelSettings)));
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
}
void ConfigureDI() {
    builder.Services.AddSingleton<HTTPSender>();
    builder.Services.AddSingleton<ConsumerEventBusRabbitMQ<NextNumberInFibonacciSequenceCalculatedIntegrationEvent>>();
    builder.Services.AddSingleton<IRabbitMQPersistentConnection, DefaultRabbitMQPersistentConnection>();
}
void StartCalculation() {
    //warm up, subscribe to the channel
    var eventBus = app.Services.GetRequiredService<ConsumerEventBusRabbitMQ<NextNumberInFibonacciSequenceCalculatedIntegrationEvent>>();
    var appOption = app.Services.GetService<IOptions<AppSettings>>();
    var fibonacciCalculator = app.Services.GetService<FibonacciCalculator>();
    int firstNumberFromTheFibonacciSequence = fibonacciCalculator.GetTheFirstNumberFromTheFibonacciSequence();

    Parallel.For(0, appOption.Value.MaxDegreeOfParallelism + 1, (i) => {
        var sender = app.Services.GetRequiredService<ISender>();
        sender.Send(firstNumberFromTheFibonacciSequence);
    });
}