using Ringleader.Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;


namespace RingleaderTests
{
    /// <summary>
    /// Data storage class for logging events during the HttpClientFactory process to evaluate in tests
    /// </summary>
    public class TestingEvaluationData
    {
        public string ContextSet { get; set; } = string.Empty;

        public TypedClientSignature ClientSignature { get; set; }

        public List<TypedClientSignature> ExpectedSignature { get; set; } = new();
        public List<string> ExpectedContext { get; set; } = new();

        public List<string> CookieApplied { get; set; } = new List<string>();
        public List<string> CookieSet { get; set; } = new List<string>();

        public bool DelegatingHandlerDidRun { get; set; } = false;

        public bool ContextsMatched { get; set; } = false;

        public void Reset()
        {
            ClientSignature = new(string.Empty);
            ContextSet = string.Empty;
            CookieApplied = new List<string>();
            CookieSet = new List<string>();
            DelegatingHandlerDidRun = false;
            ContextsMatched = false;
            ExpectedSignature.Clear();
            ExpectedContext.Clear();
        }
    }
}
