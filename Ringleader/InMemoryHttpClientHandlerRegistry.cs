using System;
using System.Collections.Generic; 

namespace Ringleader
{
    public class InMemoryHttpClientHandlerRegistry : IHttpClientHandlerRegistry
    {
        private readonly HashSet<string> _clientHandlerRegistry = new HashSet<string>();

        /// <summary>
        /// Identifies to the registry that a handler identifier and a typed client are
        /// associated and should have a unique entry in the handler pool.
        /// </summary>
        /// <param name="typedHttpClient">Typed HttpClient</param>
        /// <param name="handlerIdentifier">Identifier for the handler</param>
        /// <returns>The compound name string that should be passed through the DefaultHttpClientFactory</returns>
        public string RegisterClientHandlerKey(Type typedHttpClient, string handlerIdentifier)
        {
            string key = $"{typedHttpClient.Name}*{handlerIdentifier}";
            _clientHandlerRegistry.Add(key);
            return key;
        }

        /// <summary>
        /// Return the typed client name from a compound key to allow the DefaultHttpClientFactory to identify
        /// the correct HttpClientFactoryOptions to resolve
        /// </summary>
        /// <param name="clientHandlerKey">Compound name string produced by the registration method</param>
        /// <returns>Typed HttpClient name</returns>
        public string GetClientNameFromKey(string clientHandlerKey)
        {
            if (_clientHandlerRegistry.Contains(clientHandlerKey))
            {
                string[] splitKey = clientHandlerKey.Split('*');
                return splitKey.Length < 1 ? string.Empty : splitKey[0];
            }
            return clientHandlerKey;
        }

        /// <summary>
        /// Return the handler identifier from a compound key to allow the IPrimaryHandlerBuilder to identify
        /// the correct handler settings to use when instantiating the primary handler
        /// </summary>
        /// <param name="clientHandlerKey">Compound name string produced by the registration method</param>
        /// <returns>Handler identifier</returns>
        public string GetHandlerIdentifierFromKey(string clientHandlerKey)
        {
            if (_clientHandlerRegistry.Contains(clientHandlerKey))
            {
                string[] splitKey = clientHandlerKey.Split('*');
                return splitKey.Length < 1 ? string.Empty : splitKey[1];
            }
            return string.Empty;
        }
    }
}
