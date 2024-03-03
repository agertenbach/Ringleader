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
    }
}
