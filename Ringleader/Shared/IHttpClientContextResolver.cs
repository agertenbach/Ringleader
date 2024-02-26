using System;
using System.Net.Http;

namespace Ringleader.Shared
{
    /// <summary>
    /// Resolve or create compound context identifiers for use with <see cref="IContextualHttpClientFactory"/>
    /// </summary>
    public interface IHttpClientContextResolver
    {
        /// <summary>
        /// Combine an <see cref="HttpClient"/> name and a string-based context for an <see cref="HttpMessageHandler"/> into a compound context identifier
        /// </summary>
        /// <param name="clientName"><see cref="HttpClient"/> name</param>
        /// <param name="handlerContext">Handler context</param>
        /// <returns>A compound context string</returns>
        string CreateContextName(string clientName, string handlerContext);

        /// <summary>
        /// Parse the <see cref="HttpClient"/> name from a compound context identifier
        /// </summary>
        /// <param name="extendedContext">Client and handler context string</param>
        /// <returns><see cref="HttpClient"/> name</returns>
        string ParseClientName(string extendedContext);

        /// <summary>
        /// Parse the <see cref="HttpMessageHandler"/> context name from a compound context identifier
        /// </summary>
        /// <param name="extendedContext">Client and handler context string</param>
        /// <returns><see cref="HttpMessageHandler"/> context name</returns>
        string ParseHandlerName(string extendedContext);
    }
    
}
