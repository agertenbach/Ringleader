using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ringleader
{
    public static class RingleaderBuilderExtensions
    {
        /// <summary>
        /// Add Ringleader for managing contextual handlers using HttpClientFactory
        /// </summary>
        /// <param name="services">The IServiceCollection to register services to</param>
        /// <returns>Ringleader builder instance</returns>
        public static IRingleaderBuilder AddRingleader(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return new DefaultRingleaderBuilder(services);
        }

        /// <summary>
        /// Specify the factory to generate typed HttpClient instances based on a given context
        /// </summary>
        /// <typeparam name="TFactory">The IHttpContextualFactory implementation</typeparam>
        /// <typeparam name="TClient">The typed HttpClient</typeparam>
        /// <typeparam name="TContext">The context used to determine the handler</typeparam>
        /// <param name="builder">Ringleader builder instance</param>
        /// <param name="lifetime">Service lifetime for the factory</param>
        /// <returns>Ringleader builder instance</returns>
        public static IRingleaderBuilder WithContextualClientFactory<TFactory, TClient, TContext>(this IRingleaderBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TFactory : class, IHttpContextualClientFactory<TClient, TContext>
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    builder.Services.AddSingleton<IHttpContextualClientFactory<TClient, TContext>, TFactory>();
                    break;
                case ServiceLifetime.Scoped:
                    builder.Services.AddScoped<IHttpContextualClientFactory<TClient, TContext>, TFactory>();
                    break;
                case ServiceLifetime.Transient:
                default:
                    builder.Services.AddTransient<IHttpContextualClientFactory<TClient, TContext>, TFactory>();
                    break;
            }

            builder.IsClientFactorySet = true;
            return builder;
        }

        /// <summary>
        /// Specify the factory to generate an HttpMessageHandler to act as the primary handler for the returned client
        /// </summary>
        /// <typeparam name="TFactory">The IPrimaryHandlerFactory implementation</typeparam>
        /// <param name="builder">Ringleader builder instance</param>
        /// <param name="lifetime">Service lifetime for the factory</param>
        /// <returns>Ringleader builder instance</returns>
        public static IRingleaderBuilder WithPrimaryHandlerFactory<TFactory>(this IRingleaderBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TFactory : class, IPrimaryHandlerFactory
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    builder.Services.AddSingleton<IPrimaryHandlerFactory, TFactory>();
                    break;
                case ServiceLifetime.Scoped:
                    builder.Services.AddScoped<IPrimaryHandlerFactory, TFactory>();
                    break;
                case ServiceLifetime.Transient:
                default:
                    builder.Services.AddTransient<IPrimaryHandlerFactory, TFactory>();
                    break;
            }

            builder.IsHandlerFactorySet = true;
            return builder;
        }

        /// <summary>
        /// Override the default in-memory handler registry used to store HttpClient and handler relationships with a custom registry provider
        /// </summary>
        /// <typeparam name="TRegistry">The IHttpClientHandlerRegistry implementation</typeparam>
        /// <param name="builder">Ringleader builder instance</param>
        /// <param name="lifetime">Service lifetime for the factory</param>
        /// <returns>Ringleader builder instance</returns>
        public static IRingleaderBuilder WithCustomHandlerRegistry<TRegistry>(this IRingleaderBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TRegistry : class, IHttpClientHandlerRegistry
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    builder.Services.AddSingleton<IHttpClientHandlerRegistry, TRegistry>();
                    break;
                case ServiceLifetime.Scoped:
                    builder.Services.AddScoped<IHttpClientHandlerRegistry, TRegistry>();
                    break;
                case ServiceLifetime.Transient:
                default:
                    builder.Services.AddTransient<IHttpClientHandlerRegistry, TRegistry>();
                    break;
            }

            builder.UsingCustomRegistry = true;
            return builder;
        }
    }
   
}
