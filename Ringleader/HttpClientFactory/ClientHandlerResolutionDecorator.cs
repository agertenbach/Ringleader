using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Ringleader.Shared;

namespace Ringleader.HttpClientFactory
{
    /// <summary>
    /// Decorates the <see cref="IOptionsMonitor{TOptions}"/> for <see cref="HttpClientFactoryOptions"/> to resolve the correct typed client name 
    /// from a compound context identifier before passing it to the underlying <see cref="IOptionsMonitor{TOptions}.Get(string)"/> call
    /// </summary>
    public class ClientHandlerResolutionDecorator : IOptionsMonitor<HttpClientFactoryOptions>
    {
        private readonly IOptionsMonitor<HttpClientFactoryOptions> _optionsMonitor;
        private readonly IHttpClientContextResolver _resolver;
        private readonly ILogger _logger;

        public ClientHandlerResolutionDecorator(ILogger<ClientHandlerResolutionDecorator> logger, IOptionsMonitor<HttpClientFactoryOptions> optionsMonitor, IHttpClientContextResolver resolver)
        {
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _logger = logger;
        }

        public HttpClientFactoryOptions CurrentValue 
            => _optionsMonitor.CurrentValue;

        public HttpClientFactoryOptions Get(string name)
        {
            string client = _resolver.ParseClientName(name);
            _logger.LogTrace("Resolving options for {client} based on extended context {name}", client, name);
            return _optionsMonitor.Get(client);
        }

        public IDisposable OnChange(Action<HttpClientFactoryOptions, string> listener)
            => _optionsMonitor.OnChange(listener);
    }
}
