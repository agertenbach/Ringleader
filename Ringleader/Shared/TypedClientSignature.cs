using Ringleader.Shared.Internal;
using System.Net.Http;

namespace Ringleader.Shared
{
    /// <summary>
    /// Represents the string signature of a typed <see cref="HttpClient"/> according to <see cref="Microsoft.Extensions.Http.DefaultHttpClientFactory"/> conventions
    /// </summary>
    /// <param name="Name"></param>
    public record struct TypedClientSignature(string Name)
    {
        /// <summary>
        /// Create a signature for <typeparamref name="TClient"/> consistent with <see cref="Microsoft.Extensions.Http.DefaultHttpClientFactory"/> conventions
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <returns></returns>
        public static TypedClientSignature For<TClient>()
            => new(ClientSignature<TClient>());

        /// <summary>
        /// Does the signature match the <typeparamref name="TClient"/> typed client according to <see cref="Microsoft.Extensions.Http.DefaultHttpClientFactory"/> conventions?
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <returns></returns>
        public readonly bool IsTypedClient<TClient>()
            => Name.Equals(ClientSignature<TClient>());


        /// <summary>
        /// Return a string display name for <typeparamref name="TClient"/> according to <see cref="Microsoft.Extensions.Http.DefaultHttpClientFactory"/> conventions
        /// </summary>
        /// <see href="https://github.com/dotnet/runtime/blob/c1ae962610643a3c17bcf9621017ac42ff7703f7/src/libraries/Microsoft.Extensions.Http/src/DependencyInjection/HttpClientFactoryServiceCollectionExtensions.cs#L208"/>
        /// <typeparam name="TClient"></typeparam>
        /// <returns></returns>
        private static string ClientSignature<TClient>()
            => LocalTypeNameHelper.GetTypeDisplayName(typeof(TClient), fullName: false);

        public static implicit operator string(TypedClientSignature c) => c.Name;
        public static implicit operator TypedClientSignature(string s) => new(s);

        public override readonly string ToString() => Name;
    }
}
