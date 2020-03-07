using System;

namespace Ringleader
{
    public interface IHttpContextualClientFactory<TClient, TContext>
    {
        /// <summary>
        /// Return a typed HttpClient with an underlying HttpClient that uses the appropriate primary handler and settings based on a specified context
        /// </summary>
        /// <param name="context">A class, POCO, or DTO that can be translated into a string identifier to select the appropriate primary handler</param>
        /// <returns>A typed HttpClient</returns>
        TClient GetTypedClientByContext(TContext context);
    }
}
