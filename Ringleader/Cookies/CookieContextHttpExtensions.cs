using Microsoft.Extensions.DependencyInjection.Extensions;
using Ringleader.Cookies;
using System.Collections.Generic;

namespace System.Net.Http
{
    public static class CookieContextHttpExtensions
    {
        /// <summary>
        /// Set a context key for an <see cref="HttpRequestMessage"/> used when the client has been registered with contextual cookie support
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static HttpRequestMessage SetCookieContext(this HttpRequestMessage request, string context)
        {
            request.Options.Remove(CookieContextDelegatingHandler.ContextKey, out _);
            request.Options.TryAdd(CookieContextDelegatingHandler.ContextKey, context);
            return request;
        }
    }
}
