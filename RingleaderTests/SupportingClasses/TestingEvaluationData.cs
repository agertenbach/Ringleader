using System;
using System.Net.Http;
using System.Net.Security;


namespace RingleaderTests
{
    /// <summary>
    /// Data storage class for logging events during the HttpClientFactory process to evaluate in tests
    /// </summary>
    public class TestingEvaluationData
    {
        public SslClientAuthenticationOptions LastSetSslOptions { get; set; } = new SslClientAuthenticationOptions();

        public string CertificatesSet { get; set; } = string.Empty;

        public bool DelegatingHandlerDidRun { get; set; } = false;

        public void Reset()
        {
            LastSetSslOptions = null;
            CertificatesSet = string.Empty;
            DelegatingHandlerDidRun = false;
        }
    }
}
