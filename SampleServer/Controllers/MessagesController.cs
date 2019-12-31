using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SampleServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly ClientStore _clientStore;
        private readonly HubProxyOptions _hubProxyOptions;

        public MessagesController(IHttpClientFactory httpClientFactory, ClientStore clientStore, IOptions<HubProxyOptions> hubProxyOptions)
        {
            _httpClientFactory = httpClientFactory;
            _clientStore = clientStore;
            _hubProxyOptions = hubProxyOptions.Value;
        }

        [HttpPost("hello")]
        public async Task Hello(
            [FromHeader(Name = "x-connection-id")] string connectionId,
            [FromHeader(Name = "x-hub")] string hub,
            [FromHeader(Name = "x-user")] string user)
        {
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = _hubProxyOptions.HubProxyUrl;
            await client.PostAsync($"hub/clients/{connectionId}/message", new StringContent(JsonSerializer.Serialize(new { arg0 = $"Hello {connectionId}." })).WithContentType());
        }

        [HttpPost("discover")]
        public async Task Discover(
            [FromHeader(Name = "x-connection-id")] string connectionId,
            [FromHeader(Name = "x-hub")] string hub,
            [FromHeader(Name = "x-user")] string user)
        {
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = _hubProxyOptions.HubProxyUrl;
            await client.PostAsync($"hub/clients/{connectionId}/clients", new StringContent(JsonSerializer.Serialize(new { arg0 = _clientStore.GetConnections() })).WithContentType());
        }

        [HttpPost("chatwith")]
        public async Task ChatWith(
            [FromHeader(Name = "x-connection-id")] string connectionId,
            [FromHeader(Name = "x-hub")] string hub,
            [FromHeader(Name = "x-user")] string user,
            [FromBody] Dictionary<string, object> body)
        {
            var groupId = Guid.NewGuid().ToString("N");
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = _hubProxyOptions.HubProxyUrl;
            await client.PutAsync($"hub/groups/{groupId}/clients/{connectionId}", new ByteArrayContent(Array.Empty<byte>()));
            foreach (var id in ((JsonElement)body["arg0"]).EnumerateArray())
            {
                await client.PutAsync($"hub/groups/{groupId}/clients/{id.GetString()}", new ByteArrayContent(Array.Empty<byte>()));
            }
            var sc = new StringContent(JsonSerializer.Serialize(new { arg0 = groupId })).WithContentType();
            await client.PostAsync($"hub/clients/{connectionId}/join", sc);
            foreach (var id in ((JsonElement)body["arg0"]).EnumerateArray())
            {
                await client.PostAsync($"hub/clients/{id.GetString()}/join", sc);
            }
        }

        [HttpPost("chat")]
        public async Task Chat(
            [FromHeader(Name = "x-connection-id")] string connectionId,
            [FromHeader(Name = "x-hub")] string hub,
            [FromHeader(Name = "x-user")] string user,
            [FromBody] Dictionary<string, object> body)
        {
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = _hubProxyOptions.HubProxyUrl;
            await client.PostAsync(
                $"hub/groups/{((JsonElement)body["arg0"]).GetString()}/chat",
                new StringContent(JsonSerializer.Serialize(new { arg0 = body["arg0"], arg1 = connectionId, arg2 = body["arg1"] })).WithContentType());
        }
    }
}
