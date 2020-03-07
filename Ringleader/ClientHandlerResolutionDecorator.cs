using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Ringleader
{
    /// <summary>
    /// Decorates the IOptionsMonitor for HttpClientFactoryOptions to resolve the correct typed client name from a compound key
    /// before passing it to the underlying Get() call
    /// </summary>
    public class ClientHandlerResolutionDecorator : IOptionsMonitor<HttpClientFactoryOptions>
    {
        private readonly IOptionsMonitor<HttpClientFactoryOptions> _optionsMonitor;
        private readonly IHttpClientHandlerRegistry _handlerRegistry;
        private readonly ILogger _logger;

        public ClientHandlerResolutionDecorator(ILogger<ClientHandlerResolutionDecorator> logger, IOptionsMonitor<HttpClientFactoryOptions> optionsMonitor, IHttpClientHandlerRegistry handlerRegistry)
        {
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
            _logger = logger;
        }

        public HttpClientFactoryOptions CurrentValue => _optionsMonitor.CurrentValue;

        public HttpClientFactoryOptions Get(string name)
        {
            string client = _handlerRegistry.GetClientNameFromKey(name);
            _logger.LogDebug($"{nameof(ClientHandlerResolutionDecorator)} - Resolving options for {client} based on key lookup for {name}");
            var options = _optionsMonitor.Get(client);
            return options;
        }

        public IDisposable OnChange(Action<HttpClientFactoryOptions, string> listener)
        {
            return _optionsMonitor.OnChange(listener);
        }
    }
}
