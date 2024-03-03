using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Ringleader.Cookies;
using Ringleader.Shared;
using System;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CookieContextRegistrationExtensions
    {
        /// <summary>
        /// Register <typeparamref name="TCache"/> as the singleton cookie container cache for Ringleader
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/agertenbach/Ringleader">https://github.com/agertenbach/Ringleader</see>
        /// </remarks>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCookieContainerCache<TCache>(this IServiceCollection services)
            where TCache : class, ICookieContainerCache
        {
            services.AddSingleton<ICookieContainerCache, TCache>();
            return services;
        }

        /// <summary>
        /// Register a singleton cookie container cache instance for Ringleader
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/agertenbach/Ringleader">https://github.com/agertenbach/Ringleader</see>
        /// </remarks>
        /// <param name="services"></param>
        /// <param name="cache">Singleton instance for <see cref="ICookieContainerCache"/></param>
        /// <returns></returns>
        public static IServiceCollection AddCookieContainerCache(this IServiceCollection services, ICookieContainerCache cache)
        {
            services.AddSingleton<ICookieContainerCache>(cache);
            return services;
        }

        /// <summary>
        /// Register an implementation factory for the singleton cookie container cache instance for Ringleader
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/agertenbach/Ringleader">https://github.com/agertenbach/Ringleader</see>
        /// </remarks>
        /// <param name="services"></param>
        /// <param name="implementationFactory">Resolve an <see cref="ICookieContainerCache"/> from the service provider</param>
        /// <returns></returns>
        public static IServiceCollection AddCookieContainerCache(this IServiceCollection services, Func<IServiceProvider, ICookieContainerCache> implementationFactory)
        {
            services.AddSingleton<ICookieContainerCache>(implementationFactory);
            return services;
        }

        /// <summary>
        /// Opt the specified client into contextual cookie support on a per-request basis and disable cookies for the primary handler
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/agertenbach/Ringleader">https://github.com/agertenbach/Ringleader</see>
        /// </remarks>
        /// <param name="clientBuilder"></param>
        /// <returns></returns>
        public static IHttpClientBuilder UseContextualCookies(this IHttpClientBuilder clientBuilder) 
        {
            clientBuilder.Services.TryAddSingleton<ICookieContainerCache, ConcurrentDictionaryCookieContainerCache>();
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
    }
}
