using Microsoft.Extensions.DependencyInjection;

namespace Ringleader.HttpClientFactory
{
    internal sealed class DefaultContextualHttpClientBuilder : IContextualHttpClientBuilder
    {
        public DefaultContextualHttpClientBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
