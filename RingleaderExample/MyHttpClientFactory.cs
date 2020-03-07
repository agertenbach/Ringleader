using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Ringleader;

namespace RingleaderExample
{
    /// <summary>
    /// An example HttpClientFactory set up to provide a typed HttpClient with a contextually appropriate primary handler from the pool.
    /// In this example, the context -> handler identifier is a direct and transparent passthrough of the original string, but the TContext
    /// can be any type of class that your factory can translate to a handler identifier string using the ResolveIdentifierByContext method
    /// that you override, such as extracting some portion of a Uri or a POCO/DTO of your design.
    /// </summary>
    public class MyHttpClientFactory : HttpContextualClientFactory<MyHttpClient, string>
    {
        public MyHttpClientFactory(IHttpClientHandlerRegistry httpClientHandlerRegistry, IHttpClientFactory httpClientFactory, ILogger<MyHttpClientFactory> logger)
            : base(httpClientHandlerRegistry, httpClientFactory, logger)
        {
        }

        public override MyHttpClient GetTypedClientByContext(string context)
        {
            var httpClient = CreateClient(context);
            return new MyHttpClient(httpClient);
        }

        protected override string ResolveIdentifierByContext(string context)
        {
            return context;
        }
    }
}
