using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Ringleader.Cookies;
using Ringleader.Shared;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CookieContextRegistrationExtensions
    {
        /// <summary>
        /// Opt the specified client into contextual cookie support on a per-request basis and disable cookies for the primary handler
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/agertenbach/Ringleader">https://github.com/agertenbach/Ringleader</see>
        /// </remarks>
        /// <remarks>
        /// Use the <see cref="CookieContextHttpExtensions.SetCookieContext(HttpRequestMessage, string)"/> extension to apply to a <see cref="HttpRequestMessage"/>
        /// </remarks>
        /// <param name="clientBuilder"></param>
        /// <returns></returns>
        public static IHttpClientBuilder UseContextualCookies(this IHttpClientBuilder clientBuilder)
            => UseContextualCookies<ConcurrentDictionaryCookieContainerCache>(clientBuilder);

        /// <summary>
        /// Opt the specified client into contextual cookie support on a per-request basis and disable cookies for the primary handler
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/agertenbach/Ringleader">https://github.com/agertenbach/Ringleader</see>
        /// </remarks>
        /// <param name="clientBuilder"></param>
        /// <returns></returns>
        public static IHttpClientBuilder UseContextualCookies<TCache>(this IHttpClientBuilder clientBuilder) where TCache : class, ICookieContainerCache
        {
            clientBuilder.Services.TryAddSingleton<ICookieContainerCache, TCache>();
            clientBuilder.Services.TryAddScoped<CookieContextDelegatingHandler>();
            clientBuilder.Services.TryAddSingleton<IHttpClientContextResolver, DefaultHttpClientContextResolver>(); // This adds compat with Ringleader.HttpClientFactory
            clientBuilder.AddHttpMessageHandler<CookieContextDelegatingHandler>();
            clientBuilder.Services.Configure<DisableCookiesHandlerBuilderFilterOptions>(o => o.ClientNames.Add(clientBuilder.Name));
            clientBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, DisableCookiesHandlerBuilderFilter>());
            return clientBuilder;
        }  
        
        /// <summary>
        /// Add a generic <see cref="CookieContextHttpClient"/> that allows context-based cookie support for a subset of <see cref="HttpClient"/> APIs
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddCookieContextHttpClient(this IServiceCollection services)
            => services.AddHttpClient<CookieContextHttpClient>()
                .UseContextualCookies();

        /// <summary>
        /// Add a generic <see cref="CookieContextHttpClient"/> that allows context-based cookie support for a subset of <see cref="HttpClient"/> APIs
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddCookieContextHttpClient<TCache>(this IServiceCollection services)
            where TCache : class, ICookieContainerCache
            => services.AddHttpClient<CookieContextHttpClient>()
                .UseContextualCookies<TCache>();
    }
}
