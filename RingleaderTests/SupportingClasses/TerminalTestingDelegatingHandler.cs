using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RingleaderTests
{
    /// <summary>
    /// Updates testing evaluation singleton to show that the delegating handler pipeline for the typed client was applied and executed.
    /// Throws the HttpRequestMessage into the void and returns a 200 OK response.
    /// </summary>
    public class TerminalTestingDelegatingHandler : DelegatingHandler
    {
        private readonly TestingEvaluationData _testingEvaluationData;

        public TerminalTestingDelegatingHandler(TestingEvaluationData testingEvaluationData)
        {
            _testingEvaluationData = testingEvaluationData ?? throw new ArgumentNullException(nameof(testingEvaluationData));
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _testingEvaluationData.DelegatingHandlerDidRun = true;
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.RequestMessage = request;
            response.Headers.Remove("Set-Cookie");
            if(request.Headers.TryGetValues("Cookie", out var cookies))
            {
                _testingEvaluationData.CookieApplied.Add(cookies.First());
            }
            else
            {
                _testingEvaluationData.CookieApplied.Add(string.Empty);
            }
            
            _testingEvaluationData.CookieSet.Add("test=" + Guid.NewGuid().ToString());
            response.Headers.Add("Set-Cookie", _testingEvaluationData.CookieSet);
            return await Task.FromResult<HttpResponseMessage>(response);
        }
    }
}
