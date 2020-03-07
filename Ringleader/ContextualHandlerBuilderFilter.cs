using System;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Ringleader
{
    /// <summary>
    /// Request and apply the correct PrimaryHandler during the HttpMessageHandlerBuilder execution based on the compound name
    /// </summary>
    public class ContextualHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly IPrimaryHandlerFactory _primaryHandlerFactory;
        private readonly IHttpClientHandlerRegistry _httpClientHandlerRegistry;
        private readonly ILogger _logger;

        public ContextualHandlerBuilderFilter(ILogger<ContextualHandlerBuilderFilter> logger, IPrimaryHandlerFactory primaryHandlerFactory, IHttpClientHandlerRegistry httpClientHandlerRegistry)
        {
            _primaryHandlerFactory = primaryHandlerFactory ?? throw new ArgumentNullException(nameof(primaryHandlerFactory));
            _httpClientHandlerRegistry = httpClientHandlerRegistry ?? throw new ArgumentNullException(nameof(httpClientHandlerRegistry));
            _logger = logger;
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return (builder) =>
            {
                string handerIdentifier = _httpClientHandlerRegistry.GetHandlerIdentifierFromKey(builder.Name);
                _logger.LogDebug($"{nameof(ContextualHandlerBuilderFilter)} - Building primary handler with name [{handerIdentifier}] for builder name [{builder.Name}]");
                builder.PrimaryHandler = _primaryHandlerFactory.CreateHandler(handerIdentifier);
                next(builder);
            };
        }
    }
}
