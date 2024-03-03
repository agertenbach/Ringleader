using System;
using Xunit;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using Ringleader;
using RingleaderTests.SupportingClasses;
using Ringleader.Cookies;
using System.Threading.Tasks;

namespace RingleaderTests
{
    public class IntegrationTest
    {
        public const string DUMMY_1 = "dummy1";
        public const string DUMMY_2 = "dummy2";
        private const string DUMMY_URL = "http://www.example.com";

        private readonly IHost _host;

        public IntegrationTest()
        {
            var hostBuilder = SetupHost();
            _host =  hostBuilder.Start();
        }

        public static IHostBuilder SetupHost()
        {
            return new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    // Add our testing evaluation singleton to monitor handler construction and execution of the delegating handler pipeline
                    services.AddSingleton<TestingEvaluationData>();

                    // Add our dummy HttpClient and attach a delegating handler that can log execution for testing
                    services.AddTransient<TerminalTestingDelegatingHandler>();
                    services.AddHttpClient<TestTypedClient>()
                        .UseContextualCookies()
                        .AddHttpMessageHandler<TerminalTestingDelegatingHandler>();

                    // Add contextual factory support
                    services.AddContextualHttpClientFactory<TestingPrimaryHandlerFactory>();
                 });
        }

        /// <summary>
        /// When we request a MyHttpClient instance through our contextual factory, the underlying handler should show a certificate attached based on the context
        /// </summary>
        [Fact]
        public void Contextual_Request_Returns_Custom_Handler()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var factory = _host.Services.GetService<IContextualHttpClientFactory>();
            var typedClient = factory.CreateClient<TestTypedClient>(DUMMY_1);
            typedClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), string.Empty, default);

            Assert.Equal(DUMMY_1, handlerReference.ContextSet); // Context should be applied to the handler
        }

       /// <summary>
       /// When we request a MyHttpClient using the service container directly via DefaultHttpClientFactory, the underlying handler should show no context
       /// </summary>
        [Fact]
        public void Regular_Request_Returns_Standard_Handler()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var typedClient = _host.Services.GetService<TestTypedClient>();
            typedClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), string.Empty, default);

            Assert.Equal(string.Empty, handlerReference.ContextSet); // No context should be applied to the handler
        }

        /// <summary>
        /// A contextual factory request should create a separate handler in the pool that does not interfere with any subsequent normal requests through DefaultHttpClientFactory
        /// </summary>
        [Fact]
        public void Contextual_Request_Should_Not_Break_Regular_Request()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var factory = _host.Services.GetService<IContextualHttpClientFactory>();
            var typedClientContextual = factory.CreateClient<TestTypedClient>(DUMMY_1);
            typedClientContextual.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), string.Empty, default);
            string contextualSet = handlerReference.ContextSet;
            handlerReference.Reset();

            var typedClientRegular = _host.Services.GetService<TestTypedClient>();
            typedClientRegular.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), string.Empty, default);
            string regularSet = handlerReference.ContextSet;

            Assert.True(regularSet == string.Empty && contextualSet == DUMMY_1); // Contextual call handlers should not interfere with subsequent regular call handlers
        }

        /// <summary>
        /// A normal request through DefaultHttpClientFactory should create a separate handler in the pool that does not interfere with any subsequent contextual factory requests 
        /// </summary>
        [Fact]
        public void Regular_Request_Should_Not_Break_Contextual_Request()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var typedClientRegular = _host.Services.GetService<TestTypedClient>();
            typedClientRegular.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), string.Empty, default);
            string regularSet = handlerReference.ContextSet;
            handlerReference.Reset();

            var factory = _host.Services.GetService<IContextualHttpClientFactory>();
            var typedClientContextual = factory.CreateClient<TestTypedClient>(DUMMY_1);
            typedClientContextual.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), string.Empty, default);
            string contextualSet = handlerReference.ContextSet;

            Assert.True(regularSet == string.Empty && contextualSet == DUMMY_1); // Regular call handlers should not interfere with subsequent contextual call handlers
        }

        /// <summary>
        /// A contextual factory request should create a separate handler in the pool that does not interfere with any subsequent different contextual factory requests 
        /// </summary>
        [Fact]
        public void Different_Contextual_Requests_Use_Different_Handlers()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();
            var factory = _host.Services.GetService<IContextualHttpClientFactory>();

            var firstClientContextual = factory.CreateClient<TestTypedClient>(DUMMY_1);
            firstClientContextual.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), string.Empty, default);
            string firstSet = handlerReference.ContextSet;
            handlerReference.Reset();

            var secondClientContextual = factory.CreateClient<TestTypedClient>(DUMMY_2);
            secondClientContextual.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), string.Empty, default);
            string secondSet = handlerReference.ContextSet;

            Assert.True(firstSet == DUMMY_1 && secondSet == DUMMY_2); // Each contextual request created and used a different handler for the pool
        }

        /// <summary>
        /// When a contextual factory request is made, it should still apply the delegating handler pipeline defined for the HttpClient during service registration
        /// </summary>
        [Fact]
        public void Contextual_Request_Should_Apply_Client_Handler_Pipeline()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var factory = _host.Services.GetService<IContextualHttpClientFactory>();
            var typedClient = factory.CreateClient<TestTypedClient>(DUMMY_1);
            typedClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), string.Empty, default);

            Assert.True(handlerReference.DelegatingHandlerDidRun);
        }

        /// <summary>
        /// When a default request is made, it should still apply the delegating handler pipeline defined for the HttpClient during service registration
        /// </summary>
        [Fact]
        public void Regular_Request_Should_Apply_Client_Handler_Pipeline()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var typedClientRegular = _host.Services.GetService<TestTypedClient>();
            typedClientRegular.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), string.Empty, default);

            Assert.True(handlerReference.DelegatingHandlerDidRun);
        }

        /// <summary>
        /// When a default request is made, it should still apply the delegating handler pipeline defined for the HttpClient during service registration
        /// even when a contextual handler has previously been created
        /// </summary>
        [Fact]
        public void Subsequent_Regular_Request_Should_Apply_Client_Handler_Pipeline()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var factory = _host.Services.GetService<IContextualHttpClientFactory>();
            var typedClientContextual = factory.CreateClient<TestTypedClient>(DUMMY_1);
            typedClientContextual.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), string.Empty, default);
            handlerReference.Reset();

            var typedClientRegular = _host.Services.GetService<TestTypedClient>();
            typedClientRegular.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), string.Empty, default);

            Assert.True(handlerReference.DelegatingHandlerDidRun);
        }

        [Fact]
        public async Task Cookies_And_Handler_No_Conflict()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var factory = _host.Services.GetService<IContextualHttpClientFactory>();
            var typedClientContextual = factory.CreateClient<TestTypedClient>(DUMMY_1);
            await typedClientContextual.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), DUMMY_1, default);
            string contextualSet = handlerReference.ContextSet;
            handlerReference.Reset();

            var typedClientRegular = _host.Services.GetService<TestTypedClient>();
            await typedClientRegular.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL), DUMMY_2, default);
            string regularSet = handlerReference.ContextSet;

            var cache = _host.Services.GetRequiredService<ICookieContainerCache>();
            var cookies1 = await cache.GetOrAdd<TestTypedClient>(DUMMY_1, default);
            var cookies2 = await cache.GetOrAdd<TestTypedClient>(DUMMY_2, default);

            Assert.True(regularSet == string.Empty && contextualSet == DUMMY_1); // Contextual call handlers should not interfere with subsequent regular call handlers
            Assert.Equal(1, cookies1.Count);
            Assert.Equal(1, cookies2.Count);
        }
    }
}
