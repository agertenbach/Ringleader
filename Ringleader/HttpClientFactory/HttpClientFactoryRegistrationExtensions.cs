using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Ringleader.HttpClientFactory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net.Http;
using Ringleader.Shared;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class HttpClientFactoryRegistrationExtensions
    {
        /// <summary>
        /// Enable request context aware primary handlers for named and typed <see cref="HttpClient"/> instances through <see cref="IContextualHttpClientFactory"/>
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/agertenbach/Ringleader">https://github.com/agertenbach/Ringleader</see>
        /// </remarks>
        /// <typeparam name="TPrimaryHandlerFactory">A factory implementation for resolving primary <see cref="HttpMessageHandler"/> instances based on string-based context</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IContextualHttpClientBuilder AddContextualHttpClientFactory<TPrimaryHandlerFactory>(this IServiceCollection services)
            where TPrimaryHandlerFactory : class, IPrimaryHandlerFactory
        {
            services.AddSingleton<IPrimaryHandlerFactory, TPrimaryHandlerFactory>();
            return AddContextualHttpClientFactoryCore(services);
        }

        /// <summary>
        /// Enable request context aware primary handlers for named and typed <see cref="HttpClient"/> instances through <see cref="IContextualHttpClientFactory"/>
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/agertenbach/Ringleader">https://github.com/agertenbach/Ringleader</see>
        /// </remarks>
        /// <param name="services"></param>
        /// <param name="primaryHandlerResolver">Function for optionally resolving a custom primary <see cref="HttpMessageHandler"/> instance based on a client name and supplied handler context</param>
        /// <returns></returns>
        public static IContextualHttpClientBuilder AddContextualHttpClientFactory(this IServiceCollection services, Func<string, string, HttpMessageHandler?> primaryHandlerResolver)
        {
            services.Configure<DefaultActionedPrimaryHandlerFactoryOptions>(o => o.HandlerFactory = primaryHandlerResolver);
            services.AddSingleton<IPrimaryHandlerFactory, DefaultActionedPrimaryHandlerFactory>();
            return AddContextualHttpClientFactoryCore(services);
        }

        /// <summary>
        /// Base registration
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IContextualHttpClientBuilder AddContextualHttpClientFactoryCore(this IServiceCollection services)
        {
            services.TryAddSingleton<IOptionsMonitor<HttpClientFactoryOptions>>(c =>
                    ActivatorUtilities.CreateInstance<ClientHandlerResolutionDecorator>(
                        c,
                        ActivatorUtilities.CreateInstance<OptionsMonitor<HttpClientFactoryOptions>>(c)));

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, ContextualHandlerBuilderFilter>());
            services.TryAddSingleton<IHttpClientContextResolver, DefaultHttpClientContextResolver>();
            services.TryAddSingleton<IContextualHttpClientFactory, DefaultContextualHttpClientFactory>();

            return new DefaultContextualHttpClientBuilder(services);
        }

        /// <summary>
        /// Register a custom resolver for compound context identifiers based on client and handler names
        /// </summary>
        /// <typeparam name="TContextResolver">Implementation of <see cref="IHttpClientContextResolver"/></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IContextualHttpClientBuilder WithContextResolver<TContextResolver>(this IContextualHttpClientBuilder builder)
            where TContextResolver : class, IHttpClientContextResolver
        {
            builder.Services.AddSingleton<IHttpClientContextResolver, TContextResolver>();
            return builder;
        }

        /// <summary>
        /// Register a custom factory for resolving contextual named or typed <see cref="HttpClient"/> instances
        /// </summary>
        /// <typeparam name="TContextualClientFactory">Implementation of <see cref="IContextualHttpClientFactory"/></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IContextualHttpClientBuilder WithContextualFactory<TContextualClientFactory>(this IContextualHttpClientBuilder builder)
            where TContextualClientFactory : class, IContextualHttpClientFactory
        {
            builder.Services.AddSingleton<IContextualHttpClientFactory, TContextualClientFactory>();
            return builder;
        }
    }
}
