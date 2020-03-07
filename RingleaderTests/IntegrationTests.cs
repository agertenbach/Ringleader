using System;
using Xunit;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;

using Ringleader;
using RingleaderExample;

namespace RingleaderTests
{
    public class IntegrationTest
    {
        private const string DUMMY_1 = "dummy1";
        private const string DUMMY_2 = "dummy2";
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
                    // Add Ringleader with our contextual factory and handler factory
                    services.AddRingleader()
                        .WithContextualClientFactory<MyHttpClientFactory, MyHttpClient, string>()
                        .WithPrimaryHandlerFactory<TestingPrimaryHandlerFactory>()
                        .Build();

                    // Configure a sample repository of certificates that our primary handler factory can use
                    var myCertificateProvider = new MyCertificateProvider();
                    myCertificateProvider.SetCertificate(new X509Certificate2("./DummyCertificates/dummy_cert1.pem"), DUMMY_1);
                    myCertificateProvider.SetCertificate(new X509Certificate2("./DummyCertificates/dummy_cert2.pem"), DUMMY_2);
                    services.AddSingleton<MyCertificateProvider>(myCertificateProvider);

                    // Add our testing evaluation singleton to monitor handler construction and execution of the delegating handler pipeline
                    services.AddSingleton<TestingEvaluationData>();

                    // Add our dummy HttpClient and attach a delegating handler that can log execution for testing
                    services.AddTransient<TestingDelegatingHandler>();
                    services.AddHttpClient<MyHttpClient>()
                        .AddHttpMessageHandler<TestingDelegatingHandler>();

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

            var factory = _host.Services.GetService<IHttpContextualClientFactory<MyHttpClient, string>>();
            var typedClient = factory.GetTypedClientByContext(DUMMY_1);
            typedClient.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL));

            Assert.Equal(DUMMY_1, handlerReference.CertificatesSet); // Certificate should be applied to the handler
        }

       /// <summary>
       /// When we request a MyHttpClient using the service container directly via DefaultHttpClientFactory, the underlying handler should show no certificates
       /// </summary>
        [Fact]
        public void Regular_Request_Returns_Standard_Handler()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var typedClient = _host.Services.GetService<MyHttpClient>();
            typedClient.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL));

            Assert.Equal(string.Empty, handlerReference.CertificatesSet); // No certificates should be applied to the handler
        }

        /// <summary>
        /// A contextual factory request should create a separate handler in the pool that does not interfere with any subsequent normal requests through DefaultHttpClientFactory
        /// </summary>
        [Fact]
        public void Contextual_Request_Should_Not_Break_Regular_Request()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var factory = _host.Services.GetService<IHttpContextualClientFactory<MyHttpClient, string>>();
            var typedClientContextual = factory.GetTypedClientByContext(DUMMY_1);
            typedClientContextual.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL));
            string contextualSet = handlerReference.CertificatesSet;
            handlerReference.Reset();

            var typedClientRegular = _host.Services.GetService<MyHttpClient>();
            typedClientRegular.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL));
            string regularSet = handlerReference.CertificatesSet;

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

            var typedClientRegular = _host.Services.GetService<MyHttpClient>();
            typedClientRegular.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL));
            string regularSet = handlerReference.CertificatesSet;
            handlerReference.Reset();

            var factory = _host.Services.GetService<IHttpContextualClientFactory<MyHttpClient, string>>();
            var typedClientContextual = factory.GetTypedClientByContext(DUMMY_1);
            typedClientContextual.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL));
            string contextualSet = handlerReference.CertificatesSet;

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
            var factory = _host.Services.GetService<IHttpContextualClientFactory<MyHttpClient, string>>();

            var firstClientContextual = factory.GetTypedClientByContext(DUMMY_1);
            firstClientContextual.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL));
            string firstSet = handlerReference.CertificatesSet;
            handlerReference.Reset();

            var secondClientContextual = factory.GetTypedClientByContext(DUMMY_2);
            secondClientContextual.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL));
            string secondSet = handlerReference.CertificatesSet;

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

            var factory = _host.Services.GetService<IHttpContextualClientFactory<MyHttpClient, string>>();
            var typedClient = factory.GetTypedClientByContext(DUMMY_1);
            typedClient.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL));

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

            var typedClientRegular = _host.Services.GetService<MyHttpClient>();
            typedClientRegular.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL));

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

            var factory = _host.Services.GetService<IHttpContextualClientFactory<MyHttpClient, string>>();
            var typedClientContextual = factory.GetTypedClientByContext(DUMMY_1);
            typedClientContextual.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL));
            handlerReference.Reset();

            var typedClientRegular = _host.Services.GetService<MyHttpClient>();
            typedClientRegular.Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, DUMMY_URL));

            Assert.True(handlerReference.DelegatingHandlerDidRun);
        }
    }
}
