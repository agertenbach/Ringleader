using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ringleader.Cookies
{
    public class CookieContextDelegatingHandler : DelegatingHandler
    {
        private readonly ICookieContainerCache _cache;
        private readonly ILogger<CookieContextDelegatingHandler> _logger;

        public const string ContextKey = "CookieContext";
        private const string SetCookie = "Set-Cookie";
        private const string Cookie = "Cookie";

        public CookieContextDelegatingHandler(ICookieContainerCache cache, ILogger<CookieContextDelegatingHandler> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // If the request properties have identified a cookie context to use, invoke this handler's behavior
            bool shouldHandle = false;
            shouldHandle = request.Options.TryGetValue<string>(new HttpRequestOptionsKey<string>(ContextKey), out string? key);
            shouldHandle = request.Options.TryGetValue<string>(new HttpRequestOptionsKey<string>(nameof(ICookieContainerCache)), out string? clientName) && shouldHandle;
            
            if (shouldHandle && !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(clientName))
            {
                // Get or add the container and replace the 'Cookie' header
                var container = await _cache.GetOrAdd(clientName, key, cancellationToken);

                if(request.RequestUri is not null)
                {
                    var cookieHeader = container.GetCookieHeader(request.RequestUri);
                    _logger?.LogTrace("Setting cookie string ({length}) for context {context}", cookieHeader, key);
                    request.Headers.Remove(Cookie);
                    request.Headers.Add(Cookie, cookieHeader);
                }

                // Send the request
                var response = await base.SendAsync(request, cancellationToken);

                // Process received cookies and update the container cache
                ProcessReceivedCookies(response, container, _logger);
                await _cache.AddOrUpdate(clientName, key, container, cancellationToken);

                // Return the response
                return response;
            }
            
            return await base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Process received cookies
        /// </summary>
        /// <remarks>
        /// Based on <see href="https://source.dot.net/#System.Net.Http/System/Net/Http/SocketsHttpHandler/CookieHelper.cs"/>
        /// </remarks>
        /// <param name="response"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="logger"></param>
        public static void ProcessReceivedCookies(HttpResponseMessage response, CookieContainer cookieContainer, ILogger? logger)
        {
            if (response.Headers.TryGetValues(SetCookie, out IEnumerable<string>? values))
            {
                var valuesArray = values is not null ? (string[])values : Array.Empty<string>();
                Uri? requestUri = response.RequestMessage?.RequestUri;
                if (requestUri is not null)
                {
                    for (int i = 0; i < valuesArray.Length; i++)
                    {
                        try
                        {
                            cookieContainer.SetCookies(requestUri, valuesArray[i]);
                        }
                        catch (CookieException)
                        {
                            logger?.LogTrace("Invalid Set-Cookie '{value}' ignored", valuesArray[i]);
                        }
                    }
                }
            }
        }
    }
}
