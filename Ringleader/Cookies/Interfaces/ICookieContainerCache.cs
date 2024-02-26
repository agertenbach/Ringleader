using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ringleader.Cookies
{
    public static class CookieContainerCacheExtensions
    {
        private static string ClientName<TClient>() where TClient : class
            => typeof(TClient).Name;

        /// <summary>
        /// Get or add a <see cref="CookieContainer"/> for a <typeparamref name="TClient"/> and a cookie context key
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<CookieContainer> GetOrAdd<TClient>(this ICookieContainerCache cache, string key, CancellationToken token = default)
             where TClient : class
            => cache.GetOrAdd(ClientName<TClient>(), key, token);

        /// <summary>
        /// Add or update a <see cref="CookieContainer"/> for a <typeparamref name="TClient"/> and a cookie context key
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task AddOrUpdate<TClient>(this ICookieContainerCache cache, string key, CookieContainer value, CancellationToken token = default)
            where TClient : class
            => cache.AddOrUpdate(ClientName<TClient>(), key, value, token);

        /// <summary>
        /// Reset a <see cref="CookieContainer"/> for a <typeparamref name="TClient"/> and a cookie context key
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task Reset<TClient>(this ICookieContainerCache cache, string key, CancellationToken token = default)
            where TClient : class
            => cache.Reset(ClientName<TClient>(), key, token);
    }

    public interface ICookieContainerCache
    {
        /// <summary>
        /// Get or add a <see cref="CookieContainer"/> for a named <see cref="HttpClient"/> and a cookie context key
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<CookieContainer> GetOrAdd(string clientName, string key, CancellationToken token = default);

        /// <summary>
        /// Add or update a <see cref="CookieContainer"/> for a named <see cref="HttpClient"/> and a cookie context key
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task AddOrUpdate(string clientName, string key, CookieContainer value, CancellationToken token = default);

        /// <summary>
        /// Reset a <see cref="CookieContainer"/> for a named <see cref="HttpClient"/> and a cookie context key
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task Reset(string clientName, string key, CancellationToken token = default);
    }
}
