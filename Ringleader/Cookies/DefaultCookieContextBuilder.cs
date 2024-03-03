using Microsoft.Extensions.DependencyInjection;

namespace Ringleader.HttpClientFactory
{
    internal sealed class DefaultCookieContextBuilder : ICookieContextBuilder
    {
        public DefaultCookieContextBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
