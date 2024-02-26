using Microsoft.AspNetCore.Mvc;
using Ringleader.Cookies;

namespace WebExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IContextualHttpClientFactory _contextClientFactory;
        private readonly ICookieContainerCache _cookies;

        public TestController(ILogger<TestController> logger, IContextualHttpClientFactory contextClientFactory, ICookieContainerCache cookies)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextClientFactory = contextClientFactory ?? throw new ArgumentNullException(nameof(contextClientFactory));
            _cookies = cookies ?? throw new ArgumentNullException(nameof(cookies));
        }

        [HttpGet]
        public async Task<ActionResult> Get(CancellationToken token)
        {
            // A context can be based on pretty much any property about a request that needs to be made.
            // A random int is a dynamic example.
            int handlerContextInt = Random.Shared.Next(0, 1);

            // A typed client is resolved from the factory, and we convert our context into a string however we choose.
            // If we didn't supply the function, it would use ToString();
            var typedClient = _contextClientFactory.CreateClient<ExampleTypedClient, int>(handlerContextInt, handlerContextResolver: x => x == 1 ? "cert1" : "no-cert");

            // Our typed client was independently registered with support for cookie container context
            // We introduced this in the implementation to be based on a user ID parameter
            Guid userId = Guid.NewGuid();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.example.com");
            var response = await typedClient.SendAsync(request, userId, token);

            // This container should match the one that was attached to the above request
            var cookieContainer = await _cookies.GetOrAdd<ExampleTypedClient>(userId.ToString(), token);
            string cookieHeader = cookieContainer.GetCookieHeader(new Uri("https://www.example.com"));

            return Ok();
        }
    }
}
