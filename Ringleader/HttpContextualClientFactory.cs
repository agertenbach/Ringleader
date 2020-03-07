using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Ringleader
{
    /// <summary>
    /// Generate typed HttpClient instances using the correct primary handler resolved from the pool based on the supplied context
    /// </summary>
    /// <typeparam name="TClient">The typed HttpClient</typeparam>
    /// <typeparam name="TContext">The context used to resolve the appropriate handler</typeparam>
    public abstract class HttpContextualClientFactory<TClient, TContext>
        : IHttpContextualClientFactory<TClient, TContext>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        protected readonly IHttpClientHandlerRegistry _httpClientHandlerRegistry;
        protected readonly ILogger _logger;

        public HttpContextualClientFactory(IHttpClientHandlerRegistry httpClientHandlerRegistry, IHttpClientFactory httpClientFactory, ILogger<HttpContextualClientFactory<TClient, TContext>> logger)
        {
            _httpClientHandlerRegistry = httpClientHandlerRegistry ?? throw new ArgumentNullException(nameof(httpClientHandlerRegistry));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;
        }

        /// <summary>
        /// Generate a typed HttpClient for a given TContext
        /// </summary>
        /// <param name="context">The TContext used to resolve the correct handler</param>
        /// <returns>A typed HttpClient instance</returns>
        public abstract TClient GetTypedClientByContext(TContext context);

        /// <summary>
        /// Given a TContext, resolve the string identifier of the registered handler and handler settings to use
        /// </summary>
        /// <param name="context">A TContext to resolve to a handler identifier string</param>
        /// <returns>The string identifier of the handler</returns>
        protected abstract string ResolveIdentifierByContext(TContext context);

        /// <summary>
        /// Generate a base HttpClient with a primary handler from the pool with the appropriate configuration for the provided context
        /// </summary>
        /// <param name="context">The TContext to resolve into a primary handler to apply to the HttpClient</param>
        /// <returns>An HttpClient with the primary handler configured</returns>
        protected virtual HttpClient CreateClient(TContext context)
        {
            string key = _httpClientHandlerRegistry.RegisterClientHandlerKey(typeof(TClient), ResolveIdentifierByContext(context));
            _logger.LogDebug($"{nameof(HttpContextualClientFactory<TClient, TContext>)} - Registered client-handler key {key}");
            return _httpClientFactory.CreateClient(key);
        }
    }
}
