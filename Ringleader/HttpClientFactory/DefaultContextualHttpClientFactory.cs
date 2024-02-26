using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Ringleader.Shared;
using System;
using System.Net.Http;

namespace Ringleader.HttpClientFactory
{
    /// <summary>
    /// Default implementation wrapper around <see cref="IHttpClientContextResolver"/> and the service provider
    /// </summary>
    public class DefaultContextualHttpClientFactory : IContextualHttpClientFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientContextResolver _resolver;
        private readonly IHttpClientFactory _httpClientFactory;

        public DefaultContextualHttpClientFactory(IServiceProvider serviceProvider, IHttpClientContextResolver resolver, IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public TClient CreateClient<TClient>(string handlerContext)
        {
            var client = CreateClient(typeof(TClient).Name, handlerContext);
            return _serviceProvider.GetRequiredService<ITypedHttpClientFactory<TClient>>().CreateClient(client);
        }

        public HttpClient CreateClient(string name, string handlerContext)
            => _httpClientFactory.CreateClient(_resolver.CreateContextName(name, handlerContext));

        public TClient CreateClient<TClient, TContext>(TContext handlerContext, Func<TContext, string>? handlerContextResolver = null)
            => CreateClient<TClient>(
                handlerContextResolver is null 
                    ? (handlerContext?.ToString() ?? string.Empty) 
                    : handlerContextResolver.Invoke(handlerContext));

        public HttpClient CreateClient<TContext>(string name, TContext handlerContext, Func<TContext, string>? handlerContextResolver = null)
            => CreateClient(name,
                handlerContextResolver is null
                    ? (handlerContext?.ToString() ?? string.Empty)
                    : handlerContextResolver.Invoke(handlerContext));
    }
}
