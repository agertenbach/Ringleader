using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace RingleaderExample
{
    /// <summary>
    /// A dummy delegating handler that is attached to MyHttpClient and does nothing. It will get used both in contextual requests through the
    /// HttpContextualClientFactory or through regular resolution of a typed HttpClient using HttpClientFactory normally.
    /// </summary>
    public class MyDelegatingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
