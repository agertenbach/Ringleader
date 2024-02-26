namespace Ringleader.Shared
{
    /// <summary>
    /// Default context resolver using simple static string extensions
    /// </summary>
    public class DefaultHttpClientContextResolver : IHttpClientContextResolver
    {
        public string CreateContextName(string clientName, string handlerContext)
            => Static.CreateContextName(clientName, handlerContext);

        public string ParseClientName(string extendedContext)
            => Static.ParseClientName(extendedContext);

        public string ParseHandlerName(string extendedContext)
            => Static.ParseHandlerName(extendedContext);

        /// <summary>
        /// Default static extensions
        /// </summary>
        public static class Static
        {
            public const string Separator = "|:|";
            public static string CreateContextName(string name, string handlerContext)
                => name + Separator + handlerContext;
            public static string ParseClientName(string extendedContext)
                => string.IsNullOrWhiteSpace(extendedContext) ==  false && extendedContext.Contains(Separator) 
                    ? extendedContext.Split(Separator)[0]
                    : extendedContext; // if not compound, assumed is client name
            public static string ParseHandlerName(string extendedContext)
                => string.IsNullOrWhiteSpace(extendedContext) == false && extendedContext.Contains(Separator)
                    ? extendedContext.Split(Separator)[1]
                    : string.Empty; // if not compound, assumed is not contextual
        }
    }
    
}
