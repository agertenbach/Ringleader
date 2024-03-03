namespace System.Net.Http
{
    public static class ContextualHttpClientFactoryExtensions
    {
        /// <summary>
        /// Create a <typeparamref name="TClient"/> for a specified <typeparamref name="TContext"/> context that can be resolved to a string
        /// </summary>
        /// <typeparam name="TClient">Typed <see cref="HttpClient"/></typeparam>
        /// <typeparam name="TContext">An object that represents the context for resolving and partitioning the primary handler</typeparam>
        /// <param name="handlerContext">A string-based context for primary handler resolution and partitioning</param>
        /// <param name="handlerContextResolver">A resolver function for identifying a string-based context - if not specified, ToString() will be called</param>
        /// <returns></returns>
        public static TClient CreateClient<TClient, TContext>(this IContextualHttpClientFactory factory, TContext handlerContext, Func<TContext, string>? handlerContextResolver = null)
            => factory.CreateClient<TClient>(
                handlerContextResolver is null
                    ? (handlerContext?.ToString() ?? string.Empty)
                    : handlerContextResolver.Invoke(handlerContext));

        /// <summary>
        /// Create a named <see cref="HttpClient"/> for a specified <typeparamref name="TContext"/> context that can be resolved to a string
        /// </summary>
        /// <typeparam name="TContext">An object that represents the context for resolving and partitioning the primary handler</typeparam>
        /// <param name="handlerContext">A string-based context for primary handler resolution and partitioning</param>
        /// <param name="handlerContextResolver">A resolver function for identifying a string-based context - if not specified, ToString() will be called</param>
        /// <returns></returns>
        public static HttpClient CreateClient<TContext>(this IContextualHttpClientFactory factory, string name, TContext handlerContext, Func<TContext, string>? handlerContextResolver = null)
            => factory.CreateClient(name,
                handlerContextResolver is null
                    ? (handlerContext?.ToString() ?? string.Empty)
                    : handlerContextResolver.Invoke(handlerContext));
    }
}
