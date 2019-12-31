using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace HubProxy
{
    public class HubProxyInvoker : IHubProxyInvoker
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HubProxyOptions _options;

        public HubProxyInvoker(IHttpClientFactory httpClientFactory, IOptions<HubProxyOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public Uri BaseAddress => _options.ForwardTo;

        public virtual string GetHubName(HubConnectionContext hubConnectionContext) =>
            hubConnectionContext.Features.Get<IHttpContextFeature>().HttpContext.Request.Path;

        public virtual async Task Invoke(HubConnectionContext hubConnectionContext, string methodName, object[] arguments)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = BaseAddress;
            string text;
            switch (_options.Formatter)
            {
                case ArgumentsFormatter.Object:
                    var dict = new Dictionary<string, object>();
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        dict.Add($"arg{i}", arguments[i]);
                    }
                    text = JsonSerializer.Serialize(dict);
                    break;
                case ArgumentsFormatter.Array:
                default:
                    text = JsonSerializer.Serialize(arguments);
                    break;
            }
            var content = new StringContent(text);
            content.Headers.Add("x-hub", GetHubName(hubConnectionContext));
            content.Headers.Add("x-connection-id", hubConnectionContext.ConnectionId);
            content.Headers.Add("x-user", hubConnectionContext.UserIdentifier);
            content.Headers.ContentType.MediaType = "application/json";
            await httpClient.PostAsync($"messages/{methodName}", content);
        }

        public virtual async Task OnConnected(HubConnectionContext hubConnectionContext)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = BaseAddress;
            var content = new ByteArrayContent(Array.Empty<byte>());
            content.Headers.Add("x-hub", GetHubName(hubConnectionContext));
            content.Headers.Add("x-user", hubConnectionContext.UserIdentifier);
            await httpClient.PutAsync($"clients/{hubConnectionContext.ConnectionId}", content);
        }

        public virtual async Task OnDisconnected(HubConnectionContext hubConnectionContext, Exception ex)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = BaseAddress;
            var content = new ByteArrayContent(Array.Empty<byte>());
            content.Headers.Add("x-hub", GetHubName(hubConnectionContext));
            content.Headers.Add("x-user", hubConnectionContext.UserIdentifier);
            await httpClient.DeleteAsync($"clients/{hubConnectionContext.ConnectionId}");
        }
    }
}
