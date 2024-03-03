namespace System.Net
{
    public static class CookieContainerExtensions
    {
        /// <summary>
        /// Clone the contents of a <see cref="CookieContainer"/>
        /// </summary>
        /// <param name="container"></param>
        /// <returns>A copy of the original <see cref="CookieContainer"/></returns>
        public static CookieContainer Clone(this CookieContainer container)
        {
            var newContainer = new CookieContainer();
            newContainer.Add(container.GetAllCookies());
            return newContainer;
        }
    }
}
