using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Http;

namespace Ringleader
{
    /// <summary>
    /// Default builder implementation for Ringleader
    /// </summary>
    public class DefaultRingleaderBuilder : RingleaderBuilder
    {
        public DefaultRingleaderBuilder(IServiceCollection services) : base(services)
        {
        }

        /// <summary>
        /// Run the Ringleader builder
        /// </summary>
        public override void Build()
        {
            // Decorate the HttpClientFactoryOptions monitor to allow us to correctly resolve granular handlers to their parent typed client options and pipeline
            Services.TryAddSingleton<IOptionsMonitor<HttpClientFactoryOptions>>(c =>
                    ActivatorUtilities.CreateInstance<ClientHandlerResolutionDecorator>(
                        c,
                        ActivatorUtilities.CreateInstance<OptionsMonitor<HttpClientFactoryOptions>>(c)));

            // Add our builder filter to apply the primary handler during the DefaultHttpClientFactory execution
            Services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, ContextualHandlerBuilderFilter>());

            if (!UsingCustomRegistry)
            {
                // Add our in-memory registry for contextual HttpClient -> Handlers
                Services.AddSingleton<IHttpClientHandlerRegistry, InMemoryHttpClientHandlerRegistry>();
            }

            // Throw if builders not set
            if(!IsHandlerFactorySet || !IsClientFactorySet)
            {
                throw new ArgumentException($"{nameof(DefaultRingleaderBuilder)} - Either handler factory or client factory was not provided");
            }
        }
    }
}
