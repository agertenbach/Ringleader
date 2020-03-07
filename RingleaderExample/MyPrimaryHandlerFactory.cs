using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

using Ringleader;

namespace RingleaderExample
{
    /// <summary>
    /// An example primary handler factory. Your implementation should return an HttpMessageHandler with whatever configuration is required for the
    /// identifier that is passed through, including any certificates that are appropriate. This example uses an in-memory certificate provider, but
    /// these could be coming from anywhere.
    /// </summary>
    public class MyPrimaryHandlerFactory : IPrimaryHandlerFactory
    {
        private readonly MyCertificateProvider _myCertificateProvider;

        public MyPrimaryHandlerFactory(MyCertificateProvider myCertificateProvider)
        {
            _myCertificateProvider = myCertificateProvider ?? throw new ArgumentNullException(nameof(myCertificateProvider));
        }
        public HttpMessageHandler CreateHandler(string name)
        {
            SocketsHttpHandler socketsHttpHandler = new SocketsHttpHandler();
            if (_myCertificateProvider.HasCertificate(name))
            {
                socketsHttpHandler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions();
                socketsHttpHandler.SslOptions.ClientCertificates = new X509Certificate2Collection(_myCertificateProvider.GetCertificate(name));
            }

            return socketsHttpHandler;
        }
    }
}
