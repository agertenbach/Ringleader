using System;
using System.Net.Http;

namespace RingleaderExample
{
    /// <summary>
    /// The most basic example of a typed HttpClient
    /// </summary>
    public class MyHttpClient
    {
        public HttpClient Client { get; }

        public MyHttpClient(HttpClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }
    }
    
}
