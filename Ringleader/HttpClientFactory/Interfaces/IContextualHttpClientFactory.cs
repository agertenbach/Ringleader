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
        /// Create a named <see cref="HttpClient"/> for a specified string context
        /// </summary>
        /// <param name="handlerContext">A string-based context for primary handler resolution and partitioning</param>
        /// <returns></returns>
        HttpClient CreateClient(string clientName, string handlerContext);
    }
}
