using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;


namespace RingleaderExample
{
    /// <summary>
    /// A very simple example certificate provider to show how a IPrimaryHandlerFactory could resolve certificates for handlers
    /// </summary>
    public class MyCertificateProvider
    {
        private readonly Dictionary<string, X509Certificate2> _certificates = new Dictionary<string, X509Certificate2>();

        public X509Certificate2 GetCertificate(string certificateName)
        {
            return _certificates.TryGetValue(certificateName, out X509Certificate2 certificate) ? certificate : new X509Certificate2();
        }

        public bool HasCertificate(string certificateName)
        {
            return _certificates.ContainsKey(certificateName);
        }

        public bool SetCertificate(X509Certificate2 certificate, string certificateName)
        {
            return _certificates.TryAdd(certificateName, certificate);
        }
    }
}
