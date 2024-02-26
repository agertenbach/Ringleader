using Microsoft.Extensions.DependencyInjection;

namespace Ringleader.HttpClientFactory
{
    /// <summary>
    /// Builder for Ringleader custom HTTP client factory patterns
    /// </summary>
    public interface IContextualHttpClientBuilder
    {
        public IServiceCollection Services { get; }
    }
}
