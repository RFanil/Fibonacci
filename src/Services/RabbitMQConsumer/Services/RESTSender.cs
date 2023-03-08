using Fibonacci.Processing;

namespace RabbitMQProducerFirst.Services;
internal class RESTSender : ISender {
    private readonly HttpClient _httpClient;
    public RESTSender()
    {
        var socketsHandler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(10),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
            MaxConnectionsPerServer = 10
        };

        _httpClient = new HttpClient(socketsHandler);
        _httpClient.BaseAddress = new Uri("http://rabbitmqproducer:80/");
    }

    public async Task Send(double number)
    {
        await _httpClient.PostAsync("Fibanacci/PostNextFibonacciNumber?payload=" + number, null);
    }
}
