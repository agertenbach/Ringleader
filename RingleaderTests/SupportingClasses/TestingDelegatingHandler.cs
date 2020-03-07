using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace RingleaderTests
{
    /// <summary>
    /// Updates testing evaluation singleton to show that the delegating handler pipeline for the typed client was applied and executed.
    /// Throws the HttpRequestMessage into the void and returns a 200 OK response.
    /// </summary>
    public class TestingDelegatingHandler : DelegatingHandler
    {
        private readonly TestingEvaluationData _testingEvaluationData;

        public TestingDelegatingHandler(TestingEvaluationData testingEvaluationData)
        {
            _testingEvaluationData = testingEvaluationData ?? throw new ArgumentNullException(nameof(testingEvaluationData));
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _testingEvaluationData.DelegatingHandlerDidRun = true;
            return await Task.FromResult<HttpResponseMessage>(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
        }
    }
}
