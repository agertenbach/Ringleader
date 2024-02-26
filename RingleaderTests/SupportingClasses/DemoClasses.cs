using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ringleader.Cookies;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace RingleaderTests.SupportingClasses
{
    public class TestTypedClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TestTypedClient> _logger;

        public TestTypedClient(HttpClient httpClient, ILogger<TestTypedClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, string cookieContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("This request was sent with the example typed client using the cookie container for {context}", cookieContext);
            request.SetCookieContext(cookieContext);
            return _httpClient.SendAsync(request, cancellationToken);
        }
    }
}
