using System;
using System.Net.Http;

namespace Ringleader.HttpClientFactory
{
    public interface IPrimaryHandlerFactory
    {
        /// <summary>
        /// Optionally create a custom primary <see cref="HttpMessageHandler"/> based on a supplied <see cref="HttpClient"/> name and handler context
        /// </summary>
        /// <param name="clientName"><see cref="HttpClient"/> name</param>
        /// <param name="handlerContext">Handler context for the <see cref="HttpClient"/></param>
        /// <returns>An <see cref="HttpMessageHandler"/> configured based on the context, or <see langword="null"/> to use the default handler</returns>
        HttpMessageHandler? CreateHandler(string clientName, string handlerContext);
    }
}
