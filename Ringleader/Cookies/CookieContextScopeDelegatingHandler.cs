using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ringleader.Cookies
{
    public class CookieContextScopeDelegatingHandler : DelegatingHandler
    {
        private readonly string _clientName;

        public CookieContextScopeDelegatingHandler(string clientName)
            => _clientName = clientName ?? throw new ArgumentNullException(nameof(clientName));

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Options.TryAdd(nameof(ICookieContainerCache), _clientName);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
