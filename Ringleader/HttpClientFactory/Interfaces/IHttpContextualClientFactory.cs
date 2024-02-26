using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using System;
using System.Net.Http;

namespace System.Net.Http
{
    /// <summary>
    /// Extension interface for <see cref="IHttpClientFactory"/> to enable context-based creation and paritioning of primary handlers when resolving <see cref="HttpClient"/> instances
    /// </summary>
    /// <remarks>
    /// <see href="https://github.com/agertenbach/Ringleader">https://github.com/agertenbach/Ringleader</see>
    /// </remarks>
    public interface IContextualHttpClientFactory
    {
        /// <summary>
        /// Create a <typeparamref name="TClient"/> for a specified string context
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="handlerContext">A string-based context for primary handler resolution and partitioning</param>
        /// <returns></returns>
        TClient CreateClient<TClient>(string handlerContext);

        /// <summary>
        /// Create a <typeparamref name="TClient"/> for a specified <typeparamref name="TContext"/> context that can be resolved to a string
        /// </summary>
        /// <typeparam name="TClient">Typed <see cref="HttpClient"/></typeparam>
        /// <typeparam name="TContext">An object that represents the context for resolving and partitioning the primary handler</typeparam>
        /// <param name="handlerContext">A string-based context for primary handler resolution and partitioning</param>
        /// <param name="handlerContextResolver">A resolver function for identifying a string-based context - if not specified, ToString() will be called</param>
        /// <returns></returns>
        TClient CreateClient<TClient, TContext>(TContext handlerContext, Func<TContext, string>? handlerContextResolver = null);

        /// <summary>
        /// Create a named <see cref="HttpClient"/> for a specified string context
        /// </summary>
        /// <param name="handlerContext">A string-based context for primary handler resolution and partitioning</param>
        /// <returns></returns>
        HttpClient CreateClient(string clientName, string handlerContext);

        /// <summary>
        /// Create a named <see cref="HttpClient"/> for a specified <typeparamref name="TContext"/> context that can be resolved to a string
        /// </summary>
        /// <typeparam name="TContext">An object that represents the context for resolving and partitioning the primary handler</typeparam>
        /// <param name="handlerContext">A string-based context for primary handler resolution and partitioning</param>
        /// <param name="handlerContextResolver">A resolver function for identifying a string-based context - if not specified, ToString() will be called</param>
        /// <returns></returns>
        HttpClient CreateClient<TContext>(string name, TContext handlerContext, Func<TContext, string>? handlerContextResolver = null);
    }
}
