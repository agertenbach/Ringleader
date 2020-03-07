using System;

namespace Ringleader
{
    public interface IHttpClientHandlerRegistry
    {
        /// <summary>
        /// Identifies to the registry that a handler identifier and a typed client are
        /// associated and should have a unique entry in the handler pool.
        /// </summary>
        /// <param name="typedHttpClient">Typed HttpClient</param>
        /// <param name="handlerIdentifier">Identifier for the handler</param>
        /// <returns>The compound name string that should be passed through the DefaultHttpClientFactory</returns>
        string RegisterClientHandlerKey(Type typedHttpClient, string handlerIdentifier);

        /// <summary>
        /// Return the typed client name from a compound key to allow the DefaultHttpClientFactory to identify
        /// the correct HttpClientFactoryOptions to resolve
        /// </summary>
        /// <param name="clientHandlerKey">Compound name string produced by the registration method</param>
        /// <returns>Typed HttpClient name</returns>
        string GetClientNameFromKey(string clientHandlerKey);

        /// <summary>
        /// Return the handler identifier from a compound key to allow the IPrimaryHandlerBuilder to identify
        /// the correct handler settings to use when instantiating the primary handler
        /// </summary>
        /// <param name="clientHandlerKey">Compound name string produced by the registration method</param>
        /// <returns>Handler identifier</returns>
        string GetHandlerIdentifierFromKey(string clientHandlerKey);
    }
}
