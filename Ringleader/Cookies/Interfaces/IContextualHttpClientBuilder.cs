using Microsoft.Extensions.DependencyInjection;

namespace Ringleader.HttpClientFactory
{
    /// <summary>
    /// Builder for Ringleader custom cookie context patterns
    /// </summary>
    public interface ICookieContextBuilder
    {
        public IServiceCollection Services { get; }
    }
}
