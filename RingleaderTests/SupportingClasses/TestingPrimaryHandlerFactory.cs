using System;
using System.Net.Http;

using Ringleader.HttpClientFactory;

namespace RingleaderTests
{
    /// <summary>
    /// Dummy handler factory
    /// </summary>
    public class TestingPrimaryHandlerFactory : IPrimaryHandlerFactory
    {
        private readonly TestingEvaluationData _testEvaluation;

        public TestingPrimaryHandlerFactory(TestingEvaluationData testEvaluation)
        {
            _testEvaluation = testEvaluation ?? throw new ArgumentNullException(nameof(testEvaluation));
        }
        public HttpMessageHandler CreateHandler(string clientName, string handlerContext)
        {
            SocketsHttpHandler socketsHttpHandler = new SocketsHttpHandler();
            if (handlerContext == IntegrationTest.DUMMY_1 || handlerContext == IntegrationTest.DUMMY_2)
            {

                _testEvaluation.ContextSet = handlerContext; // Flag context set
            }
            else
            {
                _testEvaluation.ContextSet = string.Empty; // Flag no context set
            }
            
            return socketsHttpHandler;
        }
    }
}
