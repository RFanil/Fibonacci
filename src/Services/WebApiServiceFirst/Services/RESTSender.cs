using Fibonacci.Processing;

namespace WebApiServiceFirst.Services;
internal class RESTSender : ISender {
    private readonly HTTPSender _webServiceClient;

    public RESTSender(HTTPSender webServiceClient) {
        _webServiceClient = webServiceClient;
    }
    public async Task Send(double number) {
        await _webServiceClient.Post(number);
    }
}
