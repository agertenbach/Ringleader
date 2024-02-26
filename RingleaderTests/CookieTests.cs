using System;
using Xunit;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Ringleader.Cookies;
using System.Threading.Tasks;
using RingleaderTests.SupportingClasses;

namespace RingleaderTests
{
    public class CookieTest
    {
        private const string DUMMY_1 = "dummy1";
        private const string DUMMY_2 = "dummy2";
        private const string DUMMY_URL = "http://www.example.com";

        private readonly IHost _host;

        public CookieTest()
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
                });
        }

        [Fact]
        public async Task Match_Container_To_Basic_Request()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var typedClient = _host.Services.GetService<TestTypedClient>();
            
            var request = new HttpRequestMessage(HttpMethod.Get, DUMMY_URL);
            await typedClient.SendAsync(request, DUMMY_1, default);

            var cache = _host.Services.GetService<ICookieContainerCache>();
            var container = await cache.GetOrAdd(typeof(TestTypedClient).Name, DUMMY_1);
            Assert.Equal(container.GetCookieHeader(new Uri(DUMMY_URL)), handlerReference.CookieSet[0]);
        }

        [Fact]
        public async Task Confirm_Multiple_Request_Segmentation()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var typedClient = _host.Services.GetService<TestTypedClient>();

            var request1 = new HttpRequestMessage(HttpMethod.Get, DUMMY_URL);
            await typedClient.SendAsync(request1, DUMMY_1, default);

            var request2 = new HttpRequestMessage(HttpMethod.Get, DUMMY_URL);
            await typedClient.SendAsync(request2, DUMMY_2, default);

            var cache = _host.Services.GetService<ICookieContainerCache>();
            var container1 = await cache.GetOrAdd(typeof(TestTypedClient).Name, DUMMY_1, default);
            var container2 = await cache.GetOrAdd(typeof(TestTypedClient).Name, DUMMY_2, default);

            Assert.Equal(container1.GetCookieHeader(new Uri(DUMMY_URL)), handlerReference.CookieSet[0]);
            Assert.Equal(container2.GetCookieHeader(new Uri(DUMMY_URL)), handlerReference.CookieSet[1]);
        }

        [Fact]
        public async Task Confirm_Cookies_Applied()
        {
            var handlerReference = _host.Services.GetService<TestingEvaluationData>();
            handlerReference.Reset();

            var typedClient = _host.Services.GetService<TestTypedClient>();

            var request1 = new HttpRequestMessage(HttpMethod.Get, DUMMY_URL);
            await typedClient.SendAsync(request1, DUMMY_1, default);

            var request2 = new HttpRequestMessage(HttpMethod.Get, DUMMY_URL);
            await typedClient.SendAsync(request2, DUMMY_2, default);

            var request3 = new HttpRequestMessage(HttpMethod.Get, DUMMY_URL);
            request3.SetCookieContext(DUMMY_1);
            await typedClient.SendAsync(request3, DUMMY_1, default);

            var cache = _host.Services.GetService<ICookieContainerCache>();
            var container1 = await cache.GetOrAdd(typeof(TestTypedClient).Name, DUMMY_1, default);

            // We expect the 'test' cookie for DUMMY_1 to have been sent in the third request based on the first response set-cookie value
            Assert.Equal(handlerReference.CookieApplied[2], handlerReference.CookieSet[0]);

            // We expect the 'test' cookie current value for DUMMY_1 to be based on the third request response
            Assert.Equal(container1.GetCookieHeader(new Uri(DUMMY_URL)), handlerReference.CookieSet[2]);   
        }
    }
}
