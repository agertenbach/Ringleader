
namespace WebExample
{
    public class ExampleTypedClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExampleTypedClient> _logger;

        public ExampleTypedClient(HttpClient httpClient, ILogger<ExampleTypedClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public  Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, Guid userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("This request was sent with the example typed client using the cookie container for {user}", userId);
            request.SetCookieContext(userId.ToString());
            return _httpClient.SendAsync(request, cancellationToken); 
        }
    }
}
