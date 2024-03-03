using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace WebExample
{
    /// <summary>
    /// Throws the HttpRequestMessage into the void and returns a 200 OK response with random cookie data
    /// </summary>
    public class TerminalCookieHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.RequestMessage = request;
            response.Headers.Remove("Set-Cookie");
            response.Headers.Add("Set-Cookie", "test=" + Guid.NewGuid().ToString());
            return await Task.FromResult<HttpResponseMessage>(response);
        }
    }
}
