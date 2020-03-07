using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

using Ringleader;
using RingleaderExample;

namespace RingleaderTests
{
    /// <summary>
    /// Dummy handler factory
    /// </summary>
    public class TestingPrimaryHandlerFactory : IPrimaryHandlerFactory
    {
        private readonly MyCertificateProvider _myCertificateProvider;
        private readonly TestingEvaluationData _primaryHandlerReference;

        public TestingPrimaryHandlerFactory(MyCertificateProvider myCertificateProvider, TestingEvaluationData primaryHandlerReference)
        {
            _myCertificateProvider = myCertificateProvider ?? throw new ArgumentNullException(nameof(myCertificateProvider));
            _primaryHandlerReference = primaryHandlerReference ?? throw new ArgumentNullException(nameof(TestingEvaluationData));
        }
        public HttpMessageHandler CreateHandler(string name)
        {
            SocketsHttpHandler socketsHttpHandler = new SocketsHttpHandler();
            if (_myCertificateProvider.HasCertificate(name))
            {
                socketsHttpHandler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions();
                socketsHttpHandler.SslOptions.ClientCertificates = new X509Certificate2Collection(_myCertificateProvider.GetCertificate(name));
                _primaryHandlerReference.LastSetSslOptions = socketsHttpHandler.SslOptions; // Pass handler options for inspection in tests
                _primaryHandlerReference.CertificatesSet = name; // Flag certificate set
            }
            else
            {
                _primaryHandlerReference.CertificatesSet = string.Empty; // Flag no certificate set
            }
            
            return socketsHttpHandler;
        }
    }
}
