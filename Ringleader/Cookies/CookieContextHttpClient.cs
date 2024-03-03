using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Ringleader.Cookies
{
    /// <summary>
    /// Convenience typed client for <see cref="IHttpClientFactory"/> that wraps selected methods from <see cref="HttpClient"/> to enable durable, per-request context for cookie containers
    /// </summary>
    public class CookieContextHttpClient : IDisposable
    {
        public Version DefaultRequestVersion { get; set; } = HttpVersion.Version11;
        public HttpVersionPolicy DefaultVersionPolicy { get; set; } = HttpVersionPolicy.RequestVersionOrLower;

        private const HttpCompletionOption DefaultCompletionOption = HttpCompletionOption.ResponseContentRead;

        private readonly HttpClient _baseClient;

        public CookieContextHttpClient(HttpClient baseClient)
        {
            _baseClient = baseClient;
        }

        public void CancelPendingRequests()
        {
            _baseClient.CancelPendingRequests();
        }

        #region REST Send Overloads


        public Task<HttpResponseMessage> GetAsync(string? requestUri, string cookieContext) =>
            GetAsync(CreateUri(requestUri), cookieContext);

        public Task<HttpResponseMessage> GetAsync(Uri? requestUri, string cookieContext) =>
            GetAsync(requestUri, cookieContext, DefaultCompletionOption);

        public Task<HttpResponseMessage> GetAsync(string? requestUri, string cookieContext, HttpCompletionOption completionOption) =>
            GetAsync(CreateUri(requestUri), cookieContext, completionOption);

        public Task<HttpResponseMessage> GetAsync(Uri? requestUri, string cookieContext, HttpCompletionOption completionOption) =>
            GetAsync(requestUri, cookieContext, completionOption, CancellationToken.None);

        public Task<HttpResponseMessage> GetAsync(string? requestUri, string cookieContext, CancellationToken cancellationToken) =>
            GetAsync(CreateUri(requestUri), cookieContext, cancellationToken);

        public Task<HttpResponseMessage> GetAsync(Uri? requestUri, string cookieContext, CancellationToken cancellationToken) =>
            GetAsync(requestUri, cookieContext, DefaultCompletionOption, cancellationToken);

        public Task<HttpResponseMessage> GetAsync(string? requestUri, string cookieContext, HttpCompletionOption completionOption, CancellationToken cancellationToken) =>
            GetAsync(CreateUri(requestUri), cookieContext, completionOption, cancellationToken);

        public Task<HttpResponseMessage> GetAsync(Uri? requestUri, string cookieContext, HttpCompletionOption completionOption, CancellationToken cancellationToken) =>
            SendAsync(CreateRequestMessage(HttpMethod.Get, requestUri), cookieContext, completionOption, cancellationToken);

        public Task<HttpResponseMessage> PostAsync(string? requestUri, string cookieContext, HttpContent? content) =>
            PostAsync(CreateUri(requestUri), cookieContext, content);

        public Task<HttpResponseMessage> PostAsync(Uri? requestUri, string cookieContext, HttpContent? content) =>
            PostAsync(requestUri, cookieContext, content, CancellationToken.None);

        public Task<HttpResponseMessage> PostAsync(string? requestUri, string cookieContext, HttpContent? content, CancellationToken cancellationToken) =>
            PostAsync(CreateUri(requestUri), cookieContext, content, cancellationToken);

        public Task<HttpResponseMessage> PostAsync(Uri? requestUri, string cookieContext, HttpContent? content, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = CreateRequestMessage(HttpMethod.Post, requestUri);
            request.Content = content;
            return SendAsync(request, cookieContext, cancellationToken);
        }

        public Task<HttpResponseMessage> PutAsync(string? requestUri, string cookieContext, HttpContent? content) =>
            PutAsync(CreateUri(requestUri), cookieContext, content);

        public Task<HttpResponseMessage> PutAsync(Uri? requestUri, string cookieContext, HttpContent? content) =>
            PutAsync(requestUri, cookieContext, content, CancellationToken.None);

        public Task<HttpResponseMessage> PutAsync(string? requestUri, string cookieContext, HttpContent? content, CancellationToken cancellationToken) =>
            PutAsync(CreateUri(requestUri), cookieContext, content, cancellationToken);

        public Task<HttpResponseMessage> PutAsync(Uri? requestUri, string cookieContext, HttpContent? content, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = CreateRequestMessage(HttpMethod.Put, requestUri);
            request.Content = content;
            return SendAsync(request, cookieContext, cancellationToken);
        }

        public Task<HttpResponseMessage> PatchAsync(string? requestUri, string cookieContext, HttpContent? content) =>
            PatchAsync(CreateUri(requestUri), cookieContext, content);

        public Task<HttpResponseMessage> PatchAsync(Uri? requestUri, string cookieContext, HttpContent? content) =>
            PatchAsync(requestUri, cookieContext, content, CancellationToken.None);

        public Task<HttpResponseMessage> PatchAsync(string? requestUri, string cookieContext, HttpContent? content, CancellationToken cancellationToken) =>
            PatchAsync(CreateUri(requestUri), cookieContext, content, cancellationToken);

        public Task<HttpResponseMessage> PatchAsync(Uri? requestUri, string cookieContext, HttpContent? content, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = CreateRequestMessage(HttpMethod.Patch, requestUri);
            request.Content = content;
            return SendAsync(request, cookieContext, cancellationToken);
        }

        public Task<HttpResponseMessage> DeleteAsync(string? requestUri, string cookieContext) =>
            DeleteAsync(CreateUri(requestUri), cookieContext);

        public Task<HttpResponseMessage> DeleteAsync(Uri? requestUri, string cookieContext) =>
            DeleteAsync(requestUri, cookieContext, CancellationToken.None);

        public Task<HttpResponseMessage> DeleteAsync(string? requestUri, string cookieContext, CancellationToken cancellationToken) =>
            DeleteAsync(CreateUri(requestUri), cookieContext, cancellationToken);

        public Task<HttpResponseMessage> DeleteAsync(Uri? requestUri, string cookieContext, CancellationToken cancellationToken) =>
            SendAsync(CreateRequestMessage(HttpMethod.Delete, requestUri), cookieContext, cancellationToken);

        #endregion REST Send Overloads

        #region Advanced Send Overloads

        [UnsupportedOSPlatform("browser")]
        public HttpResponseMessage Send(HttpRequestMessage request, string cookieContext) =>
            Send(request, cookieContext, DefaultCompletionOption, cancellationToken: default);

        [UnsupportedOSPlatform("browser")]
        public HttpResponseMessage Send(HttpRequestMessage request, string cookieContext, HttpCompletionOption completionOption) =>
            Send(request, cookieContext, completionOption, cancellationToken: default);

        [UnsupportedOSPlatform("browser")]
        public HttpResponseMessage Send(HttpRequestMessage request, string cookieContext, CancellationToken cancellationToken) =>
            Send(request, cookieContext, DefaultCompletionOption, cancellationToken);

        [UnsupportedOSPlatform("browser")]
        public HttpResponseMessage Send(HttpRequestMessage request, string cookieContext, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            request.SetCookieContext(cookieContext);
            return _baseClient.Send(request, completionOption, cancellationToken);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, string cookieContext) =>
            SendAsync(request, cookieContext, DefaultCompletionOption, CancellationToken.None);

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, string cookieContext, CancellationToken cancellationToken) =>
            SendAsync(request, cookieContext, DefaultCompletionOption, cancellationToken);

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, string cookieContext, HttpCompletionOption completionOption) =>
            SendAsync(request, cookieContext, completionOption, CancellationToken.None);

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, string cookieContext, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            request.SetCookieContext(cookieContext);
            return _baseClient.SendAsync(request, completionOption, cancellationToken);
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _baseClient.Dispose();
        }

        #endregion

        #region Private Helpers

        private static Uri? CreateUri(string? uri) =>
            string.IsNullOrEmpty(uri) ? null : new Uri(uri, UriKind.RelativeOrAbsolute);

        private HttpRequestMessage CreateRequestMessage(HttpMethod method, Uri? uri)
            => new HttpRequestMessage(method, uri) { Version = DefaultRequestVersion, VersionPolicy = DefaultVersionPolicy };

        #endregion Private Helpers
    }
}