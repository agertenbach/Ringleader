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
        public string LastSetContext { get; set; } = string.Empty;

        public string ContextSet { get; set; } = string.Empty;

        public List<string> CookieApplied { get; set; } = new List<string>();
        public List<string> CookieSet { get; set; } = new List<string>();

        public bool DelegatingHandlerDidRun { get; set; } = false;

        public void Reset()
        {
            LastSetContext = string.Empty;
            ContextSet = string.Empty;
            CookieApplied = new List<string>();
            CookieSet = new List<string>();
            DelegatingHandlerDidRun = false;
        }
    }
}
