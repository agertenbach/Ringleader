using System;
using System.Net.Http;

namespace Ringleader
{
    public interface IPrimaryHandlerFactory
    {
        /// <summary>
        /// Given a handler identifier name (or empty string), return an appropriate HttpMessageHandler to be set as the primary handler
        /// </summary>
        /// <param name="name">Handler identifier name</param>
        /// <returns>An HttpMessageHandler configured based on the provided handler identifier</returns>
        HttpMessageHandler CreateHandler(string name);
    }
}
