using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Ringleader.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ringleader.HttpClientFactory
{
    public class ContextualClientFactoryOptions
    {
        public Dictionary<TypedClientSignature, Func<IServiceProvider, HttpClient, object>> _typedClientResolvers = new();
        
        /// <summary>
        /// Register a map between a typed client interface <typeparamref name="TInterface"/> and implementation <typeparamref name="TImplementation"/>
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public void SetTypedClientImplementation<TInterface, TImplementation>() where TInterface : class where TImplementation: class, TInterface
        {
            _typedClientResolvers.Add(TypedClientSignature.For<TInterface>(), DefaultResolver<TInterface, TImplementation>());
        } 

        /// <summary>
        /// Resolve a typed client implementation for <typeparamref name="T"/> using the specified service provider and <see cref="HttpClient"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sp"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public T ResolveTypedClientImplementation<T>(IServiceProvider sp, HttpClient client) where T : class
        {
            if(_typedClientResolvers.TryGetValue(TypedClientSignature.For<T>(), out var resolver))
            {
                object t = resolver.Invoke(sp, client);
                return (T)t;
            }
            return DefaultResolver<T, T>().Invoke(sp, client);
        }

        /// <summary>
        /// Default resolver for a typed client of <typeparamref name="TImplementation"/> for <typeparamref name="TInterface"/>
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        private static Func<IServiceProvider, HttpClient, TInterface> DefaultResolver<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface
            => (sp, client)
                => sp.GetRequiredService<ITypedHttpClientFactory<TImplementation>>().CreateClient(client);
    }
}
