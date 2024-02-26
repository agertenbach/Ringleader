using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Ringleader.Shared;

namespace Ringleader.HttpClientFactory
{
    /// <summary>
    /// Request and apply a <see cref="HttpMessageHandlerBuilder.PrimaryHandler"/> during <see cref="HttpMessageHandlerBuilder"/> execution based on the compound context name
    /// </summary>
    public class ContextualHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly IPrimaryHandlerFactory _primaryHandlerFactory;
        private readonly IHttpClientContextResolver _resolver;
        private readonly ILogger _logger;

        public ContextualHandlerBuilderFilter(ILogger<ContextualHandlerBuilderFilter> logger, IPrimaryHandlerFactory primaryHandlerFactory, IHttpClientContextResolver resolver)
        {
            _primaryHandlerFactory = primaryHandlerFactory ?? throw new ArgumentNullException(nameof(primaryHandlerFactory));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _logger = logger;
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return (builder) =>
            {
                string client = _resolver.ParseClientName(builder.Name);
                string handlerContext = _resolver.ParseHandlerName(builder.Name);
                _logger.LogDebug("Building primary handler for client [{client}] with context [{context}]", client, handlerContext);
                builder.PrimaryHandler = _primaryHandlerFactory.CreateHandler(client, handlerContext);
                if (builder.PrimaryHandler is HttpClientHandler h) h.Properties.TryAdd("Handler Context", handlerContext);
                else if (builder.PrimaryHandler is SocketsHttpHandler s) s.Properties.TryAdd("Handler Context", handlerContext);
                next(builder);
            };
        }
    }
}
