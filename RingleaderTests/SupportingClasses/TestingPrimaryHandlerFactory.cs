using System;
using System.Linq;
using System.Net.Http;

using Ringleader.HttpClientFactory;
using Ringleader.Shared;
using RingleaderTests.SupportingClasses;

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
        public HttpMessageHandler CreateHandler(TypedClientSignature client, string handlerContext)
        {
            SocketsHttpHandler socketsHttpHandler = new SocketsHttpHandler();

            _testEvaluation.ClientSignature = client;
            _testEvaluation.ContextSet = handlerContext; // Flag context set
            
            // Check that there is an expected signature match before setting context
            if (_testEvaluation.ExpectedSignature.Any(x => x == client) && _testEvaluation.ExpectedContext.Any(x => x == handlerContext))
            {
                _testEvaluation.ContextsMatched = true;   
            }

            return socketsHttpHandler;
        }
    }
}
