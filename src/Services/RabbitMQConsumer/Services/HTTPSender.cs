namespace RabbitMQProducerFirst.Services;
public class HTTPSender {
    private readonly HttpClient _httpClient;

    public HTTPSender() {
        var socketsHandler = new SocketsHttpHandler {
            PooledConnectionLifetime = TimeSpan.FromMinutes(10),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
            MaxConnectionsPerServer = 10
        };

        _httpClient = new HttpClient(socketsHandler);
        _httpClient.BaseAddress = new Uri("http://RabbitMQProducer:80/");
    }
    
    public async Task Post(double number) {
            await _httpClient.PostAsync("Fibanacci/PostNextFibonacciNumber?payload=" + number, null);
    }
}
