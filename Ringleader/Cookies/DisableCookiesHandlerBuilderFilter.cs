using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ringleader.HttpClientFactory;
using Ringleader.Shared;
using System;
using System.Net.Http;

namespace Ringleader.Cookies
{
    public class DisableCookiesHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly IOptionsMonitor<DisableCookiesHandlerBuilderFilterOptions> _options;
        private readonly IHttpClientContextResolver _contextResolver;
        private readonly ILogger<DisableCookiesHandlerBuilderFilter> _logger;

        public DisableCookiesHandlerBuilderFilter(IOptionsMonitor<DisableCookiesHandlerBuilderFilterOptions> options, IHttpClientContextResolver contextResolver, ILogger<DisableCookiesHandlerBuilderFilter> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _contextResolver = contextResolver ?? throw new ArgumentNullException(nameof(contextResolver));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return (builder) =>
            {
                next(builder);
                var clientName = _contextResolver.ParseClientName(builder.Name);
                if (_options.CurrentValue.ClientNames.Contains(clientName))
                {
                    _logger?.LogTrace("Disabling cookies for returned handler");
                    builder.AdditionalHandlers.Insert(0, new CookieContextScopeDelegatingHandler(clientName));
                    
                    if (builder.PrimaryHandler is HttpClientHandler h) h.UseCookies = false;
                    else if (builder.PrimaryHandler is SocketsHttpHandler s) s.UseCookies = false;
                }
            };
        }
    }
}
