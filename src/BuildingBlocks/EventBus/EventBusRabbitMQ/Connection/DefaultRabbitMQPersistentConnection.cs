using Fibonacci.BuildingBlocks.EventBus;
using Microsoft.Extensions.Options;

namespace Fibonacci.BuildingBlocks.EventBusRabbitMQ.Connection;

public class DefaultRabbitMQPersistentConnection
    : IRabbitMQPersistentConnection
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
    private readonly int _retryCount = 5;
    private IConnection _connection;
    public bool Disposed;

    readonly object _syncRoot = new();

    public DefaultRabbitMQPersistentConnection(IOptions<EventBusConnectionSettings> eventBusSettings, ILogger<DefaultRabbitMQPersistentConnection> logger) {
        var factory = new ConnectionFactory() {
            HostName = eventBusSettings.Value.EventBusConnection,
            DispatchConsumersAsync = true,
            Port = 5672
        };

        if (!string.IsNullOrEmpty(eventBusSettings.Value.EventBusUserName)) {
            factory.UserName = eventBusSettings.Value.EventBusUserName;
        }

        if (!string.IsNullOrEmpty(eventBusSettings.Value.EventBusPassword)) {
            factory.Password = eventBusSettings.Value.EventBusPassword;
        }

        if (eventBusSettings.Value.EventBusRetryCount != 0) {
            _retryCount = eventBusSettings.Value.EventBusRetryCount;
        }

        _connectionFactory = factory?? throw new ArgumentNullException(nameof(factory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool IsConnected => _connection is { IsOpen: true } && !Disposed;

    public IModel CreateModel()
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
        }

        return _connection.CreateModel();
    }

    public void Dispose()
    {
        if (Disposed) return;

        Disposed = true;

        try
        {
            _connection.ConnectionShutdown -= OnConnectionShutdown;
            _connection.CallbackException -= OnCallbackException;
            _connection.ConnectionBlocked -= OnConnectionBlocked;
            _connection.Dispose();
        }
        catch (IOException ex)
        {
            _logger.LogCritical(ex.ToString());
        }
    }

    public bool TryConnect()
    {
        _logger.LogInformation("RabbitMQ Client is trying to connect");

        lock (_syncRoot)
        {
            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                }
            );

            policy.Execute(() =>
            {
                _connection = _connectionFactory
                        .CreateConnection();
            });

            if (IsConnected)
            {
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;

                _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

                return true;
            }
            else
            {
                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                return false;
            }
        }
    }

    private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
    {
        if (Disposed) return;

        _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

        TryConnect();
    }

    void OnCallbackException(object sender, CallbackExceptionEventArgs e)
    {
        if (Disposed) return;

        _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

        TryConnect();
    }

    void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
    {
        if (Disposed) return;

        _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

        TryConnect();
    }
}
